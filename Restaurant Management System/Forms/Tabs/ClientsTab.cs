using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
                private void SetupClientsPanel()
                {
                    clientsPanel.Controls.Clear();
                    var label = new Label { Text = "Client Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
                    
                    var clientsGrid = new DataGridView
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
                    
                    clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client ID", DataPropertyName = "ClientId", Width = 80 });
                    clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 200 });
                    clientsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loyalty Points", DataPropertyName = "LoyaltyPoints", Width = 120 });
                    
                    LoadClients(clientsGrid);
        
                    var addClientPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
                    var nameBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };
                    
                    var addClientBtn = new Button { Text = "Add Client", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
                    addClientBtn.FlatAppearance.BorderSize = 0;
                    addClientBtn.Click += (s, e) => {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            try
                            {
                                conn.Open();
                                var cmd = new SqlCommand("INSERT INTO Clients (Name) VALUES (@name)", conn);
                                cmd.Parameters.AddWithValue("@name", nameBox.Text);
                                cmd.ExecuteNonQuery();
                                LoadClients(clientsGrid);
                            }
                            catch (Exception ex) 
                            { 
                                MessageBox.Show("Error adding client: " + ex.Message); 
                            }
                        }
                    };
                    
                    addClientPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
                    addClientPanel.Controls.Add(nameBox);
                    addClientPanel.Controls.Add(addClientBtn);
                    
                    clientsPanel.Controls.Add(addClientPanel);
                    clientsPanel.Controls.Add(clientsGrid);
                    clientsPanel.Controls.Add(label);
                }
                
    }
}