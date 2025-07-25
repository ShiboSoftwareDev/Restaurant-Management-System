using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Restaurant_Management_System.DAL;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        /* -----------------------------------------------------------
         *  UI FIELDS
         * --------------------------------------------------------- */
        private DataGridView ordersGrid;
        private NumericUpDown tableBox;
        private TextBox       clientBox;

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

            ordersPanel.Controls.AddRange(new Control[] { footer, ordersGrid, header });

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
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client",  DataPropertyName = "ClientName",  Width = 160 });

            ordersGrid.Columns.Add(MakeBtn("AssignBtn",  "Assign Srv", 90));      // before Server
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server", DataPropertyName = "Server", Width = 120 });

            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100 });

            ordersGrid.Columns.Add(MakeBtn("AddItemsBtn","➕ Items",   90));      // before Items
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Items",  DataPropertyName = "OrderItems", Width = 260 });

            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiscountFlag", HeaderText = "Discount", DataPropertyName = "DiscountFlag", Width = 90 });

            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name             = "Total",
                HeaderText       = "Price",
                DataPropertyName = "TotalPrice",
                Width            = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C" }
            });

            ordersGrid.Columns.Add(MakeBtn("ReadyBtn",  "✓ Ready",  80));
            ordersGrid.Columns.Add(MakeBtn("PaidBtn",   "$ Paid",   80));
            ordersGrid.Columns.Add(MakeBtn("DeleteBtn", "🗑",       60));

            ordersGrid.CellContentClick += OrdersGrid_CellContentClick;
        }

        private static DataGridViewButtonColumn MakeBtn(string name, string text, int w) =>
            new DataGridViewButtonColumn
            {
                Name                    = name,
                HeaderText              = "",
                Width                   = w,
                Text                    = text,
                UseColumnTextForButtonValue = true
            };

        /* -----------------------------------------------------------
         *  FOOTER (create‑order only)
         * --------------------------------------------------------- */
        private FlowLayoutPanel footer;

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
            clientBox = new TextBox       { Width   = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };

            var btnCreate = MakeButton("Create Order", Color.FromArgb(0, 120, 215));
            btnCreate.Click += (_, __) => CreateOrder();

            footer.Controls.Add(Label("Table:"));  footer.Controls.Add(tableBox);
            footer.Controls.Add(Label("Client:")); footer.Controls.Add(clientBox);
            footer.Controls.Add(btnCreate);

            Label Label(string t) => new Label
            {
                Text     = t,
                AutoSize = true,
                Font     = new Font("Segoe UI", 12),
                Padding  = new Padding(t == "Table:" ? 0 : 10, 8, 0, 0)
            };
        }

        /* -----------------------------------------------------------
         *  GRID CLICK HANDLER
         * --------------------------------------------------------- */
        private void OrdersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (ordersGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;

            var row     = ordersGrid.Rows[e.RowIndex];
            int orderId = (int)row.Cells["OrderId"].Value;
            string col  = ordersGrid.Columns[e.ColumnIndex].Name;

            if      (col == "AssignBtn")  ShowAssignDialog(orderId);
            else if (col == "AddItemsBtn")ShowItemsDialog (orderId);
            else if (col == "ReadyBtn")   OrdersDAL.UpdateProgress(orderId, 1, "Ready");
            else if (col == "PaidBtn")    HandlePayment(orderId, row);
            else if (col == "DeleteBtn" && Confirm("Delete this order?"))
                    OrdersDAL.DeleteOrder(orderId);

            RefreshGrid();
        }

        /* -----------------------------------------------------------
         *  CREATE ORDER
         * --------------------------------------------------------- */
        private void CreateOrder()
        {
            string client = clientBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(client))
            {
                MessageBox.Show("Enter client name."); return;
            }
            try
            {
                int orderId = OrdersDAL.CreateOrder((int)tableBox.Value, client);
                MessageBox.Show($"Order #{orderId} created.");
                clientBox.Clear();
                RefreshGrid();
            }
            catch (SqlException ex) when (ex.Message.Contains("Table already in use"))
            {
                MessageBox.Show("That table already has an active order.");
            }
        }

        /* -----------------------------------------------------------
         *  ASSIGN SERVER
         * --------------------------------------------------------- */
        private void ShowAssignDialog(int orderId)
        {
            using var dlg = new Form
            {
                Text = "Assign Server",
                Size = new Size(280, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var list = new ListBox
            {
                Dock          = DockStyle.Fill,
                Font          = new Font("Segoe UI", 12),
                DisplayMember = "Text",
                ValueMember   = "Value"
            };

            using (var conn = new SqlConnection("Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                using var rdr = new SqlCommand("SELECT ServerId, Name FROM dbo.Servers WHERE IsDeleted = 0", conn).ExecuteReader();
                while (rdr.Read()) list.Items.Add(new SimpleItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            var ok = MakeButton("Select", Color.FromArgb(0, 120, 215));
            ok.Dock = DockStyle.Bottom;
            ok.Click += (_, __) =>
            {
                if (list.SelectedItem is not SimpleItem it) { MessageBox.Show("Select a server."); return; }
                OrdersDAL.AssignServer(orderId, it.Id);
                dlg.Close();
                RefreshGrid();
            };

            dlg.Controls.Add(list);
            dlg.Controls.Add(ok);
            dlg.ShowDialog();
        }

        /* -----------------------------------------------------------
         *  ADD / EDIT ITEMS with quantities & live total
         * --------------------------------------------------------- */
        private void ShowItemsDialog(int orderId)
        {
            DataTable menu = OrdersDAL.GetMenuItems();
            if (menu.Rows.Count == 0) { MessageBox.Show("No menu items available."); return; }

            /* gather current quantities */
            var existingQty = new Dictionary<int, int>();
            using (var conn = new SqlConnection("Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                using var rdr = new SqlCommand("SELECT ItemId, Quantity FROM dbo.OrderItems WHERE OrderId=@o",
                                               conn) { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteReader();
                while (rdr.Read()) existingQty[rdr.GetInt32(0)] = rdr.GetInt32(1);
            }

            using var dlg = new Form
            {
                Text = "Choose Items",
                Size = new Size(550, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name="Id", DataPropertyName="Id", Visible=false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Item",  DataPropertyName="Name",  Width=250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText="Unit",
                DataPropertyName="Price",
                Width=80,
                DefaultCellStyle = new DataGridViewCellStyle { Format="C2" }
            });
            var qtyCol = new DataGridViewTextBoxColumn
            {
                Name="Qty",
                HeaderText="Qty",
                DataPropertyName="Qty",
                Width=60
            };
            grid.Columns.Add(qtyCol);

            var itemRows = menu.AsEnumerable()
                               .Select(r => new ItemRow
                               {
                                   Id    = r.Field<int>("ItemId"),
                                   Name  = r.Field<string>("Name"),
                                   Price = r.Field<decimal>("Price"),
                                   Qty   = existingQty.TryGetValue(r.Field<int>("ItemId"), out int q) ? q : 0
                               }).ToList();

            grid.DataSource = itemRows;
            grid.EditingControlShowing += (_, e) =>
            {
                if (grid.CurrentCell.ColumnIndex == grid.Columns["Qty"].Index &&
                    e.Control is TextBox tb)
                {
                    tb.KeyPress -= Qty_KeyPress;
                    tb.KeyPress += Qty_KeyPress;
                }
            };

            /* live total */
            var totalLbl = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0,0,16,0)
            };
            void RecalcTotal()
            {
                decimal sum = itemRows.Sum(r => r.Price * r.Qty);
                totalLbl.Text = $"Total: {sum:C}";
            }
            RecalcTotal();
            grid.CellEndEdit += (_,__) => { int r = grid.CurrentRow.Index; itemRows[r].Qty = Math.Max(0, itemRows[r].Qty); RecalcTotal(); };

            var ok = MakeButton("Save", Color.FromArgb(0,120,215));
            ok.Dock = DockStyle.Bottom;
            ok.Click += (_, __) =>
            {
                var dict = itemRows.Where(r => r.Qty > 0).ToDictionary(r => r.Id, r => r.Qty);
                if (dict.Count == 0) { MessageBox.Show("Add at least one item (quantity > 0)."); return; }
                OrdersDAL.AddItems(orderId, dict);
                dlg.Close();
                RefreshGrid();
            };

            dlg.Controls.Add(grid);
            dlg.Controls.Add(totalLbl);
            dlg.Controls.Add(ok);
            dlg.ShowDialog();

            /* numeric input only */
            static void Qty_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            }
        }

        private sealed class ItemRow
        {
            public int     Id    { get; set; }
            public string  Name  { get; set; }
            public decimal Price { get; set; }
            public int     Qty   { get; set; }
        }

        /* -----------------------------------------------------------
         *  PAYMENT
         * --------------------------------------------------------- */
        private void HandlePayment(int orderId, DataGridViewRow row)
        {
            decimal total    = (decimal)row.Cells["Total"].Value;
            bool    willDisc = ((string)row.Cells["DiscountFlag"].Value) == "Yes";
            int clientId     = OrdersDAL.GetClientId(orderId);

            int points = OrdersDAL.GetClientPoints(clientId) + 1;
            decimal disc = 0m;

            if (willDisc)
            {
                points = 0;
                disc   = total * 0.10m;
                total -= disc;
            }

            OrdersDAL.UpdateProgress(orderId, 2, "Paid");
            OrdersDAL.UpdateClientPoints(clientId, points);
            OrdersDAL.DeleteOrder(orderId);

            MessageBox.Show(willDisc
                ? $"Paid {total:C}. 10 % discount applied!"
                : $"Paid {total:C}. Thank you!");
        }

        /* -----------------------------------------------------------
         *  UTIL
         * --------------------------------------------------------- */
        private void RefreshGrid() => ordersGrid.DataSource = OrdersDAL.GetActiveOrders();

        private static bool Confirm(string msg) =>
            MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.Yes;

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

        private sealed record SimpleItem(int Id, string Text)
        {
            public override string ToString() => Text;
        }
    }
}
