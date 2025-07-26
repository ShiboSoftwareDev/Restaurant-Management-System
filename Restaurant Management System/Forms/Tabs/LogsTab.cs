using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Restaurant_Management_System.DAL;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        /* ---------------------------------------------------------
         *  ACTIVITY LOGS TAB
         * ------------------------------------------------------- */
        private DataGridView logsGrid;

        private void SetupLogsPanel()
        {
            logsPanel.Controls.Clear();

            var title = new Label
            {
                Text      = "Activity Logs",
                Dock      = DockStyle.Top,
                Height    = 48,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0,120,215),
                Padding   = new Padding(16,0,0,0)
            };

            logsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                RowTemplate = { Height = 32 }
            };

            var refreshBtn = new Button
            {
                Text      = "Refresh",
                Width     = 100,
                Height    = 36,
                BackColor = Color.FromArgb(0,120,215),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Dock      = DockStyle.Bottom
            };
            refreshBtn.FlatAppearance.BorderSize = 0;
            refreshBtn.Click += (_, __) => LoadLogs();

            logsPanel.Controls.Add(refreshBtn);
            logsPanel.Controls.Add(logsGrid);
            logsPanel.Controls.Add(title);

            LoadLogs();
        }

        private void LoadLogs()
        {
            logsGrid.DataSource = ActivityLogsDAL.GetLogs();
        }
    }
}
