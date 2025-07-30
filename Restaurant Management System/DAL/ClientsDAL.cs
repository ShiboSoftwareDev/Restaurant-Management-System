using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class ClientsDAL
    {
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
        public static void AddClient(string connectionString, string name)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                var existsCmd = new SqlCommand(
                    "SELECT 1 FROM Clients WHERE Name = @name AND IsDeleted = 0",
                    conn, tx);
                existsCmd.Parameters.AddWithValue("@name", name);

                bool activeExists = existsCmd.ExecuteScalar() != null;
                if (activeExists)
                {
                    MessageBox.Show("A client with this name already exists.");
                    tx.Rollback();
                    return;
                }

                var reviveCmd = new SqlCommand(
                    @"UPDATE Clients
                      SET IsDeleted      = 0,
                          LoyaltyPoints  = 0
                      WHERE Name = @name AND IsDeleted = 1",
                    conn, tx);
                reviveCmd.Parameters.AddWithValue("@name", name);

                int revived = reviveCmd.ExecuteNonQuery();

                if (revived == 0)
                {
                    var insertCmd = new SqlCommand(
                        @"INSERT INTO Clients (Name, LoyaltyPoints, IsDeleted)
                          VALUES (@name, 0, 0)",
                        conn, tx);
                    insertCmd.Parameters.AddWithValue("@name", name);
                    insertCmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            {
                tx.Rollback();
                MessageBox.Show("Client name must be unique.");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error adding client: " + ex.Message);
            }
        }

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
