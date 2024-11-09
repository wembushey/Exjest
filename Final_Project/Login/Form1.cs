using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Final_Project;

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
            // Login button
            string username = textBox1.Text;
            string password = textBox2.Text;
            string database = textBox3.Text;
            string server = textBox4.Text;

            // Connection string with placeholders for username and password
            string connectionString = $"Server={server};Database={database};User Id={username};Password={password};";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
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

