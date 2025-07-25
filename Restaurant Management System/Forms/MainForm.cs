using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private string connectionString = "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        public MainForm()
        {
            InitializeComponent();
            
            // Menu item click handlers
            ordersToolStripMenuItem.Click += (s, e) => ShowSection("Orders");
            menuToolStripMenuItem.Click += (s, e) => ShowSection("Items");
            usersToolStripMenuItem.Click += (s, e) => ShowSection("Users");
            clientsToolStripMenuItem.Click += (s, e) => ShowSection("Clients");
            serversToolStripMenuItem.Click += (s, e) => ShowSection("Servers");
            aboutToolStripMenuItem.Click += (s, e) => ShowSection("About");
            inquiryToolStripMenuItem.Click += (s, e) => ShowSection("Inquiry");
            
            ShowSection("Orders");
        }

        private void ShowSection(string section)
        {
            tablesPanel.Visible = false;
            itemsPanel.Visible = false;
            aboutPanel.Visible = false;
            usersPanel.Visible = false;
            clientsPanel.Visible = false;
            serversPanel.Visible = false;
            inquiryPanel.Visible = false;
            
            if (section == "Orders")
            {
                // if (tablesPanel.Controls.Count == 0) SetupTablesPanel();
                SetupTablesPanel();
                tablesPanel.Visible = true;
            }
            else if (section == "Items")
            {
                if (itemsPanel.Controls.Count == 0) SetupItemsPanel();
                itemsPanel.Visible = true;
            }
            else if (section == "Users")
            {
                if (usersPanel.Controls.Count == 0) SetupUsersPanel();
                usersPanel.Visible = true;
            }
            else if (section == "Clients")
            {
                // if (clientsPanel.Controls.Count == 0) SetupClientsPanel();
                SetupClientsPanel();
                clientsPanel.Visible = true;
            }
            else if (section == "About")
            {
                if (aboutPanel.Controls.Count == 0) SetupAboutPanel();
                aboutPanel.Visible = true;
            }
            else if (section == "Servers")
            {
                if (serversPanel.Controls.Count == 0) SetupServersPanel();
                serversPanel.Visible = true;
            }
            else if (section == "Inquiry")
            {
                if (inquiryPanel.Controls.Count == 0) SetupInquiryPanel();
                inquiryPanel.Visible = true;
            }
        }

        private DataGridView tablesGrid;
        private Button addTableButton;
        private Button assignServerButton;
        private DataGridView itemsGrid;
        private Button addItemButton;
        private Button editItemButton;
        private Button deleteItemButton;
        // private void LoadMenuItems()
        // {
        //     using (SqlConnection conn = new SqlConnection(connectionString))
        //     {
        //         try
        //         {
        //             conn.Open();
        //             var adapter = new SqlDataAdapter("SELECT ItemId, Name, Price FROM MenuItems", conn);
        //             var table = new DataTable();
        //             adapter.Fill(table);
        //             itemsGrid.DataSource = table;
        //         }
        //         catch (Exception ex)
        //         {
        //             MessageBox.Show("Error loading menu items: " + ex.Message);
        //         }
        //     }
        // }
        private void LoadClients(DataGridView grid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT ClientId, Name, LoyaltyPoints FROM Clients", conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    grid.DataSource = table;
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Error loading clients: " + ex.Message); 
                }
            }
        }

        private decimal CalculateTotalWithDiscount(int clientId, decimal total)
        {
            if (clientId == 0) return total;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT LoyaltyPoints FROM Clients WHERE ClientId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", clientId);
                    var points = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    // Apply 5% discount for every 100 loyalty points (max 20%)
                    decimal discountPercent = Math.Min(points / 100 * 5, 20);
                    return total * (1 - discountPercent / 100);
                }
                catch 
                { 
                    return total; 
                }
            }
        }
    }
}