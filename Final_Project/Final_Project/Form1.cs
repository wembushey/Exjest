using System;
using Excel_Import;
using datahold;
using UserControl;
namespace Final_Project
{
    public partial class Main_Page : Form
    {
        public Main_Page()
        {
            InitializeComponent();
        }

        private void xlsxToolStripMenuItem_Click(object sender, EventArgs e)
        {// loads the Import page
         // Open Final_project form after successful login
            ImportForm finalProjectForm = new ImportForm();
            finalProjectForm.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            // Create an instance of Form1 from the UserControl project
            Form1 form1 = new Form1();

            // Show Form1 as a modal dialog
            form1.ShowDialog();
        }

        private void removeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void modifyUserToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
