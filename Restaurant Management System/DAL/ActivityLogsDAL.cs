using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System.DAL
{
    public static class ActivityLogsDAL
    {
        private const string Conn =
            "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        public static void AddLog(string eventName, string description, string? username = null)
        {
            using var conn = new SqlConnection(Conn);
            conn.Open();

            using var cmd = new SqlCommand(
                "INSERT INTO dbo.ActivityLogs (EventName, Description, Username) " +
                "VALUES (@e,@d,@u)", conn);
            cmd.Parameters.AddWithValue("@e", eventName);
            cmd.Parameters.AddWithValue("@d", description);
            cmd.Parameters.AddWithValue("@u", (object?)username ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public static DataTable GetLogs(int top = 500)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(Conn);
            conn.Open();

            string sql = $"SELECT TOP {top} LogId, EventName, Description, Username, LogTime " +
                         "FROM dbo.ActivityLogs ORDER BY LogTime DESC";

            new SqlDataAdapter(sql, conn).Fill(dt);
            return dt;
        }
    }
}
