using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Excel_Import;
using Final_Project;
using datahold;


namespace Login
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // username box
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Password box
            textBox2.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes
            string username = textBox1.Text;
            string password = textBox2.Text;
            string database = textBox3.Text;
            string server = textBox4.Text;

            // Set values in Config class (this will persist in memory during runtime so that it will be lost when stoped)
            Config.Username = username;
            Config.Password = password;
            Config.Database = database;
            Config.Server = server;

            // Attempt to connect to the database using the saved settings
            using (SqlConnection connection = new SqlConnection(Config.GetConnectionString()))
            {
                try
                {
                    connection.Open();  // Try to open a connection to the database
                                        // Open Final_project form after successful login
                    Main_Page finalProjectForm = new Main_Page();
                    finalProjectForm.Show();
                    // Hide the login form
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection failed :( : {ex.Message}");
                }
            }
        }



        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

