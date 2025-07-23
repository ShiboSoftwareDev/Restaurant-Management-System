// ------------------------------------------------------------
//  MainForm.cs – full refactor with per-order independence
// ------------------------------------------------------------
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        /*--------------------------------------------------------
         *  FIELDS
         *------------------------------------------------------*/
        private const string ConnString =
            "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        // UI references we reuse

        /*--------------------------------------------------------
         *  CTOR
         *------------------------------------------------------*/

        /*--------------------------------------------------------
         * 1.  DATA / GRID HELPERS
         *------------------------------------------------------*/
        private void RefreshTablesGrid()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT o.OrderId,
                       o.TableId                       AS TableNumber,
                       c.Name                          AS ClientName,
                       ISNULL(s.Name,'')               AS Server,
                       o.Progress,
                       CASE o.Progress
                            WHEN 0 THEN N'Pending'
                            WHEN 1 THEN N'Ready'
                            WHEN 2 THEN N'Paid'
                            WHEN 3 THEN N'Deleted'
                       END                             AS Status,
                       ISNULL(STRING_AGG(mi.Name, ', '), N'--') AS OrderItems,
                       ISNULL(o.TotalPrice,0)          AS TotalPrice
                FROM dbo.Orders            o
                LEFT JOIN dbo.Clients      c  ON c.ClientId  = o.ClientId
                LEFT JOIN dbo.Servers      s  ON s.ServerId  = o.ServerId
                LEFT JOIN dbo.OrderItems   oi ON oi.OrderId  = o.OrderId
                LEFT JOIN dbo.MenuItems    mi ON mi.ItemId   = oi.ItemId
                WHERE o.Progress < 3                    -- hide deleted rows
                GROUP BY o.OrderId, o.TableId, c.Name, s.Name,
                         o.Progress, o.TotalPrice, o.OrderTime
                ORDER BY o.OrderTime DESC;", conn);

            new SqlDataAdapter(cmd).Fill(dt);
            tablesGrid.DataSource = dt;
        }

        /*--------------------------------------------------------
         * 2.  UI  SETUP
         *------------------------------------------------------*/
        private void SetupTablesPanel()
        {
            tablesPanel.Controls.Clear();

            /* ---------- Header ---------- */
            var header = new Label
            {
                Text      = "Tables & Clients",
                Dock      = DockStyle.Top,
                Height    = 48,
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Padding   = new Padding(16, 0, 0, 0)
            };

            /* ---------- Grid ---------- */
            tablesGrid = new DataGridView
            {
                Dock                     = DockStyle.Fill,
                ReadOnly                 = true,
                AutoGenerateColumns      = false,
                AllowUserToAddRows       = false,
                BackgroundColor          = Color.White,
                BorderStyle              = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor  = Color.FromArgb(0,120,215),
                    ForeColor  = Color.White,
                    Font       = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment  = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font               = new Font("Segoe UI", 12),
                    SelectionBackColor = Color.FromArgb(220, 235, 252)
                },
                RowTemplate = { Height = 36 }
            };

            /* text columns */
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name="OrderId",  DataPropertyName="OrderId",   Visible=false });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Table #", DataPropertyName="TableNumber", Width=80 });
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Client",  DataPropertyName="ClientName",  Width=180});
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Server",  DataPropertyName="Server",      Width=120});
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Status",  DataPropertyName="Status",      Width=120});
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="Items",   DataPropertyName="OrderItems",  Width=220});
            tablesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name="Total", HeaderText="Total", DataPropertyName="TotalPrice", Width=100,
                DefaultCellStyle=new DataGridViewCellStyle{ Format="C"}
            });

            /* action buttons */
            tablesGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ReadyBtn", HeaderText="Ready", Width = 80,
                Text="✓ Ready", UseColumnTextForButtonValue=true
            });
            tablesGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name="PaidBtn", HeaderText="Paid", Width=80,
                Text="$ Paid", UseColumnTextForButtonValue=true
            });
            tablesGrid.Columns.Add(new DataGridViewButtonColumn
            {
                Name="DeleteBtn", HeaderText="Del", Width=60,
                Text="🗑", UseColumnTextForButtonValue=true
            });

            tablesGrid.CellContentClick += TablesGrid_CellContentClick;

            RefreshTablesGrid();

            /* ---------- Bottom bar (create new order) ---------- */
            var bar = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding       = new Padding(0,10,0,0)
            };

            var tableBox  = new NumericUpDown { Minimum=1, Maximum=100, Width=80, Font=new Font("Segoe UI",12)};
            var clientBox = new TextBox       { Width=200, Font=new Font("Segoe UI",12), PlaceholderText="Client Name"};

            var btnCreate = Btn("Create Order",  Color.FromArgb(0,120,215));
            var btnAssign = Btn("Assign Server", Color.FromArgb(100,180,90));
            btnAssign.Enabled = false; // until order created

            int currentOrderId = -1;   // only for the one we’re creating now

            /* ---- create order & client ---- */
            btnCreate.Click += (_,__) =>
            {
                string clientName = clientBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(clientName))
                {
                    MessageBox.Show("Enter client name."); return;
                }
                int tableId = (int)tableBox.Value;

                using var conn = new SqlConnection(ConnString);
                conn.Open();

                // ensure table row exists
                new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM dbo.Tables WHERE TableId = @T)
                    BEGIN
                        SET IDENTITY_INSERT dbo.Tables ON;
                        INSERT INTO dbo.Tables (TableId, Status) VALUES (@T, 'Available');
                        SET IDENTITY_INSERT dbo.Tables OFF;
                    END;", conn)
                { Parameters = { new SqlParameter("@T", tableId) } }.ExecuteNonQuery();

                // upsert / get client
                var cmdCli = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM dbo.Clients WHERE Name=@Name)
                        SELECT ClientId FROM dbo.Clients WHERE Name=@Name
                    ELSE
                        INSERT INTO dbo.Clients (Name,LoyaltyPoints)
                        OUTPUT INSERTED.ClientId VALUES (@Name,0);", conn);
                cmdCli.Parameters.AddWithValue("@Name", clientName);
                int clientId = Convert.ToInt32(cmdCli.ExecuteScalar());

                // create order
                var cmdOrd = new SqlCommand(@"
                    INSERT INTO dbo.Orders
                      (TableId, ClientId, OrderStatus, OrderTime, TotalPrice)
                    OUTPUT INSERTED.OrderId
                    VALUES (@Table,@Client,'Pending',@Now,0);", conn);
                cmdOrd.Parameters.AddWithValue("@Table",  tableId);
                cmdOrd.Parameters.AddWithValue("@Client", clientId);
                cmdOrd.Parameters.AddWithValue("@Now",    DateTime.Now);
                currentOrderId = Convert.ToInt32(cmdOrd.ExecuteScalar());

                MessageBox.Show($"Order #{currentOrderId} created.");
                btnAssign.Enabled = true;
                RefreshTablesGrid();
            };

            /* ---- assign server to that newly created order ---- */
            btnAssign.Click += (_,__) =>
            {
                using var conn = new SqlConnection(ConnString);
                conn.Open();

                var frm = new Form { Text="Select Server", Size=new Size(280,400), StartPosition=FormStartPosition.CenterParent };
                var lb  = new ListBox { Dock=DockStyle.Fill, Font=new Font("Segoe UI",12),
                                        DisplayMember="Name", ValueMember="ServerId" };

                using (var rdr = new SqlCommand("SELECT ServerId,Name FROM dbo.Servers", conn).ExecuteReader())
                {
                    while (rdr.Read()) lb.Items.Add(new { ServerId=rdr.GetInt32(0), Name=rdr.GetString(1) });
                }
                var btnSel = new Button { Text="Select", Dock=DockStyle.Bottom, Height=40,
                                          BackColor=Color.FromArgb(0,120,215), ForeColor=Color.White };
                btnSel.Click += (_,__) =>
                {
                    if (lb.SelectedItem == null) { MessageBox.Show("Select a server."); return; }
                    dynamic sel = lb.SelectedItem;
                    new SqlCommand("UPDATE dbo.Orders SET ServerId=@S WHERE OrderId=@O", conn)
                    {
                        Parameters =
                        {
                            new SqlParameter("@S", sel.ServerId),
                            new SqlParameter("@O", currentOrderId)
                        }
                    }.ExecuteNonQuery();

                    MessageBox.Show($"Server “{sel.Name}” assigned.");
                    frm.Close();
                    btnAssign.Enabled = false;
                    RefreshTablesGrid();
                };

                frm.Controls.Add(lb);
                frm.Controls.Add(btnSel);
                frm.ShowDialog();
            };

            /* assemble */
            void AddLbl(string t, Control c)
            {
                bar.Controls.Add(new Label
                {
                    Text=t, AutoSize=true, Font=new Font("Segoe UI",12),
                    Padding=new Padding(t=="Table:"?0:10,8,0,0)
                });
                bar.Controls.Add(c);
            }
            AddLbl("Table:",  tableBox);
            AddLbl("Client:", clientBox);
            bar.Controls.AddRange(new Control[]{ btnCreate, btnAssign });

            tablesPanel.Controls.Add(bar);
            tablesPanel.Controls.Add(tablesGrid);
            tablesPanel.Controls.Add(header);
        }

        /*--------------------------------------------------------
         * 3.  GRID BUTTON HANDLER
         *------------------------------------------------------*/
        private void TablesGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (tablesGrid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn) return;

            var row      = tablesGrid.Rows[e.RowIndex];
            var orderId  = (int)row.Cells["OrderId"].Value;
            var price    = (decimal)row.Cells["Total"].Value;   // Total column fix
            var colName  = tablesGrid.Columns[e.ColumnIndex].Name;

            int clientId = GetClientIdForOrder(orderId);

            using var conn = new SqlConnection(ConnString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                /* local helpers so we pass tx & conn automatically */
                void Exec(string sql, params (string, object)[] ps)
                {
                    using var c = new SqlCommand(sql, conn, tx);
                    c.Parameters.AddWithValue("@o", orderId);
                    c.Parameters.AddWithValue("@c", clientId);
                    foreach (var (n, v) in ps) c.Parameters.AddWithValue(n, v);
                    c.ExecuteNonQuery();
                }
                object ExecScalar(string sql)
                {
                    using var c = new SqlCommand(sql, conn, tx);
                    c.Parameters.AddWithValue("@c", clientId);
                    return c.ExecuteScalar();
                }

                if (colName == "ReadyBtn")
                {
                    Exec("UPDATE dbo.Orders SET Progress = 1, OrderStatus='Ready' WHERE OrderId=@o");
                }
                else if (colName == "PaidBtn")
                {
                    Exec("UPDATE dbo.Orders SET Progress = 2, OrderStatus='Paid' WHERE OrderId=@o");

                    /* ----- loyalty logic (only here) ------ */
                    int points = Convert.ToInt32(ExecScalar("SELECT LoyaltyPoints FROM dbo.Clients WHERE ClientId=@c"));
                    points++;
                    decimal discount = 0m;
                    if (points >= 5)
                    {
                        discount = price * 0.10m;
                        points   = 0;
                        Exec("UPDATE dbo.Orders SET TotalPrice = TotalPrice - @d WHERE OrderId=@o",
                             ("@d", discount));
                    }
                    Exec("UPDATE dbo.Clients SET LoyaltyPoints=@p WHERE ClientId=@c",
                         ("@p", points));

                    MessageBox.Show(discount>0
                        ? $"Paid {price-discount:C}. 10 % loyalty discount applied!"
                        : $"Paid {price:C}. Thanks!");
                }
                else if (colName == "DeleteBtn")
                {
                    if (MessageBox.Show("Delete this order?","Confirm",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        Exec("UPDATE dbo.Orders SET Progress = 3, OrderStatus='Deleted' WHERE OrderId=@o");
                        // optionally clean children:
                        // Exec("DELETE FROM dbo.OrderItems WHERE OrderId=@o");
                    }
                }

                tx.Commit();
                RefreshTablesGrid();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /*--------------------------------------------------------
         * 4.  HELPERS
         *------------------------------------------------------*/
        private int GetClientIdForOrder(int orderId)
        {
            using var conn = new SqlConnection(ConnString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT ClientId FROM dbo.Orders WHERE OrderId=@id", conn);
            cmd.Parameters.AddWithValue("@id", orderId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static Button Btn(string text, Color bg) => new Button
        {
            Text          = text,
            Width         = 140,
            Height        = 44,
            BackColor     = bg,
            ForeColor     = Color.White,
            FlatStyle     = FlatStyle.Flat,
            Font          = new Font("Segoe UI",12,FontStyle.Bold),
            FlatAppearance = { BorderSize = 0 }
        };
    }
}
