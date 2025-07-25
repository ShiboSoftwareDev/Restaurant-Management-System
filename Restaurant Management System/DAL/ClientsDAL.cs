using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class ClientsDAL
    {
        /* -----------------------------------------------------------
         *  READ – only active clients (IsDeleted = 0)
         * --------------------------------------------------------- */
        public static DataTable GetClients(string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            var dt = new DataTable();
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ClientId, Name, LoyaltyPoints " +
                    "FROM   Clients " +
                    "WHERE  IsDeleted = 0 " +
                    "ORDER  BY Name", conn);

                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading clients: " + ex.Message);
            }
            return dt;
        }

        /* -----------------------------------------------------------
         *  CREATE – revive if soft‑deleted; otherwise insert new.
         *           When revived, IsDeleted → 0 and LoyaltyPoints → 0
         * --------------------------------------------------------- */
        public static void AddClient(string connectionString, string name)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1️⃣ Attempt to revive an existing soft‑deleted row
                var revive = new SqlCommand(
                    @"UPDATE Clients
                      SET IsDeleted      = 0,
                          LoyaltyPoints  = 0
                      WHERE Name = @name AND IsDeleted = 1",
                    conn, tx);
                revive.Parameters.AddWithValue("@name", name);

                if (revive.ExecuteNonQuery() == 0)
                {
                    // 2️⃣ No match → insert new
                    var insert = new SqlCommand(
                        @"INSERT INTO Clients (Name, LoyaltyPoints, IsDeleted)
                          VALUES (@name, 0, 0)",
                        conn, tx);
                    insert.Parameters.AddWithValue("@name", name);
                    insert.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error adding client: " + ex.Message);
            }
        }

        /* -----------------------------------------------------------
         *  SOFT DELETE – mark row as deleted
         * --------------------------------------------------------- */
        public static void DeleteClient(string connectionString, int clientId)
        {
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Clients " +
                    "SET    IsDeleted = 1 " +
                    "WHERE  ClientId = @id AND IsDeleted = 0",
                    conn);
                cmd.Parameters.AddWithValue("@id", clientId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting client: " + ex.Message);
            }
        }
    }
}
