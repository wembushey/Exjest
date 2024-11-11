using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using Microsoft.Data.SqlClient;
using datahold;
namespace Excel_Import
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
            // Enable drag-and-drop on the form
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(ImportForm_DragEnter);
            this.DragDrop += new DragEventHandler(ImportForm_DragDrop);
        }

        // Event handler that does a file type check
        private void ImportForm_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    MessageBox.Show("Connection String: " + Config.GetConnectionString());


                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in drag enter: {ex.Message}");
            }
        }

        // Event handler for DragDrop to handle the dropped file
        private void ImportForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    string filePath = files[0];
                    ImportExcelData(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling file drop: {ex.Message}");
            }
        }

        // Method to import data from the Excel file
        private void ImportExcelData(string filePath)
        {
            try
            {
                // Open the Excel file
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    var sheets = workbookPart.Workbook.Sheets.Elements<Sheet>();

                    // Connect to the database using the connection string from Config
                    using (SqlConnection connection = new SqlConnection(Config.GetConnectionString()))
                    {
                        try
                        {
                            connection.Open();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to connect to the database: {ex.Message}");
                            return; // Exit early if the connection fails
                        }

                        foreach (var sheet in sheets)
                        {
                            try
                            {
                                // Get the sheet name
                                string tableName = sheet.Name;
                                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                                // Create table dynamically based on sheet name and columns
                                CreateTableIfNotExist(tableName, sheetData, connection);

                                // Insert rows dynamically
                                InsertRowsIfNotExist(sheetData, tableName, connection, workbookPart);  // Pass workbookPart
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error processing sheet '{sheet.Name}': {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Excel file: {ex.Message}");
            }
        }

        // Helper method to dynamically create a table if it doesn't exist
        private void CreateTableIfNotExist(string tableName, SheetData sheetData, SqlConnection connection)
        {
            try
            {
                var columns = sheetData.Elements<Row>().FirstOrDefault()?.Elements<Cell>()
                    .Select(cell => GetCellValue(cell, null)).ToList(); // Use GetCellValue with null for first row (headers)

                if (columns == null || !columns.Any())
                    return;

                StringBuilder createTableQuery = new StringBuilder($"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') CREATE TABLE {tableName} (");

                foreach (var column in columns)
                {
                    // Assuming all columns are of type NVARCHAR(255) - adjust as necessary
                    createTableQuery.Append($"[{column}] NVARCHAR(255),");
                }
                createTableQuery.Length--; // Remove last comma
                createTableQuery.Append(");");

                using (SqlCommand command = new SqlCommand(createTableQuery.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating table '{tableName}': {ex.Message}");
            }
        }

        // Helper method to insert rows if they do not exist
        private void InsertRowsIfNotExist(SheetData sheetData, string tableName, SqlConnection connection, WorkbookPart workbookPart)
        {
            try
            {
                foreach (Row row in sheetData.Elements<Row>().Skip(1)) // Skip the header row
                {
                    var cellValues = row.Elements<Cell>().Select(cell => GetCellValue(cell, workbookPart)).ToList();

                    // Generate a query to check if the row already exists
                    var checkExistQuery = $"SELECT COUNT(*) FROM {tableName} WHERE " + string.Join(" AND ", cellValues.Select((value, index) => $"Column{index + 1} = @Value{index + 1}"));
                    using (SqlCommand checkCommand = new SqlCommand(checkExistQuery, connection))
                    {
                        for (int i = 0; i < cellValues.Count; i++)
                        {
                            checkCommand.Parameters.AddWithValue($"@Value{i + 1}", cellValues[i]);
                        }

                        try
                        {
                            int count = (int)checkCommand.ExecuteScalar();
                            if (count == 0)
                            {
                                // Construct insert query if the row does not exist
                                var insertQuery = $"INSERT INTO {tableName} ({string.Join(",", cellValues.Select((value, index) => $"Column{index + 1}"))}) VALUES ({string.Join(",", cellValues.Select((value, index) => $"@Value{index + 1}"))})";
                                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                {
                                    for (int i = 0; i < cellValues.Count; i++)
                                    {
                                        insertCommand.Parameters.AddWithValue($"@Value{i + 1}", cellValues[i]);
                                    }

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error checking row existence in table '{tableName}': {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting rows into table '{tableName}': {ex.Message}");
            }
        }

        // Helper method to get the cell value as text
        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            try
            {
                string value = cell.CellValue?.Text;
                if (cell.DataType?.Value == CellValues.SharedString && workbookPart != null)
                {
                    // Retrieve the shared string using the index
                    return GetSharedStringValue(workbookPart, value);
                }
                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting cell value: {ex.Message}");
                return string.Empty; // Return empty string if an error occurs
            }
        }

        // Helper method to retrieve shared string value by index
        private string GetSharedStringValue(WorkbookPart workbookPart, string value)
        {
            try
            {
                int sharedStringIndex = int.Parse(value);
                SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;

                // Return the value of the shared string
                if (sharedStringTablePart != null)
                {
                    SharedStringTable sharedStringTable = sharedStringTablePart.SharedStringTable;
                    return sharedStringTable.Elements<SharedStringItem>().ElementAt(sharedStringIndex).InnerText;
                }
                return string.Empty; // Return an empty string if the shared string table is not found
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving shared string value: {ex.Message}");
                return string.Empty; // Return empty string if an error occurs
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Implement if needed
        }
    }
}

