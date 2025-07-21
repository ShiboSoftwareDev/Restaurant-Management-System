using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
                private void SetupTablesPanel()
                {
                    tablesPanel.Controls.Clear();
                    var label = new Label { Text = "Tables & Clients", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
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
                    tableNumBox.Value = 1;
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
                    tablesPanel.Controls.Add(addOrderPanel);
                    tablesPanel.Controls.Add(tablesGrid);
                    tablesPanel.Controls.Add(label);
                }
        
    }
}