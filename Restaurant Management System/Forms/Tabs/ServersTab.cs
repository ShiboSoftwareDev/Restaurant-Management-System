using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupServersPanel()
        {
            serversPanel.Controls.Clear();
            var label = new Label { Text = "Server Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };

            var serversGrid = new DataGridView
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

            serversGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server ID", DataPropertyName = "ServerId", Width = 80 });
            serversGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 200 });

            LoadServers(serversGrid);

            var addServerPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
            var nameBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), PlaceholderText = "Server Name" };

            var addServerBtn = new Button { Text = "Add Server", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            addServerBtn.FlatAppearance.BorderSize = 0;
            addServerBtn.Click += (s, e) => {
                string name = nameBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Server name cannot be empty.");
                    return;
                }
                Restaurant_Management_System.DAL.ServersDAL.AddServer(connectionString, name);
                LoadServers(serversGrid);
            };

            var deleteServerBtn = new Button { Text = "Delete Selected", Height = 44, Width = 160, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            deleteServerBtn.FlatAppearance.BorderSize = 0;
            deleteServerBtn.Click += (s, e) => {
                if (serversGrid.SelectedRows.Count > 0)
                {
                    var row = serversGrid.SelectedRows[0];
                    if (row.DataBoundItem is DataRowView drv)
                    {
                        int serverId = Convert.ToInt32(drv["ServerId"]);
                        var confirm = MessageBox.Show($"Delete server with ID {serverId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm == DialogResult.Yes)
                        {
                            Restaurant_Management_System.DAL.ServersDAL.DeleteServer(connectionString, serverId);
                            LoadServers(serversGrid);
                        }
                    }
                }
            };

            addServerPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
            addServerPanel.Controls.Add(nameBox);
            addServerPanel.Controls.Add(addServerBtn);
            addServerPanel.Controls.Add(deleteServerBtn);

            serversPanel.Controls.Add(addServerPanel);
            serversPanel.Controls.Add(serversGrid);
            serversPanel.Controls.Add(label);
        }
        // Removed duplicate LoadServers method
                
    }
}