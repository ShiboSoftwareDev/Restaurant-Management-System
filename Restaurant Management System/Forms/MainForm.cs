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
                if (tablesPanel.Controls.Count == 0) SetupTablesPanel();
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
                if (clientsPanel.Controls.Count == 0) SetupClientsPanel();
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
        
        // private void SetupTablesPanel()
        // {
        //     tablesPanel.Controls.Clear();
        //     var label = new Label { Text = "Tables & Clients", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
        //     tablesGrid = new DataGridView
        //     {
        //         Dock = DockStyle.Fill,
        //         ReadOnly = true,
        //         AutoGenerateColumns = false,
        //         AllowUserToAddRows = false,
        //         BackgroundColor = Color.White,
        //         BorderStyle = BorderStyle.None,
        //         EnableHeadersVisualStyles = false,
        //         ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
        //         DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
        //         RowTemplate = { Height = 36 },
        //         GridColor = Color.LightGray
        //     };
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Table #", DataPropertyName = "TableNumber", Width = 80 });
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client Name", DataPropertyName = "ClientName", Width = 180 });
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server", DataPropertyName = "Server", Width = 120 });
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order", DataPropertyName = "OrderSummary", Width = 220 });
        //     tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "TotalPrice", Width = 100 });
        //     RefreshTablesGrid();

        //     var addOrderPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
        //     var tableNumBox = new NumericUpDown { Minimum = 1, Maximum = 100, Width = 80, Font = new Font("Segoe UI", 12) };
        //     tableNumBox.Value = 1;
        //     var clientNameBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };
        //     var addClientBtn = new Button { Text = "Add Client", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     addClientBtn.FlatAppearance.BorderSize = 0;
        //     var editClientBtn = new Button { Text = "Edit Client", Height = 44, Width = 140, BackColor = Color.FromArgb(255, 180, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     editClientBtn.FlatAppearance.BorderSize = 0;
        //     var assignServerButton = new Button { Text = "Assign Server", Height = 44, Width = 140, BackColor = Color.FromArgb(100, 180, 90), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     assignServerButton.FlatAppearance.BorderSize = 0;
        //     var addOrderBtn = new Button { Text = "Add Order", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     addOrderBtn.FlatAppearance.BorderSize = 0;
        //     var markReadyBtn = new Button { Text = "Mark Ready", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 180, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     markReadyBtn.FlatAppearance.BorderSize = 0;
        //     var markPaidBtn = new Button { Text = "Mark Paid", Height = 44, Width = 140, BackColor = Color.FromArgb(100, 180, 90), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     markPaidBtn.FlatAppearance.BorderSize = 0;
        //     addOrderPanel.Controls.Add(new Label { Text = "Table:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
        //     addOrderPanel.Controls.Add(tableNumBox);
        //     addOrderPanel.Controls.Add(new Label { Text = "Client:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
        //     addOrderPanel.Controls.Add(clientNameBox);
        //     addOrderPanel.Controls.Add(addClientBtn);
        //     addOrderPanel.Controls.Add(editClientBtn);
        //     addOrderPanel.Controls.Add(assignServerButton);
        //     addOrderPanel.Controls.Add(addOrderBtn);
        //     addOrderPanel.Controls.Add(markReadyBtn);
        //     addOrderPanel.Controls.Add(markPaidBtn);
        //     tablesPanel.Controls.Add(addOrderPanel);
        //     tablesPanel.Controls.Add(tablesGrid);
        //     tablesPanel.Controls.Add(label);
        // }

        private void RefreshTablesGrid()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter(@"
                        SELECT t.TableId AS TableNumber, c.Name AS ClientName, s.Name AS Server, 
                               o.OrderStatus AS Status, 
                               (SELECT STRING_AGG(mi.Name, ', ') 
                                FROM OrderItems oi
                                JOIN MenuItems mi ON oi.ItemId = mi.ItemId 
                                WHERE oi.OrderId = o.OrderId) AS OrderSummary,
                               (SELECT SUM(mi.Price * oi.Quantity) 
                                FROM OrderItems oi 
                                JOIN MenuItems mi ON oi.ItemId = mi.ItemId 
                                WHERE oi.OrderId = o.OrderId) AS TotalPrice
                        FROM Tables t
                        LEFT JOIN Orders o ON t.TableId = o.TableId
                        LEFT JOIN Clients c ON o.ClientId = c.ClientId
                        LEFT JOIN Servers s ON o.ServerId = s.ServerId", conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    tablesGrid.DataSource = table;
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Error loading orders: " + ex.Message); 
                }
            }
        }

        private DataGridView itemsGrid;
        private Button addItemButton;
        private Button editItemButton;
        private Button deleteItemButton;

        // private void SetupItemsPanel()
        // {
        //     itemsPanel.Controls.Clear();
        //     var label = new Label
        //     {
        //         Text = "Menu Items",
        //         Dock = DockStyle.Top,
        //         Font = new Font("Segoe UI", 18, FontStyle.Bold),
        //         Height = 48,
        //         TextAlign = ContentAlignment.MiddleLeft,
        //         ForeColor = Color.FromArgb(0, 120, 215),
        //         AutoSize = false,
        //         Padding = new Padding(16, 0, 0, 0)
        //     };

        //     itemsGrid = new DataGridView
        //     {
        //         Dock = DockStyle.Fill,
        //         ReadOnly = true,
        //         AutoGenerateColumns = false,
        //         AllowUserToAddRows = false,
        //         BackgroundColor = Color.White,
        //         BorderStyle = BorderStyle.None,
        //         EnableHeadersVisualStyles = false,
        //         ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        //         {
        //             BackColor = Color.FromArgb(0, 120, 215),
        //             ForeColor = Color.White,
        //             Font = new Font("Segoe UI", 12, FontStyle.Bold),
        //             Alignment = DataGridViewContentAlignment.MiddleCenter
        //         },
        //         DefaultCellStyle = new DataGridViewCellStyle
        //         {
        //             Font = new Font("Segoe UI", 12),
        //             SelectionBackColor = Color.FromArgb(220, 235, 252),
        //             SelectionForeColor = Color.Black
        //         },
        //         RowTemplate = { Height = 36 },
        //         GridColor = Color.LightGray
        //     };

        //     // Updated columns to match database schema
        //     itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        //     {
        //         HeaderText = "Item ID",
        //         DataPropertyName = "ItemId",
        //         Width = 80
        //     });
        //     itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        //     {
        //         HeaderText = "Name",
        //         DataPropertyName = "Name",
        //         Width = 300
        //     });
        //     itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        //     {
        //         HeaderText = "Price",
        //         DataPropertyName = "Price",
        //         Width = 120
        //     });

        //     LoadMenuItems();

        //     var addItemPanel = new FlowLayoutPanel
        //     {
        //         Dock = DockStyle.Bottom,
        //         Height = 60,
        //         Padding = new Padding(0, 10, 0, 0),
        //         FlowDirection = FlowDirection.LeftToRight
        //     };

        //     var nameBox = new TextBox
        //     {
        //         Width = 200,
        //         Font = new Font("Segoe UI", 12),
        //         PlaceholderText = "Name"
        //     };

        //     var priceBox = new NumericUpDown
        //     {
        //         Minimum = 0,
        //         Maximum = 1000,
        //         DecimalPlaces = 2,
        //         Width = 100,
        //         Font = new Font("Segoe UI", 12)
        //     };

        //     var addItemBtn = new Button
        //     {
        //         Text = "Add Item",
        //         Height = 44,
        //         Width = 140,
        //         BackColor = Color.FromArgb(0, 120, 215),
        //         ForeColor = Color.White,
        //         FlatStyle = FlatStyle.Flat,
        //         Font = new Font("Segoe UI", 12, FontStyle.Bold),
        //         Cursor = Cursors.Hand
        //     };
        //     addItemBtn.FlatAppearance.BorderSize = 0;

        //     addItemBtn.Click += (s, e) => {
        //         using (SqlConnection conn = new SqlConnection(connectionString))
        //         {
        //             try
        //             {
        //                 conn.Open();
        //                 var cmd = new SqlCommand("INSERT INTO MenuItems (Name, Price) VALUES (@name, @price)", conn);
        //                 cmd.Parameters.AddWithValue("@name", nameBox.Text);
        //                 cmd.Parameters.AddWithValue("@price", priceBox.Value);
        //                 cmd.ExecuteNonQuery();
        //                 LoadMenuItems();

        //                 // Clear input fields after adding
        //                 nameBox.Text = "";
        //                 priceBox.Value = 0;
        //             }
        //             catch (Exception ex)
        //             {
        //                 MessageBox.Show("Error adding item: " + ex.Message);
        //             }
        //         }
        //     };

        //     var editItemBtn = new Button
        //     {
        //         Text = "Edit Item",
        //         Height = 44,
        //         Width = 140,
        //         BackColor = Color.FromArgb(255, 180, 40),
        //         ForeColor = Color.White,
        //         FlatStyle = FlatStyle.Flat,
        //         Font = new Font("Segoe UI", 12, FontStyle.Bold),
        //         Cursor = Cursors.Hand
        //     };
        //     editItemBtn.FlatAppearance.BorderSize = 0;

        //     var deleteItemBtn = new Button
        //     {
        //         Text = "Delete Item",
        //         Height = 44,
        //         Width = 140,
        //         BackColor = Color.FromArgb(220, 60, 60),
        //         ForeColor = Color.White,
        //         FlatStyle = FlatStyle.Flat,
        //         Font = new Font("Segoe UI", 12, FontStyle.Bold),
        //         Cursor = Cursors.Hand
        //     };
        //     deleteItemBtn.FlatAppearance.BorderSize = 0;

        //     addItemPanel.Controls.Add(new Label
        //     {
        //         Text = "Name:",
        //         AutoSize = true,
        //         Font = new Font("Segoe UI", 12),
        //         TextAlign = ContentAlignment.MiddleLeft,
        //         Padding = new Padding(0, 8, 0, 0)
        //     });
        //     addItemPanel.Controls.Add(nameBox);
        //     addItemPanel.Controls.Add(new Label
        //     {
        //         Text = "Price:",
        //         AutoSize = true,
        //         Font = new Font("Segoe UI", 12),
        //         TextAlign = ContentAlignment.MiddleLeft,
        //         Padding = new Padding(10, 8, 0, 0)
        //     });
        //     addItemPanel.Controls.Add(priceBox);
        //     addItemPanel.Controls.Add(addItemBtn);
        //     addItemPanel.Controls.Add(editItemBtn);
        //     addItemPanel.Controls.Add(deleteItemBtn);

        //     itemsPanel.Controls.Add(addItemPanel);
        //     itemsPanel.Controls.Add(itemsGrid);
        //     itemsPanel.Controls.Add(label);
        // }

        private void LoadMenuItems()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT ItemId, Name, Price FROM MenuItems", conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    itemsGrid.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading menu items: " + ex.Message);
                }
            }
        }

        // User management
        // private void SetupUsersPanel()
        // {
        //     usersPanel.Controls.Clear();
        //     var label = new Label { Text = "User Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
            
        //     var usersGrid = new DataGridView
        //     {
        //         Dock = DockStyle.Fill,
        //         ReadOnly = true,
        //         AutoGenerateColumns = false,
        //         AllowUserToAddRows = false,
        //         BackgroundColor = Color.White,
        //         BorderStyle = BorderStyle.None,
        //         EnableHeadersVisualStyles = false,
        //         ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
        //         DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
        //         RowTemplate = { Height = 36 },
        //         GridColor = Color.LightGray
        //     };
            
        //     usersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "User ID", DataPropertyName = "UserId", Width = 80 });
        //     usersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Username", DataPropertyName = "Username", Width = 180 });
        //     usersGrid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Admin", DataPropertyName = "IsAdmin", Width = 80 });
            
        //     LoadUsers(usersGrid);

        //     var addUserPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
        //     var usernameBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Username" };
        //     var passwordBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Password", PasswordChar = '*' };
        //     var isAdminCheck = new CheckBox { Text = "Is Admin", Font = new Font("Segoe UI", 12), AutoSize = true, Padding = new Padding(10, 8, 0, 0) };
            
        //     var addUserBtn = new Button { Text = "Add User", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     addUserBtn.FlatAppearance.BorderSize = 0;
        //     addUserBtn.Click += (s, e) => {
        //         using (SqlConnection conn = new SqlConnection(connectionString))
        //         {
        //             try
        //             {
        //                 conn.Open();
        //                 var cmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, IsAdmin) VALUES (@user, @pass, @admin)", conn);
        //                 cmd.Parameters.AddWithValue("@user", usernameBox.Text);

        //                 // Hash the password before storing
        //                 string hashedPassword = SecurityHelper.ComputeSha256Hash(passwordBox.Text);
        //                 cmd.Parameters.AddWithValue("@pass", hashedPassword);

        //                 cmd.Parameters.AddWithValue("@admin", isAdminCheck.Checked);
        //                 cmd.ExecuteNonQuery();
        //                 LoadUsers(usersGrid);

        //                 // Clear fields after successful creation
        //                 usernameBox.Text = "";
        //                 passwordBox.Text = "";
        //                 isAdminCheck.Checked = false;
        //             }
        //             catch (Exception ex)
        //             {
        //                 MessageBox.Show("Error adding user: " + ex.Message);
        //             }
        //         }
        //     };


        //     addUserPanel.Controls.Add(new Label { Text = "Username:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
        //     addUserPanel.Controls.Add(usernameBox);
        //     addUserPanel.Controls.Add(new Label { Text = "Password:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
        //     addUserPanel.Controls.Add(passwordBox);
        //     addUserPanel.Controls.Add(isAdminCheck);
        //     addUserPanel.Controls.Add(addUserBtn);
            
        //     usersPanel.Controls.Add(addUserPanel);
        //     usersPanel.Controls.Add(usersGrid);
        //     usersPanel.Controls.Add(label);
        // }
        
        public void LoadUsers(DataGridView grid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT UserId, Username, IsAdmin FROM Users", conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    grid.DataSource = table;
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Error loading users: " + ex.Message); 
                }
            }
        }

        // Client management
        // private void SetupClientsPanel()
        // {
        //     clientsPanel.Controls.Clear();
        //     var label = new Label { Text = "Client Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
            
        //     var clientsGrid = new DataGridView
        //     {
        //         Dock = DockStyle.Fill,
        //         ReadOnly = true,
        //         AutoGenerateColumns = false,
        //         AllowUserToAddRows = false,
        //         BackgroundColor = Color.White,
        //         BorderStyle = BorderStyle.None,
        //         EnableHeadersVisualStyles = false,
        //         ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
        //         DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
        //         RowTemplate = { Height = 36 },
        //         GridColor = Color.LightGray
        //     };
            
        //     clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client ID", DataPropertyName = "ClientId", Width = 80 });
        //     clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 200 });
        //     clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loyalty Points", DataPropertyName = "LoyaltyPoints", Width = 120 });
            
        //     LoadClients(clientsGrid);

        //     var addClientPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
        //     var nameBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };
            
        //     var addClientBtn = new Button { Text = "Add Client", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     addClientBtn.FlatAppearance.BorderSize = 0;
        //     addClientBtn.Click += (s, e) => {
        //         using (SqlConnection conn = new SqlConnection(connectionString))
        //         {
        //             try
        //             {
        //                 conn.Open();
        //                 var cmd = new SqlCommand("INSERT INTO Clients (Name) VALUES (@name)", conn);
        //                 cmd.Parameters.AddWithValue("@name", nameBox.Text);
        //                 cmd.ExecuteNonQuery();
        //                 LoadClients(clientsGrid);
        //             }
        //             catch (Exception ex) 
        //             { 
        //                 MessageBox.Show("Error adding client: " + ex.Message); 
        //             }
        //         }
        //     };
            
        //     addClientPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
        //     addClientPanel.Controls.Add(nameBox);
        //     addClientPanel.Controls.Add(addClientBtn);
            
        //     clientsPanel.Controls.Add(addClientPanel);
        //     clientsPanel.Controls.Add(clientsGrid);
        //     clientsPanel.Controls.Add(label);
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

        // Server management
        // private void SetupServersPanel()
        // {
        //     serversPanel.Controls.Clear();
        //     var label = new Label { Text = "Server Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
            
        //     var serversGrid = new DataGridView
        //     {
        //         Dock = DockStyle.Fill,
        //         ReadOnly = true,
        //         AutoGenerateColumns = false,
        //         AllowUserToAddRows = false,
        //         BackgroundColor = Color.White,
        //         BorderStyle = BorderStyle.None,
        //         EnableHeadersVisualStyles = false,
        //         ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
        //         DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
        //         RowTemplate = { Height = 36 },
        //         GridColor = Color.LightGray
        //     };
            
        //     serversGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server ID", DataPropertyName = "ServerId", Width = 80 });
        //     serversGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 200 });
            
        //     LoadServers(serversGrid);

        //     var addServerPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
        //     var nameBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), PlaceholderText = "Server Name" };
            
        //     var addServerBtn = new Button { Text = "Add Server", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
        //     addServerBtn.FlatAppearance.BorderSize = 0;
        //     addServerBtn.Click += (s, e) => {
        //         using (SqlConnection conn = new SqlConnection(connectionString))
        //         {
        //             try
        //             {
        //                 conn.Open();
        //                 var cmd = new SqlCommand("INSERT INTO Servers (Name) VALUES (@name)", conn);
        //                 cmd.Parameters.AddWithValue("@name", nameBox.Text);
        //                 cmd.ExecuteNonQuery();
        //                 LoadServers(serversGrid);
        //             }
        //             catch (Exception ex) 
        //             { 
        //                 MessageBox.Show("Error adding server: " + ex.Message); 
        //             }
        //         }
        //     };
            
        //     addServerPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
        //     addServerPanel.Controls.Add(nameBox);
        //     addServerPanel.Controls.Add(addServerBtn);
            
        //     serversPanel.Controls.Add(addServerPanel);
        //     serversPanel.Controls.Add(serversGrid);
        //     serversPanel.Controls.Add(label);
        // }
        
        private void LoadServers(DataGridView grid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT ServerId, Name FROM Servers", conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    grid.DataSource = table;
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Error loading servers: " + ex.Message); 
                }
            }
        }

        // Loyalty discount calculation
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