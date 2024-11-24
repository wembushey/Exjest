using System;
using Excel_Import;
using datahold;
using UserControl;
using System.Data.SqlClient;
using System.Configuration;
namespace Final_Project;
using Views;

public partial class Main_Page : Form
{
    public string LoggedInUsername { get; set; } // Add property to store the logged-in username

    public Main_Page()
    {
        InitializeComponent();
    }

    private void xlsxToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ImportForm finalProjectForm = new ImportForm();
        finalProjectForm.Show();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.Sizable;

        textBox1.ReadOnly = true; // Make the TextBox read-only
        if (!string.IsNullOrEmpty(LoggedInUsername))
        {
            LoadShowName(LoggedInUsername); // Load the showname for the logged-in username
        }

        SetupDataGridViewColumns(); // Initialize DataGridView columns for data entry
    }

    private void toolStripTextBox1_Click(object sender, EventArgs e)
    {

    }

    private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
    {
        UserControl.Form1 form1 = new UserControl.Form1();
        form1.ShowDialog();
    }

    private void removeUserToolStripMenuItem_Click(object sender, EventArgs e)
    {
        UserControl.Form2 form2 = new UserControl.Form2();
        form2.ShowDialog();
    }

    private void modifyUserToolStripMenuItem_Click(object sender, EventArgs e)
    {
        UserControl.Form3 form3 = new UserControl.Form3();
        form3.ShowDialog();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void incidentsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Views.Form3 form3 = new Views.Form3();
        form3.ShowDialog();
    }

    private void companiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Views.Form1 form1 = new Views.Form1();
        form1.ShowDialog();
    }

    private void railroadsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Views.Form2 form2 = new Views.Form2();
        form2.ShowDialog();
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        // Logic not needed here; handled by LoadShowName
    }

    private void LoadShowName(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Fetch showname
                    string shownameQuery = "SELECT showname FROM Users WHERE name = @Username";
                    using (SqlCommand shownameCommand = new SqlCommand(shownameQuery, connection))
                    {
                        shownameCommand.Parameters.AddWithValue("@Username", username);

                        object shownameResult = shownameCommand.ExecuteScalar();
                        if (shownameResult != null)
                        {
                            textBox1.Text = "Welcome " + shownameResult.ToString(); // Set showname in textBox1
                        }
                        else
                        {
                            textBox1.Text = "Show name not found"; // Fallback message
                        }
                    }

                    // Fetch name
                    string nameQuery = "SELECT name FROM Users WHERE name = @Username";
                    using (SqlCommand nameCommand = new SqlCommand(nameQuery, connection))
                    {
                        nameCommand.Parameters.AddWithValue("@Username", username);

                        object nameResult = nameCommand.ExecuteScalar();
                        if (nameResult != null)
                        {
                            textBox2.Text = "Username " + nameResult.ToString(); // Set name in textBox2
                        }
                        else
                        {
                            textBox2.Text = "Name not found"; // Fallback message
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching data: {ex.Message}");
                }
            }
        }
        else
        {
            textBox1.Text = "Enter a username first"; // Message for empty username
            textBox2.Text = ""; // Clear namebox
        }
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    { // name box

    }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    { // this is the datagridview that is used to enter data

    }

    private void SetupDataGridViewColumns()
    {
        // Set up DataGridView columns for entering incident data
        dataGridView1.Columns.Add("seqnos", "Seqnos");
        dataGridView1.Columns.Add("date_time_received", "Date Time Received");
        dataGridView1.Columns.Add("date_time_complete", "Date Time Complete");
        dataGridView1.Columns.Add("call_type", "Call Type");
        dataGridView1.Columns.Add("responsible_city", "Responsible City");
        dataGridView1.Columns.Add("responsible_state", "Responsible State");
        dataGridView1.Columns.Add("responsible_zip", "Responsible Zip");
        dataGridView1.Columns.Add("description_of_incident", "Description of Incident");
        dataGridView1.Columns.Add("type_of_incident", "Type of Incident");
        dataGridView1.Columns.Add("incident_cause", "Incident Cause");
        dataGridView1.Columns.Add("injury_count", "Injury Count");
        dataGridView1.Columns.Add("hospitalization_count", "Hospitalization Count");
        dataGridView1.Columns.Add("fatality_count", "Fatality Count");
        dataGridView1.Columns.Add("company_id", "Company ID");
        dataGridView1.Columns.Add("railroad_id", "Railroad ID");
        dataGridView1.Columns.Add("incident_train_id", "Incident Train ID");

        dataGridView1.AllowUserToAddRows = true; // Allow adding rows
        dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter; // Allow editing on enter
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            MessageBox.Show("Connection string is missing or invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the last empty row

                    // Prepare the INSERT query
                    string query = @"
                    INSERT INTO incident 
                    (date_time_received, date_time_complete, call_type, responsible_city, 
                    responsible_state, responsible_zip, description_of_incident, 
                    type_of_incident, incident_cause, injury_count, hospitalization_count, 
                    fatality_count, company_id, railroad_id, incident_train_id) 
                    OUTPUT INSERTED.seqnos
                    VALUES 
                    (@DateTimeReceived, @DateTimeComplete, @CallType, @ResponsibleCity, 
                    @ResponsibleState, @ResponsibleZip, @DescriptionOfIncident, 
                    @TypeOfIncident, @IncidentCause, @InjuryCount, @HospitalizationCount, 
                    @FatalityCount, @CompanyId, @RailroadId, @IncidentTrainId);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters from DataGridView cells
                        command.Parameters.AddWithValue("@DateTimeReceived", row.Cells["date_time_received"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DateTimeComplete", row.Cells["date_time_complete"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CallType", row.Cells["call_type"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResponsibleCity", row.Cells["responsible_city"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResponsibleState", row.Cells["responsible_state"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResponsibleZip", row.Cells["responsible_zip"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DescriptionOfIncident", row.Cells["description_of_incident"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@TypeOfIncident", row.Cells["type_of_incident"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IncidentCause", row.Cells["incident_cause"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@InjuryCount", row.Cells["injury_count"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HospitalizationCount", row.Cells["hospitalization_count"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@FatalityCount", row.Cells["fatality_count"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyId", row.Cells["company_id"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RailroadId", row.Cells["railroad_id"].Value ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IncidentTrainId", row.Cells["incident_train_id"].Value ?? DBNull.Value);

                        // Execute the query and retrieve the auto-generated seqnos
                        object seqnosValue = command.ExecuteScalar();
                        if (seqnosValue != null)
                        {
                            row.Cells["seqnos"].Value = seqnosValue; // Set the seqnos value in the DataGridView
                        }
                    }
                }

                MessageBox.Show("Data inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
