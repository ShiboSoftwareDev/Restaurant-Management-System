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
        private void SetupAboutPanel()
        {
            aboutPanel.Controls.Clear();
            var label = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Black,
                AutoSize = false,
                Padding = new Padding(32)
            };
            aboutPanel.Controls.Add(label);

            // Show loading text and fetch about info from API
            label.Text = "Loading...";
            FetchAboutInfoAsync(label);
        }

        // ...existing code...

        private async void FetchAboutInfoAsync(Label label)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("http://dev2.alashiq.com/about.php");
                    dynamic json = JsonConvert.DeserializeObject(response);
                    if (json.success == true && json.data != null)
                    {
                        string title = json.data.title;
                        string description = json.data.description;
                        string version = json.data.system_version;
                        label.ForeColor = Color.Black;
                        label.Text = $"{title}\n\nVersion: {version}\n\n{description}";
                    }
                    else
                    {
                        label.ForeColor = Color.Black;
                        label.Text = "Failed to load about info.";
                    }
                }
            }
            catch (Exception ex)
            {
                label.ForeColor = Color.Black;
                label.Text = $"Error loading about info: {ex.Message}";
            }
        }
        
    }
}