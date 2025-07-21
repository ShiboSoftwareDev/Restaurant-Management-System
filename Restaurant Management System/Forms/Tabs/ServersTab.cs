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
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            try
                            {
                                conn.Open();
                                var cmd = new SqlCommand("INSERT INTO Servers (Name) VALUES (@name)", conn);
                                cmd.Parameters.AddWithValue("@name", nameBox.Text);
                                cmd.ExecuteNonQuery();
                                LoadServers(serversGrid);
                            }
                            catch (Exception ex) 
                            { 
                                MessageBox.Show("Error adding server: " + ex.Message); 
                            }
                        }
                    };
                    
                    addServerPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
                    addServerPanel.Controls.Add(nameBox);
                    addServerPanel.Controls.Add(addServerBtn);
                    
                    serversPanel.Controls.Add(addServerPanel);
                    serversPanel.Controls.Add(serversGrid);
                    serversPanel.Controls.Add(label);
                }
                
    }
}