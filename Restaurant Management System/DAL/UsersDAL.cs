using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Restaurant_Management_System.Models;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class UsersDAL
    {
        private static string connectionString = "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT UserId, Username, IsAdmin, IsLocked FROM Users", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            IsAdmin = reader.GetBoolean(2),
                            IsLocked = reader.GetBoolean(3)
                        });
                    }
                }
            }
            return users;
        }

        public static void AddUser(string username, string passwordHash, bool isAdmin)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, IsAdmin, IsLocked) VALUES (@user, @pass, @admin, 0)", conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", passwordHash);
                cmd.Parameters.AddWithValue("@admin", isAdmin);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateUserAdmin(int userId, bool isAdmin)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Users SET IsAdmin = @admin WHERE UserId = @id", conn);
                cmd.Parameters.AddWithValue("@admin", isAdmin);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateUserLocked(int userId, bool isLocked)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Users SET IsLocked = @locked WHERE UserId = @id", conn);
                cmd.Parameters.AddWithValue("@locked", isLocked);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @id", conn);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}