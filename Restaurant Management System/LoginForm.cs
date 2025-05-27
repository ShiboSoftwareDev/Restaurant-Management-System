using System;
using System.Windows.Forms;

namespace Restaurant_Management_System
{
    public partial class LoginForm : Form
    {
        public string Username => usernameTextBox.Text;
        public string Password => passwordTextBox.Text;

        public LoginForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(320, 180);
            this.Text = "Login";
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (Username == "admin" && Password == "admin")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
