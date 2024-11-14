using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using datahold;
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
            // Test connection string when the form is loaded
            if (IsConnectionStringValid())
            {
                MessageBox.Show("Connection string is valid.");
            }
            else
            {
                MessageBox.Show("Connection string is invalid.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // username label click
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // username box logic
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Password box logic
            textBox2.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Set values in Config class (this will persist in memory during runtime so that it will be lost when stopped)
            Config.Username = username;
            Config.Password = password;

            // Create a new SQL connection using the connection string
            using (SqlConnection connection = new SqlConnection(Config.GetConnectionString()))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Check if the account exists and the password is correct
                    bool isAccountValid = CheckAccountCredentials(connection, username, password);

                    // If the account is valid, proceed to the next form
                    if (isAccountValid)
                    {
                        Main_Page finalProjectForm = new Main_Page();
                        finalProjectForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Show an error message if the username or password is incorrect
                        MessageBox.Show("Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    // Display any errors that occur while attempting to connect
                    MessageBox.Show($"Connection failed :( : {ex.Message}");
                }
            }
        }

        // Method to check if the username and password match in the database
        private bool CheckAccountCredentials(SqlConnection connection, string username, string password)
        {
            // Define the SQL query to check if the username and password match
            string query = "SELECT COUNT(1) FROM Users WHERE name = @Username AND password = @Password";

            // Create a new SQL command using the query and connection
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                // Execute the query and get the result (count of matching rows)
                int result = Convert.ToInt32(command.ExecuteScalar());

                // Return true if a matching row was found, else false
                return result > 0;
            }
        }

        // Method to check if the connection string is valid
        private bool IsConnectionStringValid()
        {
            try
            {
                // Retrieve the connection string from the app.config
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                // Check if the connection string is null or empty
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Connection string is missing or invalid in app.config.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Test the connection using the connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Try to open the connection
                }

                // If successful, return true
                return true;
            }
            catch (SqlException sqlEx)
            {
                // Specific error for SQL related exceptions
                MessageBox.Show($"SQL Error: {sqlEx.Message}", "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle other general exceptions
                MessageBox.Show($"Error: {ex.Message}", "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // If connection failed, return false
            return false;
        }
    }
}
