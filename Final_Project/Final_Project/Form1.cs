using System;
using Excel_Import;
using datahold;
using UserControl;
namespace Final_Project;
using Views;
using System.Data.SqlClient;
using System.Configuration;

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
}
