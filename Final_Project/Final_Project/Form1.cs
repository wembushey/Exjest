using System;
using Excel_Import;
using datahold;
using UserControl;
namespace Final_Project;
using Views;

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
            UserControl.Form1 form1 = new UserControl.Form1();

            // Show Form1 as a modal dialog
            form1.ShowDialog();
        }

        private void removeUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void modifyUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void incidentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void companiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        Views.Form1 form1 = new Views.Form1();
        form1.ShowDialog();
    }
    }

