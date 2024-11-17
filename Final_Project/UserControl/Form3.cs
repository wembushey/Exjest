using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace UserControl
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string newPassword = textBox3.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Both username and new password are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Generate new salt and hash the new password
                    string newSalt = GenerateSalt();
                    string hashedPassword = HashPassword(newPassword, newSalt);

                    // Delete old row and reinsert with new credentials
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Delete old record
                            string deleteQuery = "DELETE FROM Users WHERE name = @Username";
                            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@Username", username);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Insert new record
                            string insertQuery = "INSERT INTO Users (name, password, salt) VALUES (@Username, @Password, @Salt)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@Username", username);
                                insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                                insertCommand.Parameters.AddWithValue("@Salt", newSalt);
                                insertCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + password); // Ensure same order
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Logic for when the text changes, if necessary
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Logic for when the text changes, if necessary
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Logic for when the text changes, if necessary
        }

    }
}
