using System;
using System.Data;
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
            // Optional: Any code to run on form load
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Get the username and password from textboxes
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Connection string from app.config
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    // SQL query to check if the user exists
                    string query = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password";

                    // Create SQL command
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        // Execute the query and check if the user exists
                        int userCount = (int)cmd.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Login successful");
                            // Proceed to next form or functionality
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
    }
}

