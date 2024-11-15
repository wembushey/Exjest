using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace UserControl
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        // Reference connection string from app.config
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the username from the textbox
            string usernameToRemove = textBox1.Text.Trim();

            // Validate that the username is not empty
            if (string.IsNullOrEmpty(usernameToRemove))
            {
                MessageBox.Show("Please enter a username to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm removal action
            var confirmResult = MessageBox.Show($"Are you sure you want to remove user '{usernameToRemove}'?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // Attempt to remove the user from the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Define the SQL query to delete the user
                        string deleteQuery = "DELETE FROM Users WHERE name = @Username";

                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            // Add parameter for username
                            command.Parameters.AddWithValue("@Username", usernameToRemove);

                            // Execute the delete command
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"User '{usernameToRemove}' has been successfully removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                textBox1.Clear(); // Clear the input
                            }
                            else
                            {
                                MessageBox.Show($"User '{usernameToRemove}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that occur during the database operation
                        MessageBox.Show($"An error occurred while trying to remove the user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Logic for username input can be handled here if needed
        }
    }
}
