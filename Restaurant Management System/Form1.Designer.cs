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
            menuStrip1 = new MenuStrip();
            ordersToolStripMenuItem = new ToolStripMenuItem();
            menuToolStripMenuItem = new ToolStripMenuItem();
            mainPanel = new Panel();
            tablesPanel = new Panel();
            itemsPanel = new Panel();
            menuStrip1.SuspendLayout();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(0, 120, 215);
            menuStrip1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            menuStrip1.ForeColor = Color.White;
            menuStrip1.Items.AddRange(new ToolStripItem[] { ordersToolStripMenuItem, menuToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1225, 29);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // ordersToolStripMenuItem
            // 
            ordersToolStripMenuItem.Name = "ordersToolStripMenuItem";
            ordersToolStripMenuItem.Size = new Size(72, 25);
            ordersToolStripMenuItem.Text = "Orders";
            // 
            // menuToolStripMenuItem
            // 
            menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            menuToolStripMenuItem.Size = new Size(64, 25);
            menuToolStripMenuItem.Text = "Items";
            // 
            // mainPanel
            // 
            mainPanel.BackColor = Color.WhiteSmoke;
            mainPanel.Controls.Add(tablesPanel);
            mainPanel.Controls.Add(itemsPanel);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 29);
            mainPanel.Margin = new Padding(3, 2, 3, 2);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1225, 609);
            mainPanel.TabIndex = 1;
            // 
            // tablesPanel
            // 
            tablesPanel.BackColor = Color.White;
            tablesPanel.Dock = DockStyle.Fill;
            tablesPanel.Location = new Point(0, 0);
            tablesPanel.Margin = new Padding(3, 2, 3, 2);
            tablesPanel.Name = "tablesPanel";
            tablesPanel.Padding = new Padding(35, 30, 35, 30);
            tablesPanel.Size = new Size(1225, 609);
            tablesPanel.TabIndex = 0;
            tablesPanel.Visible = false;
            //tablesPanel.Paint += tablesPanel_Paint;
            // 
            // itemsPanel
            // 
            itemsPanel.BackColor = Color.White;
            itemsPanel.Dock = DockStyle.Fill;
            itemsPanel.Location = new Point(0, 0);
            itemsPanel.Margin = new Padding(3, 2, 3, 2);
            itemsPanel.Name = "itemsPanel";
            itemsPanel.Padding = new Padding(35, 30, 35, 30);
            itemsPanel.Size = new Size(1225, 609);
            itemsPanel.TabIndex = 1;
            itemsPanel.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1225, 638);
            Controls.Add(mainPanel);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Restaurant Management System";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ordersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel tablesPanel;
        private System.Windows.Forms.Panel itemsPanel;
    }
}
