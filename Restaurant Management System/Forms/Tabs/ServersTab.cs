using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupServersPanel()
        {
            serversPanel.Controls.Clear();

            var header = new Label
            {
                Text               = "Server Management",
                Dock               = DockStyle.Top,
                Font               = new Font("Segoe UI", 18, FontStyle.Bold),
                Height             = 48,
                TextAlign          = ContentAlignment.MiddleLeft,
                ForeColor          = Color.FromArgb(0, 120, 215),
                AutoSize           = false,
                Padding            = new Padding(16, 0, 0, 0)
            };

            var grid = new DataGridView
            {
                Height = 400,
                Dock = DockStyle.Top,
                ReadOnly                  = true,
                AutoGenerateColumns       = false,
                AllowUserToAddRows        = false,
                BackgroundColor           = Color.White,
                BorderStyle               = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font               = new Font("Segoe UI", 12),
                    SelectionBackColor = Color.FromArgb(220, 235, 252),
                    SelectionForeColor = Color.Black
                },
                RowTemplate = { Height = 36 },
                GridColor   = Color.LightGray
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Server ID",
                DataPropertyName = "ServerId",
                Width            = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Name",
                DataPropertyName = "Name",
                Width            = 200
            });

            LoadServers(grid);

            var foot = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 60,
                Padding       = new Padding(0, 10, 0, 0),
                FlowDirection = FlowDirection.LeftToRight
            };

            var nameBox = new TextBox
            {
                Width          = 300,
                Font           = new Font("Segoe UI", 12),
                PlaceholderText = "Server Name"
            };

            var addBtn = new Button
            {
                Text      = "Add Server",
                Width     = 140,
                Height    = 44,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.Click += (_, __) =>
            {
                string name = nameBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Server name cannot be empty.");
                    return;
                }

                DAL.ServersDAL.AddServer(connectionString, name);
                LoadServers(grid);
                nameBox.Clear();
            };

            var delBtn = new Button
            {
                Text      = "Delete Selected",
                Width     = 160,
                Height    = 44,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            delBtn.FlatAppearance.BorderSize = 0;
            delBtn.Click += (_, __) =>
            {
                if (grid.SelectedRows.Count == 0) return;

                var row = grid.SelectedRows[0];
                if (row.DataBoundItem is DataRowView drv)
                {
                    int id = Convert.ToInt32(drv["ServerId"]);
                    if (MessageBox.Show($"Delete server with ID {id}?",
                                        "Confirm Delete",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        DAL.ServersDAL.DeleteServer(connectionString, id);
                        LoadServers(grid);
                    }
                }
            };

            foot.Controls.Add(new Label
            {
                Text      = "Name:",
                AutoSize  = true,
                Font      = new Font("Segoe UI", 12),
                Padding   = new Padding(0, 8, 0, 0)
            });
            foot.Controls.Add(nameBox);
            foot.Controls.Add(addBtn);
            foot.Controls.Add(delBtn);

            serversPanel.Controls.Add(foot);
            serversPanel.Controls.Add(grid);
            serversPanel.Controls.Add(header);
        }

        private void LoadServers(DataGridView grid) =>
            grid.DataSource = DAL.ServersDAL.GetServers(connectionString);
    }
}
