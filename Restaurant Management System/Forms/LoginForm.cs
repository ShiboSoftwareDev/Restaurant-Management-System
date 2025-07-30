
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Restaurant_Management_System
{
    public partial class LoginForm : Form
    {
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
                string hashedPassword = SecurityHelper.ComputeSha256Hash(password);

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SELECT UserId, IsLocked, LastLoginAttempt, LoginAttempts FROM Users WHERE Username = @user", connection);
                    cmd.Parameters.AddWithValue("@user", username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        passwordTextBox.Text = "";
                        passwordTextBox.Focus();
                        return;
                    }
                    reader.Read();
                    int userId = reader.GetInt32(0);
                    bool isLocked = reader.GetBoolean(1);
                    object lastLoginObj = reader.IsDBNull(2) ? null : reader.GetValue(2);
                    DateTime? lastLoginAttempt = lastLoginObj == null ? (DateTime?)null : Convert.ToDateTime(lastLoginObj);
                    int loginAttempts = reader.GetInt32(3);
                    reader.Close();

                    if (lastLoginAttempt.HasValue && (DateTime.Now - lastLoginAttempt.Value).TotalDays >= 1)
                    {
                        AppLog.Write("ACCOUNT_UNLOCKED", $"User '{username}' unlocked after 24 hours.", username);
                        var resetCmd = new SqlCommand("UPDATE Users SET IsLocked = 0, LoginAttempts = 0 WHERE UserId = @id", connection);
                        resetCmd.Parameters.AddWithValue("@id", userId);
                        resetCmd.ExecuteNonQuery();
                        isLocked = false;
                        loginAttempts = 0;
                    }

                    if (isLocked)
                    {
                        double minutesLocked = 0;
                        if (loginAttempts >= 12)
                            minutesLocked = 24 * 60; 
                        else if (loginAttempts >= 9)
                            minutesLocked = 30;
                        else if (loginAttempts >= 6)
                            minutesLocked = 5;
                        else if (loginAttempts >= 3)
                            minutesLocked = 1;

                        if (lastLoginAttempt.HasValue && (DateTime.Now - lastLoginAttempt.Value).TotalMinutes < minutesLocked)
                        {
                            MessageBox.Show($"Account locked. Try again in {Math.Ceiling(minutesLocked - (DateTime.Now - lastLoginAttempt.Value).TotalMinutes)} minutes.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            AppLog.Write("ACCOUNT_UNLOCKED", $"User '{username}' unlocked after lock period.", username);
                            var unlockCmd = new SqlCommand("UPDATE Users SET IsLocked = 0 WHERE UserId = @id", connection);
                            unlockCmd.Parameters.AddWithValue("@id", userId);
                            unlockCmd.ExecuteNonQuery();
                            isLocked = false;
                        }
                    }

                    var loginCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @user AND PasswordHash = @pass", connection);
                    loginCmd.Parameters.AddWithValue("@user", username);
                    loginCmd.Parameters.AddWithValue("@pass", hashedPassword);
                    int result = (int)loginCmd.ExecuteScalar();

                    if (result > 0)
                    {
                        AppLog.Write("LOGIN_SUCCESS", $"User '{username}' logged in successfully.", username);
                        var successCmd = new SqlCommand("UPDATE Users SET IsLocked = 0, LoginAttempts = 0, LastLoginAttempt = @now WHERE UserId = @id", connection);
                        successCmd.Parameters.AddWithValue("@id", userId);
                        successCmd.Parameters.AddWithValue("@now", DateTime.Now);
                        successCmd.ExecuteNonQuery();
                        CurrentUsername = username;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        AppLog.Write("LOGIN_FAIL", $"Failed login attempt for user '{username}'. Attempts: {loginAttempts + 1}", username);
                        loginAttempts++;
                        bool shouldLock = false;
                        double lockMinutes = 0;
                        if (loginAttempts == 3) { shouldLock = true; lockMinutes = 1; }
                        else if (loginAttempts == 6) { shouldLock = true; lockMinutes = 5; }
                        else if (loginAttempts == 9) { shouldLock = true; lockMinutes = 30; }
                        else if (loginAttempts >= 12) { shouldLock = true; lockMinutes = 24 * 60; }
                        var failCmd = new SqlCommand("UPDATE Users SET LoginAttempts = @attempts, LastLoginAttempt = @now, IsLocked = @locked WHERE UserId = @id", connection);
                        failCmd.Parameters.AddWithValue("@id", userId);
                        failCmd.Parameters.AddWithValue("@attempts", loginAttempts);
                        failCmd.Parameters.AddWithValue("@now", DateTime.Now);
                        failCmd.Parameters.AddWithValue("@locked", shouldLock ? 1 : 0);
                        failCmd.ExecuteNonQuery();
                        if (shouldLock)
                        {
                            MessageBox.Show($"Account locked due to multiple failed attempts. Try again in {lockMinutes} minutes.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        passwordTextBox.Text = "";
                        passwordTextBox.Focus();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 53 || ex.Number == 4060 || ex.Number == 18456)
                {
                    MessageBox.Show("Database connection error. Please check your connection settings.", "Connection Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show($"Database error: {ex.Message}\nPlease check your connection.", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                AppLog.Write("LOGIN_ERROR", $"Unexpected error during login: {ex.Message}", username);
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
