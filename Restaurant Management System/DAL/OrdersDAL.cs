using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class OrdersDAL
    {
        private const string Conn =
            "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        /* -----------------------------------------------------------
         *  READ
         * --------------------------------------------------------- */
        public static DataTable GetActiveOrders()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(Conn);
            conn.Open();

            var sql = @"
                SELECT o.OrderId,
                       o.TableId                       AS TableNumber,
                       c.Name                          AS ClientName,
                       ISNULL(s.Name,'')               AS Server,
                       o.Progress,
                       CASE o.Progress
                           WHEN 0 THEN N'Pending'
                           WHEN 1 THEN N'Ready'
                           WHEN 2 THEN N'Paid'
                       END                             AS Status,
                       ISNULL(STRING_AGG(mi.Name, ', '), N'--') AS OrderItems,
                       ISNULL(o.TotalPrice,0)          AS TotalPrice,
                       o.OrderTime
                FROM dbo.Orders            o
                LEFT JOIN dbo.Clients      c  ON c.ClientId  = o.ClientId
                LEFT JOIN dbo.Servers      s  ON s.ServerId  = o.ServerId
                LEFT JOIN dbo.OrderItems   oi ON oi.OrderId  = o.OrderId
                LEFT JOIN dbo.MenuItems    mi ON mi.ItemId   = oi.ItemId
                WHERE o.Progress < 3
                GROUP BY o.OrderId, o.TableId, c.Name, s.Name,
                         o.Progress, o.TotalPrice, o.OrderTime
                ORDER BY o.OrderTime DESC;";

            new SqlDataAdapter(sql, conn).Fill(dt);
            return dt;
        }

        /* -----------------------------------------------------------
         *  CREATE ORDER
         * --------------------------------------------------------- */
        public static int CreateOrder(int tableId, string clientName)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM dbo.Tables WHERE TableId = @T)
                BEGIN
                    SET IDENTITY_INSERT dbo.Tables ON;
                    INSERT INTO dbo.Tables (TableId, Status) VALUES (@T, 'Available');
                    SET IDENTITY_INSERT dbo.Tables OFF;
                END;", conn, tx)
            { Parameters = { new SqlParameter("@T", tableId) } }.ExecuteNonQuery();

            var cmdCli = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM dbo.Clients WHERE Name = @Name)
                    SELECT ClientId FROM dbo.Clients WHERE Name = @Name
                ELSE
                    INSERT INTO dbo.Clients (Name, LoyaltyPoints)
                    OUTPUT INSERTED.ClientId VALUES (@Name, 0);", conn, tx);
            cmdCli.Parameters.AddWithValue("@Name", clientName);
            int clientId = Convert.ToInt32(cmdCli.ExecuteScalar());

            var cmdOrd = new SqlCommand(@"
                INSERT INTO dbo.Orders
                      (TableId, ClientId, OrderStatus, Progress,
                       OrderTime, TotalPrice)
                OUTPUT INSERTED.OrderId
                VALUES (@Table, @Client, 'Pending', 0, @Now, 0);", conn, tx);
            cmdOrd.Parameters.AddWithValue("@Table",  tableId);
            cmdOrd.Parameters.AddWithValue("@Client", clientId);
            cmdOrd.Parameters.AddWithValue("@Now",    DateTime.Now);

            int orderId = Convert.ToInt32(cmdOrd.ExecuteScalar());

            tx.Commit();
            return orderId;
        }

        /* -----------------------------------------------------------
         *  ASSIGN SERVER
         * --------------------------------------------------------- */
        public static void AssignServer(int orderId, int serverId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Orders SET ServerId = @S WHERE OrderId = @O", conn);
            cmd.Parameters.AddWithValue("@S", serverId);
            cmd.Parameters.AddWithValue("@O", orderId);
            cmd.ExecuteNonQuery();
        }

        /* -----------------------------------------------------------
         *  UPDATE PROGRESS
         * --------------------------------------------------------- */
        public static void UpdateProgress(int orderId, int progress, string status)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Orders SET Progress = @p, OrderStatus = @s WHERE OrderId = @o", conn);
            cmd.Parameters.AddWithValue("@p", progress);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@o", orderId);
            cmd.ExecuteNonQuery();
        }

        /* -----------------------------------------------------------
         *  DELETE ORDER
         * --------------------------------------------------------- */
        public static void DeleteOrder(int orderId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var tx = conn.BeginTransaction();

            new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderId = @o", conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();
            new SqlCommand("DELETE FROM dbo.Orders WHERE OrderId = @o",     conn, tx)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteNonQuery();

            tx.Commit();
        }

        /* -----------------------------------------------------------
         *  CLIENT HELPERS
         * --------------------------------------------------------- */
        public static int GetClientId(int orderId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT ClientId FROM dbo.Orders WHERE OrderId = @o", conn);
            cmd.Parameters.AddWithValue("@o", orderId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int GetClientPoints(int clientId)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT LoyaltyPoints FROM dbo.Clients WHERE ClientId = @c", conn);
            cmd.Parameters.AddWithValue("@c", clientId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /* ---------- FIXED METHOD ---------- */
        public static void UpdateClientPoints(int clientId, int points)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Clients SET LoyaltyPoints = @p WHERE ClientId = @c", conn);
            cmd.Parameters.AddWithValue("@p", points);
            cmd.Parameters.AddWithValue("@c", clientId);
            cmd.ExecuteNonQuery();
        }
    }
}
