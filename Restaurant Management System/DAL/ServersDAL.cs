using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Restaurant_Management_System.DAL
{
    public static class ServersDAL
    {
        /* -----------------------------------------------------------
         *  READ
         * --------------------------------------------------------- */
        public static DataTable GetServers(string connectionString)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ServerId, Name FROM Servers ORDER BY Name", conn);
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading servers: " + ex.Message);
            }
            return dt;
        }

        /* -----------------------------------------------------------
         *  CREATE
         * --------------------------------------------------------- */
        public static void AddServer(string connectionString, string name)
        {
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Servers (Name) VALUES (@name)", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding server: " + ex.Message);
            }
        }

        /* -----------------------------------------------------------
         *  DELETE  (now clears references in Orders first)
         * --------------------------------------------------------- */
        public static void DeleteServer(string connectionString, int serverId)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();
            try
            {
                /* 1️⃣  Clear references in Orders                */
                var clear = new SqlCommand(
                    "UPDATE Orders SET ServerId = NULL WHERE ServerId = @id",
                    conn, tx);
                clear.Parameters.AddWithValue("@id", serverId);
                clear.ExecuteNonQuery();

                /* 2️⃣  Delete the server row                     */
                var del = new SqlCommand(
                    "DELETE FROM Servers WHERE ServerId = @id",
                    conn, tx);
                del.Parameters.AddWithValue("@id", serverId);
                int rows = del.ExecuteNonQuery();

                if (rows == 0)
                {
                    tx.Rollback();
                    MessageBox.Show("Server not found or already deleted.");
                    return;
                }

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
