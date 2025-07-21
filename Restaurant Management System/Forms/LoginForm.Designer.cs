namespace Restaurant_Management_System
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button loginButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.Location = new System.Drawing.Point(20, 20);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(90, 24);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "Username:";
            this.usernameLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            // 
            // passwordLabel
            // 
            this.passwordLabel.Location = new System.Drawing.Point(20, 60);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(90, 24);
            this.passwordLabel.TabIndex = 1;
            this.passwordLabel.Text = "Password:";
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(120, 20);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(160, 25);
            this.usernameTextBox.TabIndex = 2;
            this.usernameTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(120, 60);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(160, 25);
            this.passwordTextBox.TabIndex = 3;
            this.passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(120, 100);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(160, 35);
            this.loginButton.TabIndex = 4;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginButton;
            this.ClientSize = new System.Drawing.Size(300, 150);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.loginButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}