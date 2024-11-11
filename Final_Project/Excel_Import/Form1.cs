using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Data;
using datahold;

namespace Excel_Import
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(ImportForm_DragEnter);
            this.DragDrop += new DragEventHandler(ImportForm_DragDrop);
        }

        private void ImportForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void ImportForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    ImportExcelData(filePath);
                }
            }
        }

        private void ImportExcelData(string filePath)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                var sheets = workbookPart.Workbook.Sheets.Elements<Sheet>();

                using (SqlConnection connection = new SqlConnection(Config.GetConnectionString()))
                {
                    connection.Open();

                    foreach (var sheet in sheets)
                    {
                        string tableName = sheet.Name;
                        WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                        var headerRow = sheetData.Elements<Row>().FirstOrDefault();
                        var columns = headerRow.Elements<Cell>().Select(cell => GetCellValue(cell, workbookPart)).ToList();

                        CreateTableIfNotExist(tableName, columns, connection);

                        foreach (Row row in sheetData.Elements<Row>().Skip(1))
                        {
                            var cellValues = row.Elements<Cell>()
                                                 .Select(cell => GetCellValue(cell, workbookPart))
                                                 .ToList();

                            // Log cell values for debugging
                            Console.WriteLine("Inserting row for table: " + tableName);
                            for (int i = 0; i < cellValues.Count; i++)
                            {
                                Console.WriteLine($"Column {columns[i]}: Value '{cellValues[i]}'");
                            }

                            InsertRow(tableName, connection, columns, cellValues);
                        }
                    }
                }
            }
        }

        private void CreateTableIfNotExist(string tableName, List<string> columns, SqlConnection connection)
        {
            StringBuilder createTableQuery = new StringBuilder($"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') CREATE TABLE {tableName} (");

            foreach (var column in columns)
            {
                createTableQuery.Append($"[{column}] NVARCHAR(MAX),");
            }

            createTableQuery.Length--;  // Remove last comma
            createTableQuery.Append(");");

            using (SqlCommand command = new SqlCommand(createTableQuery.ToString(), connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertRow(string tableName, SqlConnection connection, List<string> columns, List<string> cellValues)
        {
            // Ensure there are enough values for the columns
            while (cellValues.Count < columns.Count)
            {
                cellValues.Add(null);  // Add null for missing values
            }

            // Log the number of columns and values for debugging
            Console.WriteLine($"Columns: {columns.Count}, Values: {cellValues.Count}");

            // Handle columns with 'NOT NULL' constraints
            for (int i = 0; i < columns.Count; i++)
            {
                if (IsNotNullColumn(columns[i]) && string.IsNullOrEmpty(cellValues[i]?.ToString()))
                {
                    cellValues[i] = null;  // Keep it as null for now
                    Console.WriteLine($"Default value for column {columns[i]}: {cellValues[i]}");
                }
            }

            // Create the insert query
            var sanitizedColumns = columns.Select(col => $"[{col}]").ToList();
            var insertQuery = $@"
INSERT INTO [{tableName}] 
({string.Join(",", sanitizedColumns)}) 
VALUES ({string.Join(",", Enumerable.Range(0, columns.Count).Select(i => $"@Value{i}"))})";

            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
            {
                // Loop through cellValues and assign DBNull.Value for null or empty values
                for (int i = 0; i < cellValues.Count; i++)
                {
                    // Use DBNull.Value for null fields, but leave strings as is
                    var valueToInsert = cellValues[i] == null ? DBNull.Value : (object)cellValues[i];
                    insertCommand.Parameters.AddWithValue($"@Value{i}", valueToInsert);
                }

                // Log the final SQL query and parameters
                Console.WriteLine("SQL Query: " + insertQuery);
                foreach (SqlParameter param in insertCommand.Parameters)
                {
                    Console.WriteLine($"{param.ParameterName}: {param.Value}");
                }

                // Execute the insert statement
                insertCommand.ExecuteNonQuery();
            }
        }



        private bool IsNotNullColumn(string columnName)
        {
            // Check if a column is marked as NOT NULL
            // This can be done by querying the database schema or by predefined logic
            // For this example, let's assume that 'columnName' is a non-nullable column if it matches specific criteria
            return true; // Example condition; customize as needed
        }

        private string GetDefaultValueForColumn(string columnName)
        {
            // Return a default value for a non-nullable column (if needed)
            // This could be a placeholder value or an empty string
            return "";  // Example default value; customize as needed
        }

        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            // Return empty string if the cell is empty
            if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text))
                return null;

            string value = cell.CellValue.Text;

            // Handle shared strings (i.e., text stored in a shared string table)
            if (cell.DataType?.Value == CellValues.SharedString && workbookPart != null)
            {
                int index = int.Parse(value);
                var sharedStringTable = workbookPart.SharedStringTablePart.SharedStringTable;
                return sharedStringTable.Elements<SharedStringItem>().ElementAt(index).InnerText;
            }

            // Handle date values
            if (DateTime.TryParse(value, out DateTime dateValue))
            {
                return dateValue.ToString("yyyy-MM-dd");
            }

            // Otherwise, return the raw string value
            return value;
        }
    }
}
