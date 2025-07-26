namespace Restaurant_Management_System
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ordersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serversToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inquiryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.ordersPanel = new System.Windows.Forms.Panel();
            this.itemsPanel = new System.Windows.Forms.Panel();
            this.aboutPanel = new System.Windows.Forms.Panel();
            this.logsPanel = new System.Windows.Forms.Panel();
            this.inquiryPanel = new System.Windows.Forms.Panel();
            this.usersPanel = new System.Windows.Forms.Panel();
            this.clientsPanel = new System.Windows.Forms.Panel();
            this.serversPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ordersToolStripMenuItem,
            this.menuToolStripMenuItem,
            this.usersToolStripMenuItem,
            this.clientsToolStripMenuItem,
            this.serversToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.logsToolStripMenuItem,
            this.inquiryToolStripMenuItem
            });
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(70, 25);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // logsToolStripMenuItem
            // 
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            this.logsToolStripMenuItem.Size = new System.Drawing.Size(70, 25);
            this.logsToolStripMenuItem.Text = "Logs";
            // 
            // inquiryToolStripMenuItem
            // 
            this.inquiryToolStripMenuItem.Name = "inquiryToolStripMenuItem";
            this.inquiryToolStripMenuItem.Size = new System.Drawing.Size(70, 25);
            this.inquiryToolStripMenuItem.Text = "Inquiry";
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1225, 29);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ordersToolStripMenuItem
            // 
            this.ordersToolStripMenuItem.Name = "ordersToolStripMenuItem";
            this.ordersToolStripMenuItem.Size = new System.Drawing.Size(72, 25);
            this.ordersToolStripMenuItem.Text = "Orders";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(64, 25);
            this.menuToolStripMenuItem.Text = "Items";
            // 
            // usersToolStripMenuItem
            // 
            this.usersToolStripMenuItem.Name = "usersToolStripMenuItem";
            this.usersToolStripMenuItem.Size = new System.Drawing.Size(61, 25);
            this.usersToolStripMenuItem.Text = "Users";
            // 
            // clientsToolStripMenuItem
            // 
            this.clientsToolStripMenuItem.Name = "clientsToolStripMenuItem";
            this.clientsToolStripMenuItem.Size = new System.Drawing.Size(72, 25);
            this.clientsToolStripMenuItem.Text = "Clients";
            // 
            // serversToolStripMenuItem
            // 
            this.serversToolStripMenuItem.Name = "serversToolStripMenuItem";
            this.serversToolStripMenuItem.Size = new System.Drawing.Size(76, 25);
            this.serversToolStripMenuItem.Text = "Servers";
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mainPanel.Controls.Add(this.ordersPanel);
            this.mainPanel.Controls.Add(this.itemsPanel);
            this.mainPanel.Controls.Add(this.usersPanel);
            this.mainPanel.Controls.Add(this.clientsPanel);
            this.mainPanel.Controls.Add(this.serversPanel);
            this.mainPanel.Controls.Add(this.aboutPanel);
            this.mainPanel.Controls.Add(this.logsPanel);
            this.mainPanel.Controls.Add(this.inquiryPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 29);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1225, 609);
            this.mainPanel.TabIndex = 1;
            // 
            // ordersPanel
            // 
            this.ordersPanel.BackColor = System.Drawing.Color.White;
            this.ordersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordersPanel.Location = new System.Drawing.Point(0, 0);
            this.ordersPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ordersPanel.Name = "ordersPanel";
            this.ordersPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.ordersPanel.Size = new System.Drawing.Size(1225, 609);
            this.ordersPanel.TabIndex = 0;
            this.ordersPanel.Visible = false;
            // 
            // itemsPanel
            // 
            this.itemsPanel.BackColor = System.Drawing.Color.White;
            this.itemsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsPanel.Location = new System.Drawing.Point(0, 0);
            this.itemsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.itemsPanel.Name = "itemsPanel";
            this.itemsPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.itemsPanel.Size = new System.Drawing.Size(1225, 609);
            this.itemsPanel.TabIndex = 1;
            this.itemsPanel.Visible = false;
            // 
            // aboutPanel 
            // 
            this.aboutPanel.BackColor = System.Drawing.Color.White;
            this.aboutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutPanel.Location = new System.Drawing.Point(0, 0);
            this.aboutPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.aboutPanel.Name = "aboutPanel";
            this.aboutPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.aboutPanel.Size = new System.Drawing.Size(1225, 609);
            this.aboutPanel.TabIndex = 5;
            this.aboutPanel.Visible = false;
            // 
            // logsPanel 
            // 
            this.logsPanel.BackColor = System.Drawing.Color.White;
            this.logsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logsPanel.Location = new System.Drawing.Point(0, 0);
            this.logsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.logsPanel.Name = "logsPanel";
            this.logsPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.logsPanel.Size = new System.Drawing.Size(1225, 609);
            this.logsPanel.TabIndex = 5;
            this.logsPanel.Visible = false;
            // 
            // inquiryPanel 
            // 
            this.inquiryPanel.BackColor = System.Drawing.Color.White;
            this.inquiryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inquiryPanel.Location = new System.Drawing.Point(0, 0);
            this.inquiryPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.inquiryPanel.Name = "inquiryPanel";
            this.inquiryPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.inquiryPanel.Size = new System.Drawing.Size(1225, 609);
            this.inquiryPanel.TabIndex = 5;
            this.inquiryPanel.Visible = false;
            // 
            // usersPanel
            // 
            this.usersPanel.BackColor = System.Drawing.Color.White;
            this.usersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersPanel.Location = new System.Drawing.Point(0, 0);
            this.usersPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.usersPanel.Name = "usersPanel";
            this.usersPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.usersPanel.Size = new System.Drawing.Size(1225, 609);
            this.usersPanel.TabIndex = 2;
            this.usersPanel.Visible = false;
            // 
            // clientsPanel
            // 
            this.clientsPanel.BackColor = System.Drawing.Color.White;
            this.clientsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientsPanel.Location = new System.Drawing.Point(0, 0);
            this.clientsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.clientsPanel.Name = "clientsPanel";
            this.clientsPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.clientsPanel.Size = new System.Drawing.Size(1225, 609);
            this.clientsPanel.TabIndex = 3;
            this.clientsPanel.Visible = false;
            // 
            // serversPanel
            // 
            this.serversPanel.BackColor = System.Drawing.Color.White;
            this.serversPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serversPanel.Location = new System.Drawing.Point(0, 0);
            this.serversPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.serversPanel.Name = "serversPanel";
            this.serversPanel.Padding = new System.Windows.Forms.Padding(35, 30, 35, 30);
            this.serversPanel.Size = new System.Drawing.Size(1225, 609);
            this.serversPanel.TabIndex = 4;
            this.serversPanel.Visible = false;
            // 
            // MainForm 
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1500, 638);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "Restaurant Management System";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip menuStrip1;
        public System.Windows.Forms.ToolStripMenuItem ordersToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        public System.Windows.Forms.Panel mainPanel;
        public System.Windows.Forms.Panel ordersPanel;
        public System.Windows.Forms.Panel itemsPanel;
        public System.Windows.Forms.Panel aboutPanel;
        public System.Windows.Forms.Panel logsPanel;
        public System.Windows.Forms.Panel inquiryPanel;
        public System.Windows.Forms.ToolStripMenuItem usersToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem clientsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem serversToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem inquiryToolStripMenuItem;
        public System.Windows.Forms.Panel usersPanel;
        public System.Windows.Forms.Panel clientsPanel;
        public System.Windows.Forms.Panel serversPanel;
    }
}