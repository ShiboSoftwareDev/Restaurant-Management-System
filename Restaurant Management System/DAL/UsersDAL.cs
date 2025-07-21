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
                var cmd = new SqlCommand("SELECT UserId, Username, IsAdmin FROM Users", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            IsAdmin = reader.GetBoolean(2)
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
                var cmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, IsAdmin) VALUES (@user, @pass, @admin)", conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", passwordHash);
                cmd.Parameters.AddWithValue("@admin", isAdmin);
                cmd.ExecuteNonQuery();
            }
        }

        // Add more DAL methods as needed (e.g., DeleteUser, UpdateUser, etc.)
    }
}