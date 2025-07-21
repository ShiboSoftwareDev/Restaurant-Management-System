using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupItemsPanel()
        {
            itemsPanel.Controls.Clear();
            var label = new Label
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

            itemsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
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

            var addItemPanel = new FlowLayoutPanel
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

            var addItemBtn = new Button
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
            addItemBtn.FlatAppearance.BorderSize = 0;

            addItemBtn.Click += (s, e) => {
                string name = nameBox.Text.Trim();
                decimal price = priceBox.Value;
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Item name cannot be empty.");
                    return;
                }
                Restaurant_Management_System.DAL.ItemsDAL.AddMenuItem(connectionString, name, price);
                LoadMenuItems();
                nameBox.Text = "";
                priceBox.Value = 0;
            };

            var editItemBtn = new Button
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
            editItemBtn.FlatAppearance.BorderSize = 0;
            editItemBtn.Click += (s, e) => {
                if (itemsGrid.SelectedRows.Count > 0)
                {
                    var row = itemsGrid.SelectedRows[0];
                    if (row.DataBoundItem is DataRowView drv)
                    {
                        int itemId = Convert.ToInt32(drv["ItemId"]);
                        string currentName = drv["Name"].ToString();
                        decimal currentPrice = Convert.ToDecimal(drv["Price"]);

                        // Create a custom dialog for editing
                        var dialog = new Form()
                        {
                            Width = 400,
                            Height = 220,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            Text = "Edit Menu Item",
                            StartPosition = FormStartPosition.CenterParent,
                            MinimizeBox = false,
                            MaximizeBox = false
                        };

                        var nameLabel = new Label { Text = "Name:", Left = 20, Top = 30, Width = 80, Font = new Font("Segoe UI", 12) };
                        var nameEdit = new TextBox { Left = 110, Top = 25, Width = 240, Font = new Font("Segoe UI", 12), Text = currentName };
                        var priceLabel = new Label { Text = "Price:", Left = 20, Top = 80, Width = 80, Font = new Font("Segoe UI", 12) };
                        var priceEdit = new NumericUpDown { Left = 110, Top = 75, Width = 120, Font = new Font("Segoe UI", 12), Minimum = 0, Maximum = 1000, DecimalPlaces = 2, Value = currentPrice };

                        var okBtn = new Button { Text = "OK", Left = 110, Width = 100, Top = 130, DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 11, FontStyle.Bold), BackColor = Color.FromArgb(0,120,215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                        okBtn.FlatAppearance.BorderSize = 0;
                        var cancelBtn = new Button { Text = "Cancel", Left = 220, Width = 100, Top = 130, DialogResult = DialogResult.Cancel, Font = new Font("Segoe UI", 11, FontStyle.Bold), BackColor = Color.LightGray, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat };
                        cancelBtn.FlatAppearance.BorderSize = 0;

                        dialog.Controls.Add(nameLabel);
                        dialog.Controls.Add(nameEdit);
                        dialog.Controls.Add(priceLabel);
                        dialog.Controls.Add(priceEdit);
                        dialog.Controls.Add(okBtn);
                        dialog.Controls.Add(cancelBtn);
                        dialog.AcceptButton = okBtn;
                        dialog.CancelButton = cancelBtn;

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string newName = nameEdit.Text.Trim();
                            decimal newPrice = priceEdit.Value;
                            if (string.IsNullOrWhiteSpace(newName))
                            {
                                MessageBox.Show("Item name cannot be empty.");
                                return;
                            }
                            Restaurant_Management_System.DAL.ItemsDAL.UpdateMenuItem(connectionString, itemId, newName, newPrice);
                            LoadMenuItems();
                        }
                    }
                }
            };

            var deleteItemBtn = new Button
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
            deleteItemBtn.FlatAppearance.BorderSize = 0;
            deleteItemBtn.Click += (s, e) => {
                if (itemsGrid.SelectedRows.Count > 0)
                {
                    var row = itemsGrid.SelectedRows[0];
                    if (row.DataBoundItem is DataRowView drv)
                    {
                        int itemId = Convert.ToInt32(drv["ItemId"]);
                        var confirm = MessageBox.Show($"Delete item with ID {itemId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm == DialogResult.Yes)
                        {
                            Restaurant_Management_System.DAL.ItemsDAL.DeleteMenuItem(connectionString, itemId);
                            LoadMenuItems();
                        }
                    }
                }
            };

            addItemPanel.Controls.Add(new Label
            {
                Text = "Name:",
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, 0, 0)
            });
            addItemPanel.Controls.Add(nameBox);
            addItemPanel.Controls.Add(new Label
            {
                Text = "Price:",
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 0, 0)
            });
            addItemPanel.Controls.Add(priceBox);
            addItemPanel.Controls.Add(addItemBtn);
            addItemPanel.Controls.Add(editItemBtn);
            addItemPanel.Controls.Add(deleteItemBtn);

            itemsPanel.Controls.Add(addItemPanel);
            itemsPanel.Controls.Add(itemsGrid);
            itemsPanel.Controls.Add(label);
        }

        // ...existing code...
        
    }
}