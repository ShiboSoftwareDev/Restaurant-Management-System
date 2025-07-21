using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Restaurant_Management_System.DAL
{
    public static class ServersDAL
    {
        public static DataTable GetServers(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var dt = new DataTable();
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ServerId, Name FROM Servers", conn);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading servers: " + ex.Message);
                }
                return dt;
            }
        }

        public static void AddServer(string connectionString, string name)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("INSERT INTO Servers (Name) VALUES (@name)", conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding server: " + ex.Message);
                }
            }
        }

        public static void DeleteServer(string connectionString, int serverId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Servers WHERE ServerId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", serverId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting server: " + ex.Message);
                }
            }
        }
    }
}
