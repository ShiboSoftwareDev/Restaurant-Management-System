namespace Restaurant_Management_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ShowSection("Orders");
        }

        private void ShowSection(string section)
        {
            tablesPanel.Visible = false;
            itemsPanel.Visible = false;
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
        }

        private class TableOrder
        {
            public int TableNumber { get; set; }
            public string ClientName { get; set; }
            public string Server { get; set; }
            public string Status { get; set; }
            public List<MenuItem> OrderItems { get; set; } = new List<MenuItem>();
            public decimal TotalPrice => OrderItems.Sum(i => i.Price);
        }
        private List<TableOrder> tableOrders = new List<TableOrder> {
            new TableOrder { TableNumber = 1, ClientName = "John Doe", Server = "Alice", Status = "Seated" },
            new TableOrder { TableNumber = 2, ClientName = "Jane Smith", Server = "Bob", Status = "Ordering" }
        };
        private List<string> availableServers = new List<string> { "Alice", "Bob", "Charlie" };
        private DataGridView tablesGrid;
        private Button addTableButton;
        private Button assignServerButton;
        private void SetupTablesPanel()
        {
            tablesPanel.Controls.Clear();
            var label = new Label { Text = "Tables & Clients", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215) };
            tablesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
                RowTemplate = { Height = 36 },
                GridColor = Color.LightGray
            };
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Table #", DataPropertyName = "TableNumber", Width = 80 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client Name", DataPropertyName = "ClientName", Width = 180 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server", DataPropertyName = "Server", Width = 120 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order", DataPropertyName = "OrderSummary", Width = 220 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "TotalPrice", Width = 100 });
            RefreshTablesGrid();

            var addOrderPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
            var tableNumBox = new NumericUpDown { Minimum = 1, Maximum = 100, Width = 80, Font = new Font("Segoe UI", 12) };
            int defaultTable = 1;
            var occupiedTables = tableOrders.Select(t => t.TableNumber).ToHashSet();
            for (int i = 1; i <= 100; i++)
            {
                if (!occupiedTables.Contains(i)) { defaultTable = i; break; }
            }
            tableNumBox.Value = defaultTable;
            var clientNameBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };
            var addClientBtn = new Button { Text = "Add Client", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            addClientBtn.FlatAppearance.BorderSize = 0;
            var editClientBtn = new Button { Text = "Edit Client", Height = 44, Width = 140, BackColor = Color.FromArgb(255, 180, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            editClientBtn.FlatAppearance.BorderSize = 0;
            var assignServerButton = new Button { Text = "Assign Server", Height = 44, Width = 140, BackColor = Color.FromArgb(100, 180, 90), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            assignServerButton.FlatAppearance.BorderSize = 0;
            var addOrderBtn = new Button { Text = "Add Order", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            addOrderBtn.FlatAppearance.BorderSize = 0;
            var markReadyBtn = new Button { Text = "Mark Ready", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 180, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            markReadyBtn.FlatAppearance.BorderSize = 0;
            var markPaidBtn = new Button { Text = "Mark Paid", Height = 44, Width = 140, BackColor = Color.FromArgb(100, 180, 90), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            markPaidBtn.FlatAppearance.BorderSize = 0;
            addOrderPanel.Controls.Add(new Label { Text = "Table:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
            addOrderPanel.Controls.Add(tableNumBox);
            addOrderPanel.Controls.Add(new Label { Text = "Client:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
            addOrderPanel.Controls.Add(clientNameBox);
            addOrderPanel.Controls.Add(addClientBtn);
            addOrderPanel.Controls.Add(editClientBtn);
            addOrderPanel.Controls.Add(assignServerButton);
            addOrderPanel.Controls.Add(addOrderBtn);
            addOrderPanel.Controls.Add(markReadyBtn);
            addOrderPanel.Controls.Add(markPaidBtn);
            tablesPanel.Controls.Add(label);
            tablesPanel.Controls.Add(addOrderPanel);
            tablesPanel.Controls.Add(tablesGrid);
            label.Dock = DockStyle.Top;
            addOrderPanel.Dock = DockStyle.Bottom;
            tablesGrid.Dock = DockStyle.Fill;
            addOrderPanel.BringToFront();
            label.BringToFront();
        }
        private void RefreshTablesGrid()
        {
            tablesGrid.DataSource = null;
            tablesGrid.DataSource = tableOrders.Select(t => new
            {
                t.TableNumber,
                t.ClientName,
                t.Server,
                t.Status,
                OrderSummary = t.OrderItems.Count > 0 ? string.Join(", ", t.OrderItems.Select(i => i.Name)) : "",
                TotalPrice = t.OrderItems.Sum(i => i.Price)
            }).ToList();
        }

        private class MenuItem
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
        }
        private List<MenuItem> menuItems = new List<MenuItem> {
            new MenuItem { Name = "Pizza", Price = 10.0m, Category = "Main" },
            new MenuItem { Name = "Salad", Price = 6.0m, Category = "Starter" }
        };
        private DataGridView itemsGrid;
        private Button addItemButton;
        private Button editItemButton;
        private Button deleteItemButton;
        private void SetupItemsPanel()
        {
            itemsPanel.Controls.Clear();
            var label = new Label { Text = "Menu Items", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215) };
            itemsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 12), SelectionBackColor = Color.FromArgb(220, 235, 252), SelectionForeColor = Color.Black },
                RowTemplate = { Height = 36 },
                GridColor = Color.LightGray
            };
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 300 });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Width = 120 });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Category", DataPropertyName = "Category", Width = 180 });
            itemsGrid.DataSource = menuItems.Select(m => new { m.Name, m.Price, m.Category }).ToList();

            var addItemPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
            var nameBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Name" };
            var priceBox = new NumericUpDown { Minimum = 0, Maximum = 1000, DecimalPlaces = 2, Width = 100, Font = new Font("Segoe UI", 12) };
            var categoryBox = new TextBox { Width = 150, Font = new Font("Segoe UI", 12), PlaceholderText = "Category" };
            var addItemBtn = new Button { Text = "Add Item", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            addItemBtn.FlatAppearance.BorderSize = 0;
            var editItemBtn = new Button { Text = "Edit Item", Height = 44, Width = 140, BackColor = Color.FromArgb(255, 180, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            editItemBtn.FlatAppearance.BorderSize = 0;
            var deleteItemBtn = new Button { Text = "Delete Item", Height = 44, Width = 140, BackColor = Color.FromArgb(220, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            deleteItemBtn.FlatAppearance.BorderSize = 0;
            addItemPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
            addItemPanel.Controls.Add(nameBox);
            addItemPanel.Controls.Add(new Label { Text = "Price:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
            addItemPanel.Controls.Add(priceBox);
            addItemPanel.Controls.Add(new Label { Text = "Category:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
            addItemPanel.Controls.Add(categoryBox);
            addItemPanel.Controls.Add(addItemBtn);
            addItemPanel.Controls.Add(editItemBtn);
            addItemPanel.Controls.Add(deleteItemBtn);
            itemsPanel.Controls.Add(label);
            itemsPanel.Controls.Add(addItemPanel);
            itemsPanel.Controls.Add(itemsGrid);
            label.Dock = DockStyle.Top;
            addItemPanel.Dock = DockStyle.Bottom;
            itemsGrid.Dock = DockStyle.Fill;
            addItemPanel.BringToFront();
            label.BringToFront();
        }
        private void RefreshItemsGrid()
        {
            itemsGrid.DataSource = null;
            itemsGrid.DataSource = menuItems.Select(m => new { m.Name, m.Price, m.Category }).ToList();
        }

    }
}
