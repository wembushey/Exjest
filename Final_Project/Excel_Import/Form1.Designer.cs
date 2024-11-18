namespace Excel_Import
{
    partial class ImportForm
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
            label1 = new Label();
            progressBar1 = new ProgressBar();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(124, 33);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 0;
            label1.Text = "Drop file here";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 121);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(310, 12);
            progressBar1.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(28, 59);
            label2.Name = "label2";
            label2.Size = new Size(245, 15);
            label2.TabIndex = 2;
            label2.Text = "This page will close after the data is imported";
            // 
            // ImportForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(334, 145);
            Controls.Add(label2);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "ImportForm";
            Text = "Import";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ProgressBar progressBar1;
        private Label label2;
    }
}
