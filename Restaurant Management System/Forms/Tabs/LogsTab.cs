using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Restaurant_Management_System.DAL;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private DataGridView logsGrid;

        private void SetupLogsPanel()
        {
            logsPanel.Controls.Clear();

            bool   isAdmin  = false;
            string username = LoginForm.CurrentUsername ?? "";

            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    using var conn = new SqlConnection(
                        "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;");
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT IsAdmin FROM dbo.Users WHERE Username=@u", conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    object? flag = cmd.ExecuteScalar();
                    if (flag != null && flag != DBNull.Value)
                        isAdmin = (bool)flag;
                }
                catch
                {
                    logsPanel.Controls.Add(ErrorLabel("Error checking admin status."));
                    return;
                }
            }

            if (!isAdmin)
            {
                logsPanel.Controls.Add(new Label
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
                Text      = "Activity Logs",
                Dock      = DockStyle.Top,
                Height    = 48,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Padding   = new Padding(16, 0, 0, 0)
            };

            logsGrid = new DataGridView
            {
                Dock                   = DockStyle.Fill,
                ReadOnly               = true,
                AutoGenerateColumns    = true,
                AllowUserToAddRows     = false,
                BackgroundColor        = Color.White,
                BorderStyle            = BorderStyle.None,
                AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.AllCells,
                RowTemplate            = { Height = 32 }
            };

            var refreshBtn = new Button
            {
                Text      = "Refresh",
                Width     = 110,
                Height    = 36,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Dock      = DockStyle.Bottom
            };
            refreshBtn.FlatAppearance.BorderSize = 0;
            refreshBtn.Click += (_, __) => LoadLogs();

            logsPanel.Controls.Add(refreshBtn);
            logsPanel.Controls.Add(logsGrid);
            logsPanel.Controls.Add(header);

            LoadLogs();
        }

        private void LoadLogs() =>
            logsGrid.DataSource = ActivityLogsDAL.GetLogs();

        private static Label ErrorLabel(string txt) => new Label
        {
            Text      = txt,
            Dock      = DockStyle.Fill,
            Font      = new Font("Segoe UI", 16),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Red,
            AutoSize  = false,
            Padding   = new Padding(32)
        };
    }
}
