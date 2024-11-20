using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
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
        
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Username label click
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Username box logic
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Password box logic
            textBox2.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes (user credentials)
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Set values in Config class (optional, can be used elsewhere in the program)
            Config.Username = username;
            Config.Password = password;

            // Get the connection string from app.config for connecting to the database
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string is missing or invalid in app.config.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a new SQL connection using the connection string from app.config
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Check if the account exists and the password is correct by querying the database
                    bool isAccountValid = CheckAccountCredentials(connection, username, password);

                    if (isAccountValid)
                    {
                        Main_Page finalProjectForm = new Main_Page();
                        finalProjectForm.Show();
                        this.Hide();
                    }
                    else
                    {
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
            // SQL query to retrieve the hashed password and salt for the given username
            string query = "SELECT password, salt FROM Users WHERE name = @Username";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Retrieve the stored hashed password and salt
                        string storedHashedPassword = reader["password"].ToString();
                        string salt = reader["salt"].ToString();

                        // Hash the entered password with the retrieved salt
                        string hashedPassword = HashPassword(password, salt);

                        // Compare the hashed password with the stored hashed password
                        return hashedPassword == storedHashedPassword;
                    }
                    else
                    {
                        // No matching username found
                        return false;
                    }
                }
            }
        }

        // Method to hash the password with the salt
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
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
