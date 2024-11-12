using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using datahold;
using System.IO;
using ClosedXML.Excel;

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
            using (XLWorkbook workbook = new XLWorkbook(filePath))
            using (SqlConnection connection = new SqlConnection(Config.GetConnectionString()))
            {
                connection.Open();

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    // Convert worksheet name to lowercase for compatibility
                    string tableName = worksheet.Name.ToLower();
                    var headers = worksheet.FirstRowUsed().Cells().Select(cell => cell.GetString()).ToList();

                    // Ensure table exists in the database
                    EnsureTableExists(tableName, headers, connection);

                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                    {
                        List<object> values = row.Cells(1, headers.Count).Select(cell => (object)cell.GetString()).ToList();
                        InsertRow(tableName, headers, values, connection);
                    }
                }
            }

            // Close the form once the import is complete :)
            this.Close();
        }

        private void EnsureTableExists(string tableName, List<string> columns, SqlConnection connection)
        {
            // Query to check if the table exists in the database
            string checkTableQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";

            using (SqlCommand checkCommand = new SqlCommand(checkTableQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@TableName", tableName);

                // If the table does not exist, create it
                if ((int)checkCommand.ExecuteScalar() == 0)
                {
                    StringBuilder createTableQuery = new StringBuilder($"CREATE TABLE [{tableName}] (");

                    // Define each column as NVARCHAR(MAX) by default for simplicity and compatability with wierd data
                    foreach (var column in columns)
                    {
                        createTableQuery.Append($"[{column}] NVARCHAR(MAX),");
                    }

                    createTableQuery.Length--;
                    createTableQuery.Append(");");

                    using (SqlCommand createCommand = new SqlCommand(createTableQuery.ToString(), connection))
                    {
                        createCommand.ExecuteNonQuery();
                        Console.WriteLine($"Table '{tableName}' created successfully.");
                    }
                }
            }
        }

        private void InsertRow(string tableName, List<string> columns, List<object> values, SqlConnection connection)
        {
            // Adjust length of values to match columns
            while (values.Count < columns.Count)
            {
                values.Add(DBNull.Value);  // Add null for missing values
            }

            var insertQuery = $@"
                INSERT INTO [{tableName}]
                ({string.Join(",", columns.Select(col => $"[{col}]"))}) 
                VALUES ({string.Join(",", Enumerable.Range(0, columns.Count).Select(i => $"@Value{i}"))})";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    command.Parameters.AddWithValue($"@Value{i}", values[i] ?? DBNull.Value);
                }

                command.ExecuteNonQuery();
            }
        }
    }
}

