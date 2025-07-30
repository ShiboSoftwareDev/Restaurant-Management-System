using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupClientsPanel()
        {
            clientsPanel.Controls.Clear();

            var header = new Label
            {
                Text      = "Client Management",
                Dock      = DockStyle.Top,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                Height    = 48,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize  = false,
                Padding   = new Padding(16, 0, 0, 0)
            };

            var grid = new DataGridView
            {
                Height                    = 400,
                Dock                      = DockStyle.Top,
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
                HeaderText       = "Client ID",
                DataPropertyName = "ClientId",
                Width            = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Name",
                DataPropertyName = "Name",
                Width            = 200
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Loyalty Points",
                DataPropertyName = "LoyaltyPoints",
                Width            = 140
            });

            LoadClients(grid);

            var footer = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 60,
                Padding       = new Padding(0, 10, 0, 0),
                FlowDirection = FlowDirection.LeftToRight
            };

            var nameBox = new TextBox
            {
                Width           = 300,
                Font            = new Font("Segoe UI", 12),
                PlaceholderText = "Client Name"
            };

            var addBtn = new Button
            {
                Text      = "Add Client",
                Height    = 44,
                Width     = 140,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.Click += (_, __) =>
            {
                string n = nameBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(n))
                {
                    MessageBox.Show("Client name cannot be empty."); return;
                }
                DAL.ClientsDAL.AddClient(connectionString, n);
                LoadClients(grid);
                nameBox.Clear();
            };

            var delBtn = new Button
            {
                Text      = "Delete Selected",
                Height    = 44,
                Width     = 160,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            delBtn.FlatAppearance.BorderSize = 0;
            delBtn.Click += (_, __) =>
            {
                if (grid.SelectedRows.Count == 0) return;
                var drv = grid.SelectedRows[0].DataBoundItem as DataRowView;
                if (drv == null) return;

                int id = Convert.ToInt32(drv["ClientId"]);
                if (MessageBox.Show($"Delete client with ID {id}?", "Confirm Delete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DAL.ClientsDAL.DeleteClient(connectionString, id);
                    LoadClients(grid);
                }
            };

            footer.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), Padding = new Padding(0, 8, 0, 0) });
            footer.Controls.Add(nameBox);
            footer.Controls.Add(addBtn);
            footer.Controls.Add(delBtn);

            clientsPanel.Controls.Add(footer);
            clientsPanel.Controls.Add(grid);
            clientsPanel.Controls.Add(header);
        }

        private void LoadClients(DataGridView g) =>
            g.DataSource = DAL.ClientsDAL.GetClients(connectionString);
    }
}
