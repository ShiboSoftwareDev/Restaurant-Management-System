
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = loginButton;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            // Input validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Hash the password before checking
                string hashedPassword = SecurityHelper.ComputeSha256Hash(password);
                bool isValidUser = ValidateCredentials(username, hashedPassword);

                if (isValidUser)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passwordTextBox.Text = "";
                    passwordTextBox.Focus();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}\nPlease check your connection.", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool ValidateCredentials(string username, string hashedPassword)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username = @user AND PasswordHash = @pass",
                    connection);

                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", hashedPassword);

                int result = (int)cmd.ExecuteScalar();
                return result > 0;
            }
        }
    }
}