using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace Views
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Path to your Excel file
            string filePath = @"C:\Path\To\Your\ExcelFile.xlsx";

            // Load specific columns from the Excel file into the DataGridView
            LoadSpecificColumns(filePath);
        }

        private void LoadSpecificColumns(string filePath)
        {
            try
            {
                // Define the specific columns to load
                string[] columnsToLoad = { "company_id", "company_name", "org_type" };

                // Open the Excel file
                using (var workbook = new XLWorkbook(filePath))
                {
                    // Get the first worksheet
                    var worksheet = workbook.Worksheet(1);

                    // Create a DataTable to hold the specific data
                    DataTable dataTable = new DataTable();

                    // Read the headers from the first row
                    var headerRow = worksheet.Row(1).CellsUsed().ToList();

                    // Map the header columns to indices
                    var columnMappings = headerRow
                        .Select((cell, index) => new { Header = cell.Value.ToString(), Index = index + 1 }) // Excel indices are 1-based
                        .Where(mapping => columnsToLoad.Contains(mapping.Header, StringComparer.OrdinalIgnoreCase))
                        .ToDictionary(mapping => mapping.Header, mapping => mapping.Index);

                    // Add the selected columns to the DataTable
                    foreach (var columnName in columnsToLoad)
                    {
                        if (columnMappings.ContainsKey(columnName))
                        {
                            dataTable.Columns.Add(columnName);
                        }
                    }

                    // Read the rows and only include data from the selected columns
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        DataRow dataRow = dataTable.NewRow();

                        foreach (var columnName in columnsToLoad)
                        {
                            if (columnMappings.ContainsKey(columnName))
                            {
                                int columnIndex = columnMappings[columnName];
                                dataRow[columnName] = row.Cell(columnIndex).Value;
                            }
                        }

                        dataTable.Rows.Add(dataRow);
                    }

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Placeholder for cell click handling
        }
    }
}
