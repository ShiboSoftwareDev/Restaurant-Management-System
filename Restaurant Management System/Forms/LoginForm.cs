
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class LoginForm : Form
    {
        // Store the current logged-in username for use in other forms
        // Store the current logged-in username for use in other forms
        public static string CurrentUsername { get; private set; }
    
        private string connectionString = "Server=SHIBO;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True;";
        private ErrorProvider errorProvider = new ErrorProvider();

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = loginButton;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            errorProvider.Clear();
            bool hasError = false;
            if (string.IsNullOrEmpty(username))
            {
                errorProvider.SetError(usernameTextBox, "Username is required.");
                hasError = true;
            }
            else if (username.Length < 4)
            {
                errorProvider.SetError(usernameTextBox, "Username must be at least 4 characters.");
                hasError = true;
            }
            if (string.IsNullOrEmpty(password))
            {
                errorProvider.SetError(passwordTextBox, "Password is required.");
                hasError = true;
            }
            else if (password.Length < 8 || !HasNumber(password) || !HasUpper(password))
            {
                errorProvider.SetError(passwordTextBox, "Password must be at least 8 characters, contain a number and a capital letter.");
                hasError = true;
            }
            if (hasError)
            {
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
                    CurrentUsername = username;
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
        
        private bool HasNumber(string s)
        {
            foreach (char c in s)
                if (char.IsDigit(c)) return true;
            return false;
        }

        private bool HasUpper(string s)
        {
            foreach (char c in s)
                if (char.IsUpper(c)) return true;
            return false;
        }
    }
}
