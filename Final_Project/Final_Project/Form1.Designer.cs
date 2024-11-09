namespace Final_Project
{
    partial class Main_Page
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            importToolStripMenuItem = new ToolStripMenuItem();
            xlsxToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.Gray;
            menuStrip1.Items.AddRange(new ToolStripItem[] { importToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.BackColor = Color.Silver;
            importToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { xlsxToolStripMenuItem });
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Padding = new Padding(0);
            importToolStripMenuItem.Size = new Size(47, 20);
            importToolStripMenuItem.Text = "Import";
            // 
            // xlsxToolStripMenuItem
            // 
            xlsxToolStripMenuItem.Name = "xlsxToolStripMenuItem";
            xlsxToolStripMenuItem.Size = new Size(180, 22);
            xlsxToolStripMenuItem.Text = ".xlsx";
            xlsxToolStripMenuItem.Click += xlsxToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            ClientSize = new Size(800, 450);
            Controls.Add(menuStrip1);
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "INSERT_NAME";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem xlsxToolStripMenuItem;
    }
}
