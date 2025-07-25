using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Restaurant_Management_System
{
    public partial class MainForm : Form
    {
        private void SetupInquiryPanel()
        {
            inquiryPanel.Controls.Clear();
            bool isAdmin = false;
            string username = LoginForm.CurrentUsername ?? "";
            int userId = 0;
            // Get userId from database
            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand("SELECT UserId, IsAdmin FROM Users WHERE Username = @user", connection);
                        cmd.Parameters.AddWithValue("@user", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                                isAdmin = reader.GetBoolean(1);
                            }
                        }
                    }
                }
                catch
                {
                    var errorLabel = new Label
                    {
                        Text = "Error checking admin status.",
                        Dock = DockStyle.Fill,
                        Font = new Font("Segoe UI", 16, FontStyle.Regular),
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.Red,
                        AutoSize = false,
                        Padding = new Padding(32)
                    };
                    inquiryPanel.Controls.Add(errorLabel);
                    return;
                }
            }

            if (isAdmin)
            {
                // Admin: view all messages
                var messagesGrid = new DataGridView
                {
                    Height = 400,
                    Dock = DockStyle.Top,
                    ReadOnly = true,
                    AutoGenerateColumns = true,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                };
                inquiryPanel.Controls.Add(messagesGrid);

                // Load messages from API
                Task.Run(async () =>
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var url = "http://dev2.alashiq.com/message.php?systemId=61273619842346";
                            var response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode();
                            var json = await response.Content.ReadAsStringAsync();
                            var root = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
                            var messagesArr = root?["data"]?["messages"] as Newtonsoft.Json.Linq.JArray;
                            var dt = new DataTable();
                            if (messagesArr != null && messagesArr.Count > 0)
                            {
                                // Add columns from first message
                                foreach (var col in ((Newtonsoft.Json.Linq.JObject)messagesArr[0]).Properties())
                                {
                                    dt.Columns.Add(col.Name);
                                }
                                // Add rows
                                foreach (Newtonsoft.Json.Linq.JObject msgObj in messagesArr)
                                {
                                    var dr = dt.NewRow();
                                    foreach (var col in msgObj.Properties())
                                    {
                                        dr[col.Name] = col.Value;
                                    }
                                    dt.Rows.Add(dr);
                                }
                            }
                            messagesGrid.Invoke(new Action(() =>
                            {
                                messagesGrid.DataSource = dt;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        messagesGrid.Invoke(new Action(() =>
                        {
                            messagesGrid.Visible = false;
                            var errorLabel = new Label
                            {
                                Text = "Error loading messages: " + ex.Message,
                                Dock = DockStyle.Fill,
                                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                                TextAlign = ContentAlignment.MiddleCenter,
                                ForeColor = Color.Red,
                                AutoSize = false,
                                Padding = new Padding(32)
                            };
                            inquiryPanel.Controls.Add(errorLabel);
                        }));
                    }
                });
            }
            else
            {
                // Non-admin: post message
                var flowPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    Padding = new Padding(32),
                    AutoScroll = true
                };
                var infoLabel = new Label
                {
                    Text = "Send a message to admin:",
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.Black,
                    AutoSize = true
                };
                var messageBox = new TextBox
                {
                    Width = 600,
                    Font = new Font("Segoe UI", 14),
                    Multiline = true,
                    Height = 100
                };
                var sendButton = new Button
                {
                    Text = "Send",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Height = 44,
                    Width = 140,
                    FlatStyle = FlatStyle.Flat
                };
                sendButton.FlatAppearance.BorderSize = 0;
                var statusLabel = new Label
                {
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Green,
                    AutoSize = true
                };
                flowPanel.Controls.Add(infoLabel);
                flowPanel.Controls.Add(messageBox);
                flowPanel.Controls.Add(sendButton);
                flowPanel.Controls.Add(statusLabel);
                inquiryPanel.Controls.Add(flowPanel);

                sendButton.Click += async (s, e) =>
                {
                    statusLabel.Text = "";
                    var message = messageBox.Text.Trim();
                    if (string.IsNullOrEmpty(message))
                    {
                        statusLabel.ForeColor = Color.Red;
                        statusLabel.Text = "Message cannot be empty.";
                        return;
                    }
                    // Disable controls and show sending status
                    messageBox.Enabled = false;
                    sendButton.Enabled = false;
                    statusLabel.ForeColor = Color.Blue;
                    statusLabel.Text = "Sending...";
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var url = "http://dev2.alashiq.com/send.php?systemId=61273619842346";
                            var payload = new
                            {
                                user_id = userId,
                                username = username,
                                message = message
                            };
                            var json = JsonConvert.SerializeObject(payload);
                            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(url, content);
                            response.EnsureSuccessStatusCode();
                            statusLabel.ForeColor = Color.Green;
                            statusLabel.Text = "Message sent successfully.";
                            messageBox.Text = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        statusLabel.ForeColor = Color.Red;
                        statusLabel.Text = "Error sending message: " + ex.Message;
                    }
                    finally
                    {
                        messageBox.Enabled = true;
                        sendButton.Enabled = true;
                    }
                };
            }
        }
        
    }
}