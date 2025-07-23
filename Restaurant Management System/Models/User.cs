using System;

namespace Restaurant_Management_System.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
    }
}