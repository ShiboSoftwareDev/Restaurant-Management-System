using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Restaurant_Management_System.DAL
{
    public static class ClientsDAL
    {
        public static DataTable GetClients(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var dt = new DataTable();
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ClientId, Name, LoyaltyPoints FROM Clients", conn);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading clients: " + ex.Message);
                }
                return dt;
            }
        }

        public static void AddClient(string connectionString, string name)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("INSERT INTO Clients (Name) VALUES (@name)", conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding client: " + ex.Message);
                }
            }
        }

        public static void DeleteClient(string connectionString, int clientId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Clients WHERE ClientId = @id", conn);
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
}