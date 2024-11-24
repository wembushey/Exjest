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
            exitToolStripMenuItem = new ToolStripMenuItem();
            importToolStripMenuItem = new ToolStripMenuItem();
            xlsxToolStripMenuItem = new ToolStripMenuItem();
            usersToolStripMenuItem = new ToolStripMenuItem();
            addUserToolStripMenuItem = new ToolStripMenuItem();
            removeUserToolStripMenuItem = new ToolStripMenuItem();
            modifyUserToolStripMenuItem = new ToolStripMenuItem();
            dataViewsToolStripMenuItem = new ToolStripMenuItem();
            incidentsToolStripMenuItem = new ToolStripMenuItem();
            companiesToolStripMenuItem = new ToolStripMenuItem();
            railroadsToolStripMenuItem = new ToolStripMenuItem();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            button1 = new Button();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.Items.AddRange(new ToolStripItem[] { exitToolStripMenuItem, importToolStripMenuItem, usersToolStripMenuItem, dataViewsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1924, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(38, 20);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.BackColor = Color.Transparent;
            importToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { xlsxToolStripMenuItem });
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Padding = new Padding(0);
            importToolStripMenuItem.Size = new Size(47, 20);
            importToolStripMenuItem.Text = "Import";
            // 
            // xlsxToolStripMenuItem
            // 
            xlsxToolStripMenuItem.Name = "xlsxToolStripMenuItem";
            xlsxToolStripMenuItem.Size = new Size(97, 22);
            xlsxToolStripMenuItem.Text = ".xlsx";
            xlsxToolStripMenuItem.Click += xlsxToolStripMenuItem_Click;
            // 
            // usersToolStripMenuItem
            // 
            usersToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addUserToolStripMenuItem, removeUserToolStripMenuItem, modifyUserToolStripMenuItem });
            usersToolStripMenuItem.Name = "usersToolStripMenuItem";
            usersToolStripMenuItem.Size = new Size(47, 20);
            usersToolStripMenuItem.Text = "Users";
            // 
            // addUserToolStripMenuItem
            // 
            addUserToolStripMenuItem.Name = "addUserToolStripMenuItem";
            addUserToolStripMenuItem.Size = new Size(168, 22);
            addUserToolStripMenuItem.Text = "Add User";
            addUserToolStripMenuItem.Click += addUserToolStripMenuItem_Click;
            // 
            // removeUserToolStripMenuItem
            // 
            removeUserToolStripMenuItem.Name = "removeUserToolStripMenuItem";
            removeUserToolStripMenuItem.Size = new Size(168, 22);
            removeUserToolStripMenuItem.Text = "Remove User";
            removeUserToolStripMenuItem.Click += removeUserToolStripMenuItem_Click;
            // 
            // modifyUserToolStripMenuItem
            // 
            modifyUserToolStripMenuItem.Name = "modifyUserToolStripMenuItem";
            modifyUserToolStripMenuItem.Size = new Size(168, 22);
            modifyUserToolStripMenuItem.Text = "Change Password";
            modifyUserToolStripMenuItem.Click += modifyUserToolStripMenuItem_Click;
            // 
            // dataViewsToolStripMenuItem
            // 
            dataViewsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { incidentsToolStripMenuItem, companiesToolStripMenuItem, railroadsToolStripMenuItem });
            dataViewsToolStripMenuItem.Name = "dataViewsToolStripMenuItem";
            dataViewsToolStripMenuItem.Size = new Size(76, 20);
            dataViewsToolStripMenuItem.Text = "Data Views";
            // 
            // incidentsToolStripMenuItem
            // 
            incidentsToolStripMenuItem.Name = "incidentsToolStripMenuItem";
            incidentsToolStripMenuItem.Size = new Size(134, 22);
            incidentsToolStripMenuItem.Text = "Incidents";
            incidentsToolStripMenuItem.Click += incidentsToolStripMenuItem_Click;
            // 
            // companiesToolStripMenuItem
            // 
            companiesToolStripMenuItem.Name = "companiesToolStripMenuItem";
            companiesToolStripMenuItem.Size = new Size(134, 22);
            companiesToolStripMenuItem.Text = "Companies";
            companiesToolStripMenuItem.Click += companiesToolStripMenuItem_Click;
            // 
            // railroadsToolStripMenuItem
            // 
            railroadsToolStripMenuItem.Name = "railroadsToolStripMenuItem";
            railroadsToolStripMenuItem.Size = new Size(134, 22);
            railroadsToolStripMenuItem.Text = "Railroads";
            railroadsToolStripMenuItem.Click += railroadsToolStripMenuItem_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.Window;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Location = new Point(12, 27);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 14);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.Window;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Location = new Point(12, 47);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 14);
            textBox2.TabIndex = 2;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(799, 47);
            label1.Name = "label1";
            label1.Size = new Size(174, 29);
            label1.TabIndex = 3;
            label1.Text = "Enter Incident";
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.Window;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(36, 171);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1656, 65);
            dataGridView1.TabIndex = 4;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // button1
            // 
            button1.Location = new Point(853, 261);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Enter Data";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Main_Page
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(1924, 929);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(menuStrip1);
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MainMenuStrip = menuStrip1;
            Name = "Main_Page";
            Text = "Exjest";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem xlsxToolStripMenuItem;
        private ToolStripMenuItem usersToolStripMenuItem;
        private ToolStripMenuItem addUserToolStripMenuItem;
        private ToolStripMenuItem removeUserToolStripMenuItem;
        private ToolStripMenuItem modifyUserToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem dataViewsToolStripMenuItem;
        private ToolStripMenuItem incidentsToolStripMenuItem;
        private ToolStripMenuItem companiesToolStripMenuItem;
        private ToolStripMenuItem railroadsToolStripMenuItem;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label1;
        private DataGridView dataGridView1;
        private Button button1;
    }
}
