using System;
using Restaurant_Management_System.DAL;

namespace Restaurant_Management_System
{
    /// <summary>Lightweight helper to record an application event.</summary>
    internal static class AppLog
    {
        /// <param name="eventName">Short tag, e.g. "PAY_ORDER"</param>
        /// <param name="description">Free‑text details</param>
        /// <param name="username">Optional current user</param>
        public static void Write(string eventName, string description, string? username = null)
        {
            try
            {
                ActivityLogsDAL.AddLog(eventName, description, username);
            }
            catch (Exception ex)
            {
                // last‑chance fallback: never crash the app because logging failed
                System.Diagnostics.Debug.WriteLine("LOG‑FAIL: " + ex.Message);
            }
        }
    }
}
