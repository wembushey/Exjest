using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Views
{
    public partial class Form3 : Form
    {
        private DataTable incidentTable; // Store the DataTable for filtering

        public Form3()
        {
            InitializeComponent();
            this.Load += Form3_Load;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            InitializeDateTimePicker();
            LoadSpecificColumns();
            dataGridView1.AllowUserToResizeRows = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
        }

        private void InitializeDateTimePicker()
        {
            // Start Date Picker (dateTimePicker1)
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = " "; // Set blank initially
            dateTimePicker1.ShowCheckBox = true; // Allow user to toggle date selection
            dateTimePicker1.Checked = false; // Uncheck initially
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            // End Date Picker (dateTimePicker2)
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = " "; // Set blank initially
            dateTimePicker2.ShowCheckBox = true; // Allow user to toggle date selection
            dateTimePicker2.Checked = false; // Uncheck initially
            dateTimePicker2.ValueChanged += dateTimePicker2_ValueChanged;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Checked)
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "yyyy-MM-dd"; // Set format when checked
            }
            else
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = " "; // Set blank format
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Checked)
            {
                dateTimePicker2.Format = DateTimePickerFormat.Custom;
                dateTimePicker2.CustomFormat = "yyyy-MM-dd"; // Set format when checked
            }
            else
            {
                dateTimePicker2.Format = DateTimePickerFormat.Custom;
                dateTimePicker2.CustomFormat = " "; // Set blank format
            }
        }


        private void LoadSpecificColumns()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            string query = @"
        SELECT 
            seqnos, 
            date_time_received, 
            date_time_complete, 
            call_type, 
            responsible_city, 
            responsible_state, 
            responsible_zip, 
            description_of_incident, 
            type_of_incident, 
            incident_cause, 
            injury_count, 
            hospitalization_count, 
            fatality_count, 
            company_id, 
            railroad_id, 
            incident_train_id
        FROM incident;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Load all columns into the incidentTable
                            incidentTable = new DataTable();
                            adapter.Fill(incidentTable);

                            // Create a new DataTable with only the visible columns
                            DataTable visibleTable = incidentTable.DefaultView.ToTable(false,
                                "seqnos",
                                "date_time_received",
                                "call_type",
                                "responsible_state",
                                "type_of_incident");

                            // Bind the visible columns to the DataGridView
                            dataGridView1.DataSource = visibleTable;

                            // Adjust column widths for better visibility
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            if (incidentTable != null)
            {
                DataView dataView = incidentTable.DefaultView;
                string filterText = textBox1.Text;

                try
                {
                    if (!string.IsNullOrEmpty(filterText))
                    {
                        // Apply the filter on the seqnos column
                        dataView.RowFilter = $"Convert(seqnos, 'System.String') LIKE '%{filterText}%'";
                    }
                    else
                    {
                        // Reset filter if text is empty
                        dataView.RowFilter = string.Empty;
                    }

                    // Update DataGridView to show only visible columns with the filtered data
                    DataTable filteredTable = dataView.ToTable(false,
                        "seqnos",
                        "date_time_complete",
                        "call_type",
                        "responsible_state",
                        "type_of_incident");

                    dataGridView1.DataSource = filteredTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (incidentTable != null)
            {
                DataView dataView = incidentTable.DefaultView;

                // Collect filters
                List<string> filters = new List<string>();

                try
                {
                    // Filter by State (responsible_state, textBox2)
                    if (!string.IsNullOrWhiteSpace(textBox2.Text))
                    {
                        filters.Add($"responsible_state LIKE '%{textBox2.Text}%'");
                    }

                    // Filter by Date Range (date_time_complete)
                    if (dateTimePicker1.Checked && dateTimePicker2.Checked)
                    {
                        filters.Add($"date_time_complete >= #{dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss")}#");
                        filters.Add($"date_time_complete <= #{dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss")}#");
                    }
                    else if (dateTimePicker1.Checked)
                    {
                        filters.Add($"date_time_complete >= #{dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss")}#");
                    }
                    else if (dateTimePicker2.Checked)
                    {
                        filters.Add($"date_time_complete <= #{dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss")}#");
                    }

                    // Filter by Fatality Count (fatality_count, textBox3)
                    if (!string.IsNullOrWhiteSpace(textBox3.Text) && int.TryParse(textBox3.Text, out int fatalityCount))
                    {
                        filters.Add($"fatality_count = {fatalityCount}");
                    }

                    // Filter by Injury Count (injury_count, textBox4)
                    if (!string.IsNullOrWhiteSpace(textBox4.Text) && int.TryParse(textBox4.Text, out int injuryCount))
                    {
                        filters.Add($"injury_count = {injuryCount}");
                    }

                    // Combine filters using AND
                    string combinedFilter = string.Join(" AND ", filters);
                    dataView.RowFilter = combinedFilter;

                    // Update DataGridView to show only visible columns with the filtered data
                    DataTable filteredTable = dataView.ToTable(false,
                        "seqnos",
                        "date_time_complete",
                        "call_type",
                        "responsible_state",
                        "type_of_incident");

                    dataGridView1.DataSource = filteredTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        private void textBox3_TextChanged(object sender, EventArgs e)
        { // fatality count

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        { // injuries count 

        }

        
    }
}
