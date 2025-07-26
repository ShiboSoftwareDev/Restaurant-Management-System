using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class OrdersDAL
    {
        private const string Conn =
            "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        /* ===========================================================
         *  1. READ  — active orders with discount preview
         * ========================================================= */
        public static DataTable GetActiveOrders()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(Conn);
            conn.Open();

            const string sql = @"
                WITH agg AS (
                    SELECT  o.OrderId,
                            SUM(mi.Price * oi.Quantity) AS SubTotal
                    FROM dbo.OrderItems oi
                    JOIN dbo.MenuItems  mi ON mi.ItemId = oi.ItemId
                    JOIN dbo.Orders     o  ON o.OrderId = oi.OrderId
                    WHERE o.Progress < 3
                    GROUP BY o.OrderId
                )
                SELECT  o.OrderId,
                        o.TableId                       AS TableNumber,
                        c.Name                          AS ClientName,
                        ISNULL(s.Name,'')               AS Server,
                        o.Progress,
                        CASE o.Progress
                             WHEN 0 THEN N'Pending'
                             WHEN 1 THEN N'Ready'
                             WHEN 2 THEN N'Paid'
                        END                             AS Status,
                        ISNULL(
                            STRING_AGG(
                                mi.Name + CASE WHEN oi.Quantity>1
                                               THEN ' x'+CONVERT(varchar,oi.Quantity)
                                               ELSE '' END, ', '
                            ), N'--')                   AS OrderItems,
                        CASE WHEN ISNULL(c.LoyaltyPoints,0) >= 4
                             THEN 'Yes' ELSE 'No' END   AS DiscountFlag,
                        CASE WHEN ISNULL(c.LoyaltyPoints,0) >= 4
                             THEN CAST(ISNULL(a.SubTotal,0)*0.9 AS decimal(10,2))
                             ELSE ISNULL(a.SubTotal,0) END       AS TotalPrice,
                        o.OrderTime
                FROM  dbo.Orders            o
                LEFT  JOIN dbo.Clients      c  ON c.ClientId  = o.ClientId
                LEFT  JOIN dbo.Servers      s  ON s.ServerId  = o.ServerId
                LEFT  JOIN dbo.OrderItems   oi ON oi.OrderId  = o.OrderId
                LEFT  JOIN dbo.MenuItems    mi ON mi.ItemId   = oi.ItemId
                LEFT  JOIN agg              a  ON a.OrderId   = o.OrderId
                WHERE o.Progress < 3
                GROUP BY o.OrderId, o.TableId, c.Name, s.Name,
                         o.Progress, a.SubTotal, c.LoyaltyPoints,
                         o.OrderTime
                ORDER BY o.OrderTime DESC;";

            new SqlDataAdapter(sql, conn).Fill(dt);
            return dt;
        }

        public static DataTable GetMenuItems()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(Conn);
            conn.Open();
            new SqlDataAdapter(
                "SELECT ItemId, Name, Price FROM dbo.MenuItems WHERE IsDeleted = 0 ORDER BY Name",
                conn).Fill(dt);
            return dt;
        }

        /* ===========================================================
         *  2. ORDER CREATION & BASIC OPS
         * ========================================================= */
        public static int CreateOrder(int tableId, string clientName)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            /* prohibit duplicate active order */
            using (var chk = new SqlCommand(
                   "IF EXISTS (SELECT 1 FROM dbo.Orders WHERE TableId=@T AND Progress<3) "
                 + "RAISERROR ('Table already in use.',16,1);", conn, tx))
            {
                chk.Parameters.AddWithValue("@T", tableId);
                chk.ExecuteNonQuery();
            }

            /* ensure table row exists */
            new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM dbo.Tables WHERE TableId=@T)
                BEGIN
                    SET IDENTITY_INSERT dbo.Tables ON;
                    INSERT INTO dbo.Tables (TableId, Status) VALUES (@T,'Available');
                    SET IDENTITY_INSERT dbo.Tables OFF;
                END;", conn, tx)
            { Parameters = { new SqlParameter("@T", tableId) } }.ExecuteNonQuery();

            /* client upsert */
            var cmdCli = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM dbo.Clients WHERE Name=@N AND IsDeleted=0)
                    SELECT ClientId FROM dbo.Clients WHERE Name=@N AND IsDeleted=0
                ELSE
                    INSERT INTO dbo.Clients (Name,LoyaltyPoints)
                    OUTPUT INSERTED.ClientId VALUES (@N,0);", conn, tx);
            cmdCli.Parameters.AddWithValue("@N", clientName);
            int clientId = Convert.ToInt32(cmdCli.ExecuteScalar());

            /* create order */
            var cmdOrd = new SqlCommand(@"
                INSERT INTO dbo.Orders
                      (TableId, ClientId, OrderStatus, Progress,
                       OrderTime, TotalPrice)
                OUTPUT INSERTED.OrderId
                VALUES (@T,@C,'Pending',0,@Now,0);", conn, tx);
            cmdOrd.Parameters.AddWithValue("@T",   tableId);
            cmdOrd.Parameters.AddWithValue("@C",   clientId);
            cmdOrd.Parameters.AddWithValue("@Now", DateTime.Now);

            int orderId = Convert.ToInt32(cmdOrd.ExecuteScalar());
            tx.Commit();
            return orderId;
        }

        public static void AssignServer(int orderId, int serverId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Orders SET ServerId=@S WHERE OrderId=@O", conn);
            cmd.Parameters.AddWithValue("@S", serverId);
            cmd.Parameters.AddWithValue("@O", orderId);
            cmd.ExecuteNonQuery();
        }

        /* add / update items (quantity dictionary) */
        public static void AddItems(int orderId, Dictionary<int, int> items)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderId=@o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();

            var ins = new SqlCommand(
                "INSERT INTO dbo.OrderItems (OrderId, ItemId, Quantity) VALUES (@o,@i,@q)",
                conn, tx);
            ins.Parameters.Add("@o", System.Data.SqlDbType.Int).Value = orderId;
            ins.Parameters.Add("@i", System.Data.SqlDbType.Int);
            ins.Parameters.Add("@q", System.Data.SqlDbType.Int);

            foreach (var (id, qty) in items)
            {
                ins.Parameters["@i"].Value = id;
                ins.Parameters["@q"].Value = qty;
                ins.ExecuteNonQuery();
            }

            new SqlCommand(@"
                UPDATE o SET TotalPrice =
                (SELECT SUM(mi.Price*oi.Quantity)
                 FROM dbo.OrderItems oi
                 JOIN dbo.MenuItems mi ON mi.ItemId = oi.ItemId
                 WHERE oi.OrderId=@o)
                FROM dbo.Orders o WHERE o.OrderId=@o", conn, tx)
            { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();

            tx.Commit();
        }

        /* ===========================================================
         *  3. PAY ORDER  (single transaction)
         * ========================================================= */
        public static (decimal finalTotal, bool discountApplied) PayOrder(int orderId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            /* fetch client & current total */
            int clientId;
            decimal total;
            using (var r = new SqlCommand(
                   "SELECT ClientId, ISNULL(TotalPrice,0) FROM dbo.Orders WHERE OrderId=@o",
                   conn, tx)
                   { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteReader())
            {
                if (!r.Read()) throw new InvalidOperationException("Order not found.");
                clientId = r.GetInt32(0);
                total    = r.GetDecimal(1);
            }

            /* get / update loyalty */
            int points;
            using (var cmd = new SqlCommand(
                   "SELECT LoyaltyPoints FROM dbo.Clients WHERE ClientId=@c", conn, tx))
            {
                cmd.Parameters.AddWithValue("@c", clientId);
                points = (int)cmd.ExecuteScalar();
            }

            bool discount = points >= 4;
            if (discount)
            {
                total *= 0.9m;
                points = 0;
            }
            else
            {
                points += 1;
            }

            using (var cmd = new SqlCommand(
                   "UPDATE dbo.Clients SET LoyaltyPoints=@p WHERE ClientId=@c", conn, tx))
            {
                cmd.Parameters.AddWithValue("@p", points);
                cmd.Parameters.AddWithValue("@c", clientId);
                cmd.ExecuteNonQuery();
            }

            /* mark paid (for auditing) */
            using (var cmd = new SqlCommand(
                   "UPDATE dbo.Orders SET Progress=2, OrderStatus='Paid', TotalPrice=@t WHERE OrderId=@o",
                   conn, tx))
            {
                cmd.Parameters.AddWithValue("@t", total);
                cmd.Parameters.AddWithValue("@o", orderId);
                cmd.ExecuteNonQuery();
            }

            /* remove rows */
            new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderId=@o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();
            new SqlCommand("DELETE FROM dbo.Orders WHERE OrderId=@o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();

            tx.Commit();
            return (total, discount);
        }

        /* ===========================================================
         *  4. SMALL HELPERS (still used elsewhere)
         * ========================================================= */
        public static void UpdateProgress(int orderId, int progress, string status)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Orders SET Progress=@p, OrderStatus=@s WHERE OrderId=@o", conn);
            cmd.Parameters.AddWithValue("@p", progress);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@o", orderId);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteOrder(int orderId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderId=@o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();
            new SqlCommand("DELETE FROM dbo.Orders WHERE OrderId=@o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();

            tx.Commit();
        }

        public static int GetClientId(int orderId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT ClientId FROM dbo.Orders WHERE OrderId=@o", conn);
            cmd.Parameters.AddWithValue("@o", orderId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int GetClientPoints(int clientId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT LoyaltyPoints FROM dbo.Clients WHERE ClientId=@c", conn);
            cmd.Parameters.AddWithValue("@c", clientId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
