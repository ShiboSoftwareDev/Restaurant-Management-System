using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
                private void SetupUsersPanel()
                {
                    usersPanel.Controls.Clear();
                    var label = new Label { Text = "User Management", Dock = DockStyle.Top, Font = new Font("Segoe UI", 18, FontStyle.Bold), Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(0, 120, 215), AutoSize = false, Padding = new Padding(16, 0, 0, 0) };
                    
                    var usersGrid = new DataGridView
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
                    
                    usersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "User ID", DataPropertyName = "UserId", Width = 80 });
                    usersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Username", DataPropertyName = "Username", Width = 180 });
                    usersGrid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Admin", DataPropertyName = "IsAdmin", Width = 80 });
                    
                    LoadUsers(usersGrid);
        
                    var addUserPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0, 10, 0, 0), FlowDirection = FlowDirection.LeftToRight };
                    var usernameBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Username" };
                    var passwordBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Password", PasswordChar = '*' };
                    var isAdminCheck = new CheckBox { Text = "Is Admin", Font = new Font("Segoe UI", 12), AutoSize = true, Padding = new Padding(10, 8, 0, 0) };
                    
                    var addUserBtn = new Button { Text = "Add User", Height = 44, Width = 140, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
                    addUserBtn.FlatAppearance.BorderSize = 0;
                    addUserBtn.Click += (s, e) => {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            try
                            {
                                conn.Open();
                                var cmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, IsAdmin) VALUES (@user, @pass, @admin)", conn);
                                cmd.Parameters.AddWithValue("@user", usernameBox.Text);
        
                                // Hash the password before storing
                                string hashedPassword = SecurityHelper.ComputeSha256Hash(passwordBox.Text);
                                cmd.Parameters.AddWithValue("@pass", hashedPassword);
        
                                cmd.Parameters.AddWithValue("@admin", isAdminCheck.Checked);
                                cmd.ExecuteNonQuery();
                                LoadUsers(usersGrid);
        
                                // Clear fields after successful creation
                                usernameBox.Text = "";
                                passwordBox.Text = "";
                                isAdminCheck.Checked = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error adding user: " + ex.Message);
                            }
                        }
                    };
        
        
                    addUserPanel.Controls.Add(new Label { Text = "Username:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
                    addUserPanel.Controls.Add(usernameBox);
                    addUserPanel.Controls.Add(new Label { Text = "Password:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
                    addUserPanel.Controls.Add(passwordBox);
                    addUserPanel.Controls.Add(isAdminCheck);
                    addUserPanel.Controls.Add(addUserBtn);
                    
                    usersPanel.Controls.Add(addUserPanel);
                    usersPanel.Controls.Add(usersGrid);
                    usersPanel.Controls.Add(label);
                }
                
    }
}