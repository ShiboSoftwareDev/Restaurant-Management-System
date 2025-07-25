using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Restaurant_Management_System.DAL;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        /* -----------------------------------------------------------
         *  FIELDS
         * --------------------------------------------------------- */
        private DataGridView ordersGrid;
        private FlowLayoutPanel footer;

        /* -----------------------------------------------------------
         *  TAB SETUP
         * --------------------------------------------------------- */
        private void SetupOrdersPanel()
        {
            ordersPanel.Controls.Clear();

            var header = new Label
            {
                Text      = "Orders",
                Dock      = DockStyle.Top,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                Height    = 48,
                ForeColor = Color.FromArgb(0, 120, 215),
                Padding   = new Padding(16, 0, 0, 0)
            };

            BuildGrid();
            BuildFooter();

            ordersPanel.Controls.Add(footer);
            ordersPanel.Controls.Add(ordersGrid);
            ordersPanel.Controls.Add(header);

            RefreshGrid();
        }

        /* -----------------------------------------------------------
         *  GRID
         * --------------------------------------------------------- */
        private void BuildGrid()
        {
            ordersGrid = new DataGridView
            {
                Dock                      = DockStyle.Fill,
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
                    SelectionBackColor = Color.FromArgb(220, 235, 252)
                },
                RowTemplate = { Height = 36 }
            };

            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "OrderId", DataPropertyName = "OrderId", Visible = false });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Table #", DataPropertyName = "TableNumber", Width = 80 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client", DataPropertyName = "ClientName", Width = 180 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server", DataPropertyName = "Server", Width = 120 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Items", DataPropertyName = "OrderItems", Width = 220 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name               = "Total",
                HeaderText         = "Total",
                DataPropertyName   = "TotalPrice",
                Width              = 100,
                DefaultCellStyle   = new DataGridViewCellStyle { Format = "C" }
            });

            ordersGrid.Columns.Add(MakeBtn("ReadyBtn",  "✓ Ready",  80));
            ordersGrid.Columns.Add(MakeBtn("PaidBtn",   "$ Paid",   80));
            ordersGrid.Columns.Add(MakeBtn("DeleteBtn", "🗑",        60));

            ordersGrid.CellContentClick += OrdersGrid_CellContentClick;
            ordersGrid.SelectionChanged  += (_, __) => UpdateAssignButtonState();

            static DataGridViewButtonColumn MakeBtn(string name, string text, int w) =>
                new DataGridViewButtonColumn
                {
                    Name                    = name,
                    HeaderText              = "",
                    Width                   = w,
                    Text                    = text,
                    UseColumnTextForButtonValue = true
                };
        }

        /* -----------------------------------------------------------
         *  FOOTER (new‑order / assign server)
         * --------------------------------------------------------- */
        private Button btnAssign;
        private NumericUpDown tableBox;
        private TextBox clientBox;

        private void BuildFooter()
        {
            footer = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding       = new Padding(0, 10, 0, 0)
            };

            tableBox  = new NumericUpDown { Minimum = 1, Maximum = 100, Width = 80, Font = new Font("Segoe UI", 12) };
            clientBox = new TextBox       { Width   = 200, Font   = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };

            var btnCreate = MakeButton("Create Order", Color.FromArgb(0, 120, 215));
            btnAssign     = MakeButton("Assign Server", Color.FromArgb(100, 180, 90));

            btnCreate.Click += (_, __) => CreateOrder();
            btnAssign.Click += (_, __) => AssignServerToSelected();

            UpdateAssignButtonState();

            footer.Controls.Add(Label("Table:"));  footer.Controls.Add(tableBox);
            footer.Controls.Add(Label("Client:")); footer.Controls.Add(clientBox);
            footer.Controls.Add(btnCreate);
            footer.Controls.Add(btnAssign);

            Label Label(string t) => new Label
            {
                Text     = t,
                AutoSize = true,
                Font     = new Font("Segoe UI", 12),
                Padding  = new Padding(t == "Table:" ? 0 : 10, 8, 0, 0)
            };
        }

        /* -----------------------------------------------------------
         *  BUTTON ACTIONS
         * --------------------------------------------------------- */
        private void CreateOrder()
        {
            string cName = clientBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cName))
            {
                MessageBox.Show("Enter client name."); return;
            }

            int tableId = (int)tableBox.Value;
            int orderId = OrdersDAL.CreateOrder(tableId, cName);

            MessageBox.Show($"Order #{orderId} created.");
            clientBox.Clear();
            RefreshGrid();
        }

        private void AssignServerToSelected()
        {
            if (ordersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an order first."); return;
            }

            int orderId = (int)ordersGrid.SelectedRows[0].Cells["OrderId"].Value;

            /* choose a server */
            using var dlg = new Form
            {
                Text           = "Select Server",
                Size           = new Size(280, 400),
                StartPosition  = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox    = false,
                MaximizeBox    = false
            };

            var list = new ListBox
            {
                Dock          = DockStyle.Fill,
                Font          = new Font("Segoe UI", 12),
                DisplayMember = "Text",
                ValueMember   = "Value"
            };

            using (var conn = new Microsoft.Data.SqlClient.SqlConnection("Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                using var rdr = new Microsoft.Data.SqlClient.SqlCommand("SELECT ServerId, Name FROM dbo.Servers WHERE IsDeleted = 0", conn).ExecuteReader();
                while (rdr.Read())
                {
                    list.Items.Add(new ListItem(rdr.GetInt32(0), rdr.GetString(1)));
                }
            }

            var ok = new Button
            {
                Text       = "Select",
                Dock       = DockStyle.Bottom,
                Height     = 40,
                BackColor  = Color.FromArgb(0, 120, 215),
                ForeColor  = Color.White,
                FlatStyle  = FlatStyle.Flat
            };
            ok.FlatAppearance.BorderSize = 0;
            ok.Click += (_, __) =>
            {
                if (list.SelectedItem is not ListItem li)
                {
                    MessageBox.Show("Choose a server."); return;
                }
                OrdersDAL.AssignServer(orderId, li.Value);
                MessageBox.Show($"Server “{li.Text}” assigned.");
                dlg.Close();
                RefreshGrid();
            };

            dlg.Controls.Add(list);
            dlg.Controls.Add(ok);
            dlg.ShowDialog();
        }

        /* -----------------------------------------------------------
         *  GRID CELL CLICK
         * --------------------------------------------------------- */
        private void OrdersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (ordersGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;

            var row     = ordersGrid.Rows[e.RowIndex];
            int orderId = (int)row.Cells["OrderId"].Value;
            string col  = ordersGrid.Columns[e.ColumnIndex].Name;

            if (col == "ReadyBtn")
            {
                OrdersDAL.UpdateProgress(orderId, 1, "Ready");
            }
            else if (col == "PaidBtn")
            {
                HandlePayment(orderId, row);
            }
            else if (col == "DeleteBtn")
            {
                if (MessageBox.Show("Delete this order?", "Confirm",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    OrdersDAL.DeleteOrder(orderId);
                }
            }

            RefreshGrid();
        }

        /* -----------------------------------------------------------
         *  PAYMENT LOGIC
         * --------------------------------------------------------- */
        private void HandlePayment(int orderId, DataGridViewRow row)
        {
            decimal total = (decimal)row.Cells["Total"].Value;
            int clientId  = OrdersDAL.GetClientId(orderId);

            int points = OrdersDAL.GetClientPoints(clientId) + 1;
            decimal discount = 0m;

            if (points >= 5)
            {
                points = 0;
                discount = total * 0.10m;
                total   -= discount;
            }

            OrdersDAL.UpdateProgress(orderId, 2, "Paid");
            OrdersDAL.UpdateClientPoints(clientId, points);
            OrdersDAL.DeleteOrder(orderId);   // remove completed order completely

            MessageBox.Show(discount > 0
                ? $"Paid {total:C}. 10 % discount applied!"
                : $"Paid {total:C}. Thank you!");
        }

        /* -----------------------------------------------------------
         *  UTIL
         * --------------------------------------------------------- */
        private void RefreshGrid() =>
            ordersGrid.DataSource = OrdersDAL.GetActiveOrders();

        private void UpdateAssignButtonState() =>
            btnAssign.Enabled = ordersGrid.SelectedRows.Count > 0;

        private static Button MakeButton(string text, Color bg) => new Button
        {
            Text         = text,
            Width        = 140,
            Height       = 44,
            BackColor    = bg,
            ForeColor    = Color.White,
            Font         = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle    = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };

        /* helper object for the server ListBox */
        private sealed record ListItem(int Value, string Text)
        {
            public override string ToString() => Text;
        }
    }
}
