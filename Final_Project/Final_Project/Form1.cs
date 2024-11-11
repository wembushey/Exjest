using System;
using Excel_Import;
using datahold;
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
    }
}
