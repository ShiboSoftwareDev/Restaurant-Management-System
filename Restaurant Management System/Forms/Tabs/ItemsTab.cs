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
        
                    // Updated columns to match database schema
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
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            try
                            {
                                conn.Open();
                                var cmd = new SqlCommand("INSERT INTO MenuItems (Name, Price) VALUES (@name, @price)", conn);
                                cmd.Parameters.AddWithValue("@name", nameBox.Text);
                                cmd.Parameters.AddWithValue("@price", priceBox.Value);
                                cmd.ExecuteNonQuery();
                                LoadMenuItems();
        
                                // Clear input fields after adding
                                nameBox.Text = "";
                                priceBox.Value = 0;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error adding item: " + ex.Message);
                            }
                        }
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
        
    }
}