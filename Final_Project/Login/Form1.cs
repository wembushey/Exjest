using System;
using System.Data.SqlClient;
using System.Windows.Forms;

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
            // This is the username box
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // This is the password box
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Login button
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Connection string with placeholders for username and password
            string connectionString = $"Server=your_server;Database=your_database;User Id={username};Password={password};";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Connection successful!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection failed: {ex.Message}");
                }
            }
        }
    }
}

