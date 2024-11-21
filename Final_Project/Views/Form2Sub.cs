using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Views
{
    public partial class Form2Sub : Form
    {
        private string railroadId; // Store the passed railroad ID

        // Constructor that accepts a railroad ID
        public Form2Sub(string railroadId)
        {
            InitializeComponent();
            this.railroadId = railroadId;
            this.Load += Form2Sub_Load;
        }

        private void Form2Sub_Load(object sender, EventArgs e)
        {
            // Use the railroadId to load data or display information
            LoadIncidents();
        }

        private void LoadIncidents()
        {
            // Connection string from app.config
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string is missing or invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SQL query to fetch incidents for the selected railroad
            string query = "SELECT * FROM incident WHERE railroad_id = @railroadId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add the parameter for the railroad ID
                        command.Parameters.AddWithValue("@railroadId", railroadId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Bind the results to the DataGridView
                            dataGridView1.DataSource = dataTable;

                            // Dynamically adjust column sizes
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2Sub_Load_1(object sender, EventArgs e)
        {

        }
    }
}
