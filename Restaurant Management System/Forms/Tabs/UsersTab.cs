using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupUsersPanel()
        {
            usersPanel.Controls.Clear();

            bool   isAdmin  = false;
            string username = LoginForm.CurrentUsername ?? "";

            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    using var conn = new SqlConnection(connectionString);
                    conn.Open();

                    var cmd = new SqlCommand(
                        "SELECT IsAdmin " +
                        "FROM   Users " +
                        "WHERE  Username = @u AND IsDeleted = 0", conn);
                    cmd.Parameters.AddWithValue("@u", username);

                    object? flag = cmd.ExecuteScalar();
                    if (flag != null && flag != DBNull.Value)
                        isAdmin = (bool)flag;
                }
                catch
                {
                    usersPanel.Controls.Add(new Label
                    {
                        Text       = "Error checking admin status.",
                        Dock       = DockStyle.Fill,
                        Font       = new Font("Segoe UI", 16, FontStyle.Regular),
                        TextAlign  = ContentAlignment.MiddleCenter,
                        ForeColor  = Color.Red,
                        AutoSize   = false,
                        Padding    = new Padding(32)
                    });
                    return;
                }
            }

            if (!isAdmin)
            {
                usersPanel.Controls.Add(new Label
                {
                    Text      = "Admin page",
                    Dock      = DockStyle.Fill,
                    Font      = new Font("Segoe UI", 24, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(0, 120, 215),
                    AutoSize  = false
                });
                return;
            }

            var header = new Label
            {
                Text      = "User Management",
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
                HeaderText       = "User ID",
                DataPropertyName = "UserId",
                Width            = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Username",
                DataPropertyName = "Username",
                Width            = 180
            });
            grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText       = "Admin",
                DataPropertyName = "IsAdmin",
                Width            = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText       = "Locked",
                DataPropertyName = "IsLocked",
                Width            = 80
            });

            LoadUsers(grid);

            var footer = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 60,
                Padding       = new Padding(0, 10, 0, 0),
                FlowDirection = FlowDirection.LeftToRight
            };

            var usernameBox = new TextBox
            {
                Width           = 200,
                Font            = new Font("Segoe UI", 12),
                PlaceholderText = "Username"
            };
            var passwordBox = new TextBox
            {
                Width           = 200,
                Font            = new Font("Segoe UI", 12),
                PlaceholderText = "Password",
                PasswordChar    = '*'
            };
            var isAdminChk = new CheckBox
            {
                Text     = "Is Admin",
                Font     = new Font("Segoe UI", 12),
                AutoSize = true,
                Padding  = new Padding(10, 8, 0, 0)
            };
            var errorProvider = new ErrorProvider();

            var addBtn = new Button
            {
                Text      = "Add User",
                Height    = 44,
                Width     = 100,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.Click += (_, __) =>
            {
                string u = usernameBox.Text.Trim();
                string p = passwordBox.Text;

                errorProvider.Clear();
                bool hasErr = false;

                if (string.IsNullOrWhiteSpace(u))
                {
                    errorProvider.SetError(usernameBox, "Username is required.");
                    hasErr = true;
                }
                else if (u.Length < 4)
                {
                    errorProvider.SetError(usernameBox, "Must be at least 4 characters.");
                    hasErr = true;
                }

                if (string.IsNullOrWhiteSpace(p))
                {
                    errorProvider.SetError(passwordBox, "Password is required.");
                    hasErr = true;
                }
                else if (p.Length < 8 || !HasNumber(p) || !HasUpper(p))
                {
                    errorProvider.SetError(passwordBox,
                        "Password must be ≥8 chars, contain a number and a capital letter.");
                    hasErr = true;
                }

                if (hasErr) return;

                string hash = SecurityHelper.ComputeSha256Hash(p);

                try
                {
                    DAL.UsersDAL.AddUser(u, hash, isAdminChk.Checked);
                    LoadUsers(grid);
                    usernameBox.Clear();
                    passwordBox.Clear();
                    isAdminChk.Checked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message);
                }
            };

            var delBtn = new Button
            {
                Text      = "Delete",
                Height    = 44,
                Width     = 100,
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

                int id = GetSelectedUserId(grid);
                if (id <= 0) return;

                if (MessageBox.Show($"Delete user with ID {id}?",
                                    "Confirm Delete",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DAL.UsersDAL.DeleteUser(id);
                        LoadUsers(grid);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting user: " + ex.Message);
                    }
                }
            };

            var unlockBtn = new Button
            {
                Text      = "Unlock",
                Height    = 44,
                Width     = 100,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            unlockBtn.FlatAppearance.BorderSize = 0;
            unlockBtn.Click += (_, __) =>
            {
                if (grid.SelectedRows.Count == 0) return;

                int id = GetSelectedUserId(grid);
                if (id <= 0) return;

                try
                {
                    DAL.UsersDAL.UpdateUserLocked(id, false);
                    LoadUsers(grid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error unlocking user: " + ex.Message);
                }
            };

            grid.CellContentClick += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 2) return;

                var row  = grid.Rows[e.RowIndex];
                var user = row.DataBoundItem as Models.User;
                if (user == null) return;

                bool newFlag = !user.IsAdmin;

                try
                {
                    DAL.UsersDAL.UpdateUserAdmin(user.UserId, newFlag);
                    LoadUsers(grid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating admin status: " + ex.Message);
                }
            };

            footer.Controls.Add(new Label
            {
                Text     = "Username:",
                AutoSize = true,
                Font     = new Font("Segoe UI", 12),
                Padding  = new Padding(0, 8, 0, 0)
            });
            footer.Controls.Add(usernameBox);

            footer.Controls.Add(new Label
            {
                Text     = "Password:",
                AutoSize = true,
                Font     = new Font("Segoe UI", 12),
                Padding  = new Padding(10, 8, 0, 0)
            });
            footer.Controls.Add(passwordBox);

            footer.Controls.Add(isAdminChk);
            footer.Controls.Add(addBtn);
            footer.Controls.Add(delBtn);
            footer.Controls.Add(unlockBtn);

            usersPanel.Controls.Add(footer);
            usersPanel.Controls.Add(grid);
            usersPanel.Controls.Add(header);

            bool HasNumber(string s) { foreach (char c in s) if (char.IsDigit(c)) return true; return false; }
            bool HasUpper (string s) { foreach (char c in s) if (char.IsUpper(c)) return true; return false; }

            int GetSelectedUserId(DataGridView g)
            {
                var data = g.SelectedRows[0].DataBoundItem;
                if (data is DataRowView drv && drv.Row.Table.Columns.Contains("UserId"))
                    return (int)drv["UserId"];
                if (data is Models.User u) return u.UserId;
                return -1;
            }
        }

        private void LoadUsers(DataGridView g) =>
            g.DataSource = DAL.UsersDAL.GetAllUsers();
    }
}
