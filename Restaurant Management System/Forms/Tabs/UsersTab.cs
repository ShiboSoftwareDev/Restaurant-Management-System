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
                string username = usernameBox.Text.Trim();
                string password = passwordBox.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Username and password cannot be empty.");
                    return;
                }
                string hashedPassword = SecurityHelper.ComputeSha256Hash(password);
                try
                {
                    Restaurant_Management_System.DAL.UsersDAL.AddUser(username, hashedPassword, isAdminCheck.Checked);
                    LoadUsers(usersGrid);
                    usernameBox.Text = "";
                    passwordBox.Text = "";
                    isAdminCheck.Checked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message);
                }
            };

            // Allow toggling admin status in grid with single click
            usersGrid.CellContentClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex == 2) // Admin column
                {
                    var row = usersGrid.Rows[e.RowIndex];
                    var data = row.DataBoundItem;
                    Restaurant_Management_System.Models.User user = null;
                    if (data is System.Data.DataRowView drv && drv.Row.Table.Columns.Contains("UserId"))
                    {
                        user = new Restaurant_Management_System.Models.User
                        {
                            UserId = (int)drv["UserId"],
                            Username = drv["Username"].ToString(),
                            IsAdmin = (bool)drv["IsAdmin"]
                        };
                    }
                    else if (data is Restaurant_Management_System.Models.User u)
                    {
                        user = u;
                    }
                    if (user != null)
                    {
                        bool newIsAdmin = !user.IsAdmin;
                        try
                        {
                            Restaurant_Management_System.DAL.UsersDAL.UpdateUserAdmin(user.UserId, newIsAdmin);
                            LoadUsers(usersGrid);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating admin status: " + ex.Message);
                        }
                    }
                }
            };

            var deleteUserBtn = new Button { Text = "Delete Selected", Height = 44, Width = 160, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            deleteUserBtn.FlatAppearance.BorderSize = 0;
            deleteUserBtn.Click += (s, e) => {
                if (usersGrid.SelectedRows.Count > 0)
                {
                    var row = usersGrid.SelectedRows[0];
                    var data = row.DataBoundItem;
                    int userId = -1;
                    if (data is System.Data.DataRowView drv && drv.Row.Table.Columns.Contains("UserId"))
                        userId = (int)drv["UserId"];
                    else if (data is Restaurant_Management_System.Models.User u)
                        userId = u.UserId;
                    if (userId > 0)
                    {
                        var confirm = MessageBox.Show($"Delete user with ID {userId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm == DialogResult.Yes)
                        {
                            try
                            {
                                Restaurant_Management_System.DAL.UsersDAL.DeleteUser(userId);
                                LoadUsers(usersGrid);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error deleting user: " + ex.Message);
                            }
                        }
                    }
                }
            };

            addUserPanel.Controls.Add(new Label { Text = "Username:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
            addUserPanel.Controls.Add(usernameBox);
            addUserPanel.Controls.Add(new Label { Text = "Password:", AutoSize = true, Font = new Font("Segoe UI", 12), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
            addUserPanel.Controls.Add(passwordBox);
            addUserPanel.Controls.Add(isAdminCheck);
            addUserPanel.Controls.Add(addUserBtn);
            addUserPanel.Controls.Add(deleteUserBtn);

            usersPanel.Controls.Add(addUserPanel);
            usersPanel.Controls.Add(usersGrid);
            usersPanel.Controls.Add(label);
        }

        // ...existing code...
                
    }
}