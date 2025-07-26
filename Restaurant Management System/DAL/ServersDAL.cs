using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class ServersDAL
    {
        /* -----------------------------------------------------------
         *  READ – only active (IsDeleted = 0)
         * --------------------------------------------------------- */
        public static DataTable GetServers(string connectionString)
        {
            var dt = new DataTable();

            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ServerId, Name " +
                    "FROM   Servers " +
                    "WHERE  IsDeleted = 0 " +
                    "ORDER  BY Name", conn);

                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading servers: " + ex.Message);
            }

            return dt;
        }

        /* -----------------------------------------------------------
         *  CREATE  (with restore‑if‑soft‑deleted logic)
         * --------------------------------------------------------- */
        public static void AddServer(string connectionString, string name)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();
            try
            {
                /* 1️⃣  Does a soft‑deleted row with this name exist? */
                var reviveCheck = new SqlCommand(
                    "SELECT ServerId " +
                    "FROM   Servers " +
                    "WHERE  Name = @name AND IsDeleted = 1", conn, tx);
                reviveCheck.Parameters.AddWithValue("@name", name);

                var idObj = reviveCheck.ExecuteScalar();

                if (idObj != null)          // Found a soft‑deleted match – revive it
                {
                    var reviveCmd = new SqlCommand(
                        "UPDATE Servers " +
                        "SET    IsDeleted = 0 " +
                        "WHERE  ServerId = @id", conn, tx);
                    reviveCmd.Parameters.AddWithValue("@id", (int)idObj);
                    reviveCmd.ExecuteNonQuery();

                    tx.Commit();
                    AppLog.Write("SERVER_RESTORED", $"Server '{name}' revived from soft-deletion.", null);
                    return;
                }

                /* 2️⃣  Otherwise, insert a brand‑new row */
                var insert = new SqlCommand(
                    "INSERT INTO Servers (Name, IsDeleted) VALUES (@name, 0)",
                    conn, tx);
                insert.Parameters.AddWithValue("@name", name);
                insert.ExecuteNonQuery();

                AppLog.Write("SERVER_ADDED", $"Server '{name}' added.", null);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error adding server: " + ex.Message);
            }
        }

        /* -----------------------------------------------------------
         *  SOFT DELETE – flag row & clear references
         * --------------------------------------------------------- */
        public static void DeleteServer(string connectionString, int serverId)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();
            try
            {
                /* 1️⃣  Clear references in Orders */
                var clear = new SqlCommand(
                    "UPDATE Orders " +
                    "SET    ServerId = NULL " +
                    "WHERE  ServerId = @id", conn, tx);
                clear.Parameters.AddWithValue("@id", serverId);
                clear.ExecuteNonQuery();

                /* 2️⃣  Flag the server as deleted */
                var del = new SqlCommand(
                    "UPDATE Servers " +
                    "SET    IsDeleted = 1 " +
                    "WHERE  ServerId = @id AND IsDeleted = 0", conn, tx);
                del.Parameters.AddWithValue("@id", serverId);

                if (del.ExecuteNonQuery() == 0)
                {
                    tx.Rollback();
                    MessageBox.Show("Server not found or already deleted.");
                    return;
                }

                AppLog.Write("SERVER_DELETED", $"Server ID {serverId} soft-deleted.", null);
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error deleting server: " + ex.Message);
            }
        }
    }
}
