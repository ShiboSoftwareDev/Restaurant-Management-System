using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    internal static class Program
    {
        private static string connectionString = "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        [STAThread]
        static void Main()
        {
            // Initialize database
            InitializeDatabase();

            ApplicationConfiguration.Initialize();
            bool loginSuccess = false;
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    loginSuccess = true;
                }
            }

            if (loginSuccess)
            {
                Application.Run(new Form1());
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                // Create database if not exists
                using (var connection = new SqlConnection("Server=SHIBO;Trusted_Connection=True;TrustServerCertificate=True;"))
                {
                    connection.Open();
                    var createDbCmd = new SqlCommand(
                        "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Restaurant') " +
                        "CREATE DATABASE Restaurant", connection);
                    createDbCmd.ExecuteNonQuery();
                }

                // Create tables in the Restaurant database
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var createTablesCmd = new SqlCommand(
                        @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                        CREATE TABLE Users (
                            UserId INT IDENTITY(1,1) PRIMARY KEY,
                            Username NVARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(256) NOT NULL,
                            IsAdmin BIT NOT NULL
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Clients' AND xtype='U')
                        CREATE TABLE Clients (
                            ClientId INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL,
                            LoyaltyPoints INT NOT NULL DEFAULT 0
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Servers' AND xtype='U')
                        CREATE TABLE Servers (
                            ServerId INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
                        CREATE TABLE MenuItems (
                            ItemId INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL,
                            Price DECIMAL(10,2) NOT NULL,
                            Category NVARCHAR(50) NOT NULL
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tables' AND xtype='U')
                        CREATE TABLE Tables (
                            TableId INT IDENTITY(1,1) PRIMARY KEY,
                            Status NVARCHAR(50) NOT NULL DEFAULT 'Available'
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
                        CREATE TABLE Orders (
                            OrderId INT IDENTITY(1,1) PRIMARY KEY,
                            TableId INT NOT NULL FOREIGN KEY REFERENCES Tables(TableId),
                            ClientId INT NOT NULL FOREIGN KEY REFERENCES Clients(ClientId),
                            ServerId INT NOT NULL FOREIGN KEY REFERENCES Servers(ServerId),
                            OrderStatus NVARCHAR(50) NOT NULL,
                            OrderTime DATETIME NOT NULL DEFAULT GETDATE()
                        );
                        
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderItems' AND xtype='U')
                        CREATE TABLE OrderItems (
                            OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
                            OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderId),
                            ItemId INT NOT NULL FOREIGN KEY REFERENCES MenuItems(ItemId),
                            Quantity INT NOT NULL
                        );
                        
                        -- Add default admin user if not exists
                        IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
                        INSERT INTO Users (Username, PasswordHash, IsAdmin) 
                        VALUES ('admin', 'admin', 1);", connection);

                    createTablesCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database initialization failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}