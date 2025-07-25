using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class ItemsDAL
    {
        /* -----------------------------------------------------------
         *  READ – only active items
         * --------------------------------------------------------- */
        public static DataTable GetMenuItems(string connectionString)
        {
            using var conn = new SqlConnection(connectionString);
            var dt = new DataTable();
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ItemId, Name, Price " +
                    "FROM   MenuItems " +
                    "WHERE  IsDeleted = 0 " +
                    "ORDER  BY Name", conn);

                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading menu items: " + ex.Message);
            }
            return dt;
        }

        /* -----------------------------------------------------------
         *  CREATE – revive if soft‑deleted; otherwise insert new
         * --------------------------------------------------------- */
        public static void AddMenuItem(string connectionString, string name, decimal price)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1️⃣  Try to revive a soft‑deleted row
                var revive = new SqlCommand(
                    @"UPDATE MenuItems
                      SET    IsDeleted = 0,
                             Price     = @price
                      WHERE  Name = @name AND IsDeleted = 1",
                    conn, tx);
                revive.Parameters.AddWithValue("@name",  name);
                revive.Parameters.AddWithValue("@price", price);

                if (revive.ExecuteNonQuery() == 0)
                {
                    // 2️⃣  Insert new
                    var ins = new SqlCommand(
                        @"INSERT INTO MenuItems (Name, Price, IsDeleted)
                          VALUES (@name, @price, 0)",
                        conn, tx);
                    ins.Parameters.AddWithValue("@name",  name);
                    ins.Parameters.AddWithValue("@price", price);
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show("Error adding menu item: " + ex.Message);
            }
        }

        /* -----------------------------------------------------------
         *  UPDATE – only when item is active
         * --------------------------------------------------------- */
        public static void UpdateMenuItem(string connectionString, int itemId, string name, decimal price)
        {
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE MenuItems " +
                    "SET    Name = @name, Price = @price " +
                    "WHERE  ItemId = @id AND IsDeleted = 0", conn);
                cmd.Parameters.AddWithValue("@name",  name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@id",    itemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating menu item: " + ex.Message);
            }
        }

        /* -----------------------------------------------------------
         *  SOFT DELETE – mark as deleted
         * --------------------------------------------------------- */
        public static void DeleteMenuItem(string connectionString, int itemId)
        {
            using var conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE MenuItems " +
                    "SET    IsDeleted = 1 " +
                    "WHERE  ItemId = @id AND IsDeleted = 0", conn);
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
