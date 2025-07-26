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

        /* -----------------------------------------------------------
         *  READ – only show active (IsDeleted = 0)
         * --------------------------------------------------------- */
        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT UserId, Username, IsAdmin, IsLocked, IsDeleted " +
                    "FROM Users " +
                    "WHERE IsDeleted = 0 " +
                    "ORDER BY Username", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId    = reader.GetInt32(0),
                            Username  = reader.GetString(1),
                            IsAdmin   = reader.GetBoolean(2),
                            IsLocked  = reader.GetBoolean(3),
                            IsDeleted = reader.GetBoolean(4)
                        });
                    }
                }
            }
            return users;
        }

        /* -----------------------------------------------------------
         *  CREATE – if username exists but soft-deleted, revive it
         *            (set IsDeleted = 0 and update password hash)
         * --------------------------------------------------------- */
        public static void AddUser(string username, string passwordHash, bool isAdmin)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using var tx = conn.BeginTransaction();

                try
                {
                    // 1) Try to revive a soft-deleted user with same username
                    var reviveCmd = new SqlCommand(
                        @"UPDATE Users
                          SET IsDeleted = 0,
                              PasswordHash = @pass,
                              IsAdmin = @admin,
                              IsLocked = 0
                          WHERE Username = @user AND IsDeleted = 1",
                        conn, tx);
                    reviveCmd.Parameters.AddWithValue("@user", username);
                    reviveCmd.Parameters.AddWithValue("@pass", passwordHash);
                    reviveCmd.Parameters.AddWithValue("@admin", isAdmin);

                    int revived = reviveCmd.ExecuteNonQuery();
                    if (revived == 0)
                    {
                        // 2) No soft-deleted row to revive, insert new
                        var insertCmd = new SqlCommand(
                            @"INSERT INTO Users (Username, PasswordHash, IsAdmin, IsLocked, IsDeleted)
                              VALUES (@user, @pass, @admin, 0, 0)",
                            conn, tx);
                        insertCmd.Parameters.AddWithValue("@user", username);
                        insertCmd.Parameters.AddWithValue("@pass", passwordHash);
                        insertCmd.Parameters.AddWithValue("@admin", isAdmin);
                        insertCmd.ExecuteNonQuery();
                    }
                    
                    AppLog.Write("USER_ADDED", $"User '{username}' added or revived.", username);
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public static void UpdateUserAdmin(int userId, bool isAdmin)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Users SET IsAdmin = @admin WHERE UserId = @id AND IsDeleted = 0", conn);
                cmd.Parameters.AddWithValue("@admin", isAdmin);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
                AppLog.Write("USER_UPDATED", $"User ID {userId} admin status updated to {isAdmin}.", null);
            }
        }

        public static void UpdateUserLocked(int userId, bool isLocked)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Users SET IsLocked = @locked WHERE UserId = @id AND IsDeleted = 0", conn);
                cmd.Parameters.AddWithValue("@locked", isLocked);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
                AppLog.Write("USER_UPDATED", $"User ID {userId} locked status updated to {isLocked}.", null);
            }
        }

        /* -----------------------------------------------------------
         *  SOFT DELETE – set IsDeleted = 1 (do NOT actually delete)
         * --------------------------------------------------------- */
        public static void DeleteUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Users SET IsDeleted = 1 WHERE UserId = @id AND IsDeleted = 0", conn);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
                AppLog.Write("USER_DELETED", $"User ID {userId} soft-deleted.", null);
            }
        }

        public static bool IsAdmin(string username)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT IsAdmin FROM Users WHERE Username = @u AND IsDeleted = 0", conn);
            cmd.Parameters.AddWithValue("@u", username);
            var o = cmd.ExecuteScalar();
            return o != null && o != DBNull.Value && (bool)o;
        }
    }
}
