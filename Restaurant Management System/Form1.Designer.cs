namespace Restaurant_Management_System
{
    partial class Form1
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

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ordersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.tablesPanel = new System.Windows.Forms.Panel();
            this.itemsPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.Items.Clear();
            this.ordersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ordersToolStripMenuItem.Name = "ordersToolStripMenuItem";
            this.ordersToolStripMenuItem.Size = new System.Drawing.Size(80, 28);
            this.ordersToolStripMenuItem.Text = "Orders";
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(80, 28);
            this.menuToolStripMenuItem.Text = "Items";
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.ordersToolStripMenuItem,
                this.menuToolStripMenuItem
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = true;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 28);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1400, 800);
            this.mainPanel.TabIndex = 1;
            this.mainPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mainPanel.Padding = new System.Windows.Forms.Padding(0);
            this.mainPanel.Controls.Add(this.tablesPanel);
            this.mainPanel.Controls.Add(this.itemsPanel);
            this.mainPanel.Visible = true;
            this.tablesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablesPanel.BackColor = System.Drawing.Color.White;
            this.tablesPanel.Padding = new System.Windows.Forms.Padding(40);
            this.tablesPanel.Visible = false;
            this.itemsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsPanel.BackColor = System.Drawing.Color.White;
            this.itemsPanel.Padding = new System.Windows.Forms.Padding(40);
            this.itemsPanel.Visible = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 850);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Restaurant Management System";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ordersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel tablesPanel;
        private System.Windows.Forms.Panel itemsPanel;
    }
}
