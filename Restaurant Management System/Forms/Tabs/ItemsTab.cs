using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        /* -----------------------------------------------------------
         *  MENU ITEMS TAB
         * --------------------------------------------------------- */
        private void SetupItemsPanel()
        {
            itemsPanel.Controls.Clear();

            /* Header ------------------------------------------------ */
            var header = new Label
            {
                Text = "Menu Items",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Height = 48,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = false,
                Padding = new Padding(16, 0, 0, 0)
            };

            /* Grid -------------------------------------------------- */
            itemsGrid = new DataGridView
            {
                Height = 400,
                Dock = DockStyle.Top,
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
                    SelectionBackColor = Color.FromArgb(220, 235, 252),
                    SelectionForeColor = Color.Black
                },
                RowTemplate = { Height = 36 },
                GridColor = Color.LightGray
            };

            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Item ID",
                DataPropertyName = "ItemId",
                Width = 80
            });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                DataPropertyName = "Name",
                Width = 300
            });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                DataPropertyName = "Price",
                Width = 120
            });

            LoadMenuItems();

            /* Footer (add/edit/delete) ----------------------------- */
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0),
                FlowDirection = FlowDirection.LeftToRight
            };

            var nameBox = new TextBox
            {
                Width = 200,
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Name"
            };
            var priceBox = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 2,
                Width = 100,
                Font = new Font("Segoe UI", 12)
            };

            var addBtn = new Button
            {
                Text = "Add Item",
                Height = 44,
                Width = 140,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.Click += (_, __) =>
            {
                string n = nameBox.Text.Trim();
                decimal p = priceBox.Value;
                if (string.IsNullOrWhiteSpace(n))
                {
                    MessageBox.Show("Item name cannot be empty."); return;
                }
                DAL.ItemsDAL.AddMenuItem(connectionString, n, p);
                LoadMenuItems();
                nameBox.Clear();
                priceBox.Value = 0;
            };

            var editBtn = new Button
            {
                Text = "Edit Item",
                Height = 44,
                Width = 140,
                BackColor = Color.FromArgb(255, 180, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            editBtn.FlatAppearance.BorderSize = 0;
            editBtn.Click += (_, __) => EditSelectedItem();

            var delBtn = new Button
            {
                Text = "Delete Item",
                Height = 44,
                Width = 140,
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            delBtn.FlatAppearance.BorderSize = 0;
            delBtn.Click += (_, __) =>
            {
                if (itemsGrid.SelectedRows.Count == 0) return;

                var drv = itemsGrid.SelectedRows[0].DataBoundItem as DataRowView;
                if (drv == null) return;

                int id = Convert.ToInt32(drv["ItemId"]);
                if (MessageBox.Show($"Delete item with ID {id}?", "Confirm Delete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DAL.ItemsDAL.DeleteMenuItem(connectionString, id); // soft delete
                    LoadMenuItems();
                }
            };

            /* assemble footer */
            footer.Controls.Add(new Label { Text = "Name:", AutoSize = true, Font = new Font("Segoe UI", 12), Padding = new Padding(0, 8, 0, 0) });
            footer.Controls.Add(nameBox);
            footer.Controls.Add(new Label { Text = "Price:", AutoSize = true, Font = new Font("Segoe UI", 12), Padding = new Padding(10, 8, 0, 0) });
            footer.Controls.Add(priceBox);
            footer.Controls.Add(addBtn);
            footer.Controls.Add(editBtn);
            footer.Controls.Add(delBtn);

            /* assemble panel */
            itemsPanel.Controls.Add(footer);
            itemsPanel.Controls.Add(itemsGrid);
            itemsPanel.Controls.Add(header);
        }

        /* -----------------------------------------------------------
         *  Helpers
         * --------------------------------------------------------- */
        private void LoadMenuItems() =>
            itemsGrid.DataSource = DAL.ItemsDAL.GetMenuItems(connectionString);

        private void EditSelectedItem()
        {
            if (itemsGrid.SelectedRows.Count == 0) return;

            var drv = itemsGrid.SelectedRows[0].DataBoundItem as DataRowView;
            if (drv == null) return;

            int id = Convert.ToInt32(drv["ItemId"]);
            string name = drv["Name"].ToString();
            decimal price = Convert.ToDecimal(drv["Price"]);

            /* simple inline edit form -------------------------------- */
            using var dlg = new Form
            {
                Width = 420,
                Height = 220,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Edit Menu Item",
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var lblName = new Label { Text = "Name:", Left = 20, Top = 30, Width = 80, Font = new Font("Segoe UI", 12) };
            var txtName = new TextBox { Left = 110, Top = 25, Width = 260, Font = new Font("Segoe UI", 12), Text = name };

            var lblPrice = new Label { Text = "Price:", Left = 20, Top = 80, Width = 80, Font = new Font("Segoe UI", 12) };
            var numPrice = new NumericUpDown { Left = 110, Top = 75, Width = 120, Font = new Font("Segoe UI", 12), Minimum = 0, Maximum = 1000, DecimalPlaces = 2, Value = price };

            var ok = new Button
            {
                Text = "OK",
                Left = 110,
                Width = 100,
                Top = 130,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            ok.FlatAppearance.BorderSize = 0;

            var cancel = new Button
            {
                Text = "Cancel",
                Left = 220,
                Width = 100,
                Top = 130,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            cancel.FlatAppearance.BorderSize = 0;

            dlg.Controls.AddRange(new Control[] { lblName, txtName, lblPrice, numPrice, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string newName = txtName.Text.Trim();
                decimal newPrice = numPrice.Value;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Item name cannot be empty."); return;
                }
                DAL.ItemsDAL.UpdateMenuItem(connectionString, id, newName, newPrice);
                LoadMenuItems();
            }
        }
    }
}
