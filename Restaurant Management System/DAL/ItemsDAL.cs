using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Restaurant_Management_System.DAL
{
    public static class ItemsDAL
    {
        public static DataTable GetMenuItems(string connectionString)
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                var dt = new DataTable();
                try
                {
                    conn.Open();
                    var cmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT ItemId, Name, Price FROM MenuItems", conn);
                    using (var da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading menu items: " + ex.Message);
                }
                return dt;
            }
        }

        public static void AddMenuItem(string connectionString, string name, decimal price)
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new Microsoft.Data.SqlClient.SqlCommand("INSERT INTO MenuItems (Name, Price) VALUES (@name, @price)", conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding menu item: " + ex.Message);
                }
            }
        }

        public static void UpdateMenuItem(string connectionString, int itemId, string name, decimal price)
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new Microsoft.Data.SqlClient.SqlCommand("UPDATE MenuItems SET Name = @name, Price = @price WHERE ItemId = @id", conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating menu item: " + ex.Message);
                }
            }
        }

        public static void DeleteMenuItem(string connectionString, int itemId)
        {
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new Microsoft.Data.SqlClient.SqlCommand("DELETE FROM MenuItems WHERE ItemId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", itemId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting menu item: " + ex.Message);
                }
            }
        }
    }
}