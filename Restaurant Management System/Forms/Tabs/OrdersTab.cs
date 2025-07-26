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
        /* ---- UI fields ---- */
        private DataGridView ordersGrid;
        private NumericUpDown tableBox;
        private TextBox clientBox;

        /* ===========================================================
         *  TAB SETUP
         * ========================================================= */
        private void SetupOrdersPanel()
        {
            ordersPanel.Controls.Clear();

            var header = new Label
            {
                Text = "Orders",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Height = 48,
                ForeColor = Color.FromArgb(0, 120, 215),
                Padding = new Padding(16, 0, 0, 0)
            };

            BuildGrid();
            BuildFooter();

            ordersPanel.Controls.Add(footer);
            ordersPanel.Controls.Add(ordersGrid);
            ordersPanel.Controls.Add(header);

            RefreshGrid();
        }

        /* ---------- GRID ---------- */
        private void BuildGrid()
        {
            ordersGrid = new DataGridView
            {
                Height = 430,
                Dock = DockStyle.Top,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 12),
                    SelectionBackColor = Color.FromArgb(220, 235, 252)
                },
                RowTemplate = { Height = 36 }
            };

            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "OrderId", DataPropertyName = "OrderId", Visible = false });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Table #", DataPropertyName = "TableNumber", Width = 80 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Client", DataPropertyName = "ClientName", Width = 160 });

            ordersGrid.Columns.Add(MakeBtn("AssignBtn", "Assign Srv", 90));
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Server", DataPropertyName = "Server", Width = 120 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiscountFlag", HeaderText = "Discount", DataPropertyName = "DiscountFlag", Width = 90 });
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Price",
                DataPropertyName = "TotalPrice",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C" }
            });

            ordersGrid.Columns.Add(MakeBtn("ReadyBtn", "✓ Ready", 80));
            ordersGrid.Columns.Add(MakeBtn("PaidBtn", "$ Paid", 80));
            ordersGrid.Columns.Add(MakeBtn("DeleteBtn", "🗑", 60));

            ordersGrid.Columns.Add(MakeBtn("AddItemsBtn", "➕ Items", 90));
            ordersGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemsCol",
                HeaderText = "Items",
                DataPropertyName = "OrderItems",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            ordersGrid.CellContentClick += OrdersGrid_CellContentClick;
        }

        private static DataGridViewButtonColumn MakeBtn(string name, string text, int w) =>
            new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                Width = w,
                UseColumnTextForButtonValue = true
            };

        /* ---------- FOOTER ---------- */
        private FlowLayoutPanel footer;

        private void BuildFooter()
        {
            footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 10, 0, 0)
            };

            tableBox = new NumericUpDown { Minimum = 1, Maximum = 100, Width = 80, Font = new Font("Segoe UI", 12) };
            clientBox = new TextBox { Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Client Name" };
            var btnCreate = MakeButton("Create Order", Color.FromArgb(0, 120, 215));
            btnCreate.Click += (_, _) => CreateOrder();

            footer.Controls.Add(new Label { Text = "Table:", AutoSize = true, Font = new Font("Segoe UI", 12), Padding = new Padding(0, 8, 0, 0) });
            footer.Controls.Add(tableBox);
            footer.Controls.Add(new Label { Text = "Client:", AutoSize = true, Font = new Font("Segoe UI", 12), Padding = new Padding(10, 8, 0, 0) });
            footer.Controls.Add(clientBox);
            footer.Controls.Add(btnCreate);
        }

        private static Button MakeButton(string text, Color bg) => new Button
        {
            Text = text,
            Width = 140,
            Height = 44,
            BackColor = bg,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };

        /* ===========================================================
         *  GRID CLICK HANDLER
         * ========================================================= */
        private void OrdersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (ordersGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;

            int orderId = (int)ordersGrid.Rows[e.RowIndex].Cells["OrderId"].Value;
            string col = ordersGrid.Columns[e.ColumnIndex].Name;

            if      (col == "AssignBtn")  ShowAssignDialog(orderId);
            else if (col == "AddItemsBtn")ShowItemsDialog(orderId);
            else if (col == "ReadyBtn")   OrdersDAL.UpdateProgress(orderId, 1, "Ready");
            else if (col == "PaidBtn")    HandlePayment(orderId);
            else if (col == "DeleteBtn" && Confirm("Delete this order?"))
                    OrdersDAL.DeleteOrder(orderId);

            RefreshGrid();
        }

        /* ---------- create order ---------- */
        private void CreateOrder()
        {
            string client = clientBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(client)) { MessageBox.Show("Client name required."); return; }

            try
            {
                OrdersDAL.CreateOrder((int)tableBox.Value, client);
                clientBox.Clear();
                RefreshGrid();
            }
            catch (SqlException ex) when (ex.Message.Contains("Table already in use"))
            {
                MessageBox.Show("That table already has an active order.");
            }
        }

        /* ---------- assign server ---------- */
        private void ShowAssignDialog(int orderId)
        {
            using var dlg = new Form
            {
                Text = "Assign Server",
                Size = new Size(280, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var list = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12),
                DisplayMember = "Text",
                ValueMember = "Value"
            };

            using (var conn = new SqlConnection("Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                using var rdr = new SqlCommand("SELECT ServerId, Name FROM dbo.Servers WHERE IsDeleted=0", conn).ExecuteReader();
                while (rdr.Read()) list.Items.Add(new SimpleItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            var ok = MakeButton("Select", Color.FromArgb(0, 120, 215));
            ok.Dock = DockStyle.Bottom;
            ok.Click += (_, _) =>
            {
                if (list.SelectedItem is not SimpleItem s) { MessageBox.Show("Choose a server."); return; }
                OrdersDAL.AssignServer(orderId, s.Id);
                dlg.Close();
                RefreshGrid();
            };

            dlg.Controls.Add(list);
            dlg.Controls.Add(ok);
            dlg.ShowDialog();
        }

        /* ---------- add / edit items (Qty editable) ---------- */
        private void ShowItemsDialog(int orderId)
        {
            DataTable menu = OrdersDAL.GetMenuItems();
            if (menu.Rows.Count == 0) { MessageBox.Show("No menu items."); return; }

            /* current quantities */
            var existing = new Dictionary<int, int>();
            using (var conn = new SqlConnection("Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                conn.Open();
                using var rdr = new SqlCommand("SELECT ItemId,Quantity FROM dbo.OrderItems WHERE OrderId=@o", conn)
                { Parameters = { new SqlParameter("@o", orderId) } }.ExecuteReader();
                while (rdr.Read()) existing[rdr.GetInt32(0)] = rdr.GetInt32(1);
            }

            using var dlg = new Form
            {
                Text = "Choose Items",
                Size = new Size(560, 620),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
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
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Item", DataPropertyName="Name", Width=270 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Unit", DataPropertyName="Price", Width=80, DefaultCellStyle = new DataGridViewCellStyle { Format="C2"} });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name="Qty", HeaderText="Qty *", DataPropertyName="Qty", Width=60, DefaultCellStyle = new DataGridViewCellStyle{BackColor=Color.LightYellow} });

            /* bind to mutable class so Qty is editable */
            var items = menu.AsEnumerable().Select(r => new ItemRow
            {
                Id    = r.Field<int>("ItemId"),
                Name  = r.Field<string>("Name"),
                Price = r.Field<decimal>("Price"),
                Qty   = existing.TryGetValue(r.Field<int>("ItemId"), out int q) ? q : 0
            }).ToList();

            grid.DataSource = items;
            grid.EditingControlShowing += (_, e) =>
            {
                if (grid.CurrentCell.ColumnIndex == grid.Columns["Qty"].Index && e.Control is TextBox tb)
                {
                    tb.KeyPress -= QtyKey; tb.KeyPress += QtyKey;
                }
            };
            static void QtyKey(object sender, KeyPressEventArgs e)
            { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; }

            /* live total */
            var totalLbl = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 16, 0)
            };
            void Recalc() => totalLbl.Text = $"Total: {items.Sum(i => i.Price * i.Qty):C}";
            Recalc();

            grid.CellEndEdit += (_, ev) =>
            {
                if (grid.Rows[ev.RowIndex].DataBoundItem is ItemRow r)
                {
                    r.Qty = Math.Max(0, r.Qty);
                    grid.InvalidateRow(ev.RowIndex);
                    Recalc();
                }
            };

            var ok = MakeButton("Save", Color.FromArgb(0,120,215));
            ok.Dock = DockStyle.Bottom;
            ok.Click += (_, _) =>
            {
                var dict = items.Where(i => i.Qty > 0).ToDictionary(i => i.Id, i => i.Qty);
                if (dict.Count==0) { MessageBox.Show("Select at least one item."); return; }
                OrdersDAL.AddItems(orderId, dict);
                dlg.Close();
                RefreshGrid();
            };

            dlg.Controls.Add(grid);
            dlg.Controls.Add(totalLbl);
            dlg.Controls.Add(ok);
            dlg.ShowDialog();
        }

        private sealed class ItemRow
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
        }

        /* ---------- pay ---------- */
        private void HandlePayment(int orderId)
        {
            var (total, discount) = OrdersDAL.PayOrder(orderId);

        }

        /* ---------- misc helpers ---------- */
        private void RefreshGrid() => ordersGrid.DataSource = OrdersDAL.GetActiveOrders();
        private static bool Confirm(string msg) =>
            MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.Yes;

        private sealed record SimpleItem(int Id, string Text) { public override string ToString() => Text; }
    }
}
