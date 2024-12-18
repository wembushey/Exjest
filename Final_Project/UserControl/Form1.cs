using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace UserControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Reference connection string from app.config
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        private void label2_Click(object sender, EventArgs e)
        {
            // Existing label2 logic (if any)
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Existing label1 logic (if any)
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Username textbox logic (if any)
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
            // Password textbox logic (if any)
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes
            string username = textBox2.Text;
            string password = textBox1.Text;
            string showName = textBox3.Text; // Get the show name

            // Generate salt and hash the password with the salt
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            // Store username, hashed password, salt, and show name in the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Users (name, password, salt, showname) VALUES (@Username, @Password, @Salt, @ShowName)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@ShowName", showName); // Add show name parameter

                        command.ExecuteNonQuery();
                        MessageBox.Show("User created successfully!");

                        // Clear textboxes
                        textBox1.Text = string.Empty;
                        textBox2.Text = string.Empty;
                        textBox3.Text = string.Empty;
                        textBox2.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        { // show name

        }
    }
}
