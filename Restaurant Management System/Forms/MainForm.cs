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
            
            ordersToolStripMenuItem.Click += (s, e) => ShowSection("Orders");
            menuToolStripMenuItem.Click += (s, e) => ShowSection("Items");
            usersToolStripMenuItem.Click += (s, e) => ShowSection("Users");
            clientsToolStripMenuItem.Click += (s, e) => ShowSection("Clients");
            serversToolStripMenuItem.Click += (s, e) => ShowSection("Servers");
            aboutToolStripMenuItem.Click += (s, e) => ShowSection("About");
            inquiryToolStripMenuItem.Click += (s, e) => ShowSection("Inquiry");
            logsToolStripMenuItem.Click += (s, e) => ShowSection("Logs");
            
            ShowSection("Orders");
        }

        private void ShowSection(string section)
        {
            ordersPanel.Visible = false;
            itemsPanel.Visible = false;
            aboutPanel.Visible = false;
            logsPanel.Visible = false;
            usersPanel.Visible = false;
            clientsPanel.Visible = false;
            serversPanel.Visible = false;
            inquiryPanel.Visible = false;
            
            if (section == "Orders")
            {
                SetupOrdersPanel();
                ordersPanel.Visible = true;
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
                SetupClientsPanel();
                clientsPanel.Visible = true;
            }
            else if (section == "About")
            {
                if (aboutPanel.Controls.Count == 0) SetupAboutPanel();
                aboutPanel.Visible = true;
            }
            else if (section == "Logs")
            {
                if (logsPanel.Controls.Count == 0) SetupLogsPanel();
                logsPanel.Visible = true;
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
    }
}