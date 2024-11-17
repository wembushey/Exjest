using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Configuration;

namespace Excel_Import
{
    public partial class ImportForm : Form
    {
        private int rowsProcessed = 0;
        private int totalRows = 0;

        public ImportForm()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += ImportForm_DragEnter;
            this.DragDrop += ImportForm_DragDrop;
            progressBar1.Visible = false;
        }

        private void ImportForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = files.Any(file => Path.GetExtension(file).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
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

            // Close the form after processing all files
            this.Close();
        }

        private void ImportExcelData(string filePath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string is missing or invalid in app.config.");
                return;
            }

            using (XLWorkbook workbook = new XLWorkbook(filePath))
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                totalRows = workbook.Worksheets.Sum(ws => ws.RowsUsed().Count() - 1); // Exclude header rows
                progressBar1.Maximum = totalRows;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    string tableName = worksheet.Name.ToLower();
                    var headers = worksheet.FirstRowUsed().Cells().Select(cell => cell.GetString()).ToList();

                    // Skip this worksheet if the table already exists
                    if (TableExists(tableName, connection))
                    {
                        Console.WriteLine($"Skipping worksheet '{tableName}' as the table already exists.");
                        continue;
                    }

                    CreateTable(tableName, headers, connection);

                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1)) // Skip header row
                    {
                        var values = row.Cells(1, headers.Count).Select(cell => (object)cell.GetString()).ToList();
                        InsertRow(tableName, headers, values, connection);

                        rowsProcessed++;
                        progressBar1.Value = rowsProcessed;
                        Application.DoEvents();
                    }
                }
            }
        }

        private bool TableExists(string tableName, SqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        private void CreateTable(string tableName, List<string> columns, SqlConnection connection)
        {
            StringBuilder query = new StringBuilder($"CREATE TABLE [{tableName}] (");

            foreach (string column in columns)
            {
                query.Append($"[{column}] NVARCHAR(MAX),");
            }

            query.Length--; // Remove trailing comma
            query.Append(");");

            using (SqlCommand command = new SqlCommand(query.ToString(), connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine($"Table '{tableName}' created successfully.");
            }
        }

        private void InsertRow(string tableName, List<string> columns, List<object> values, SqlConnection connection)
        {
            // Ensure all columns have corresponding values
            while (values.Count < columns.Count)
            {
                values.Add(DBNull.Value);
            }

            string query = $@"
                INSERT INTO [{tableName}]
                ({string.Join(",", columns.Select(col => $"[{col}]"))}) 
                VALUES ({string.Join(",", columns.Select((_, i) => $"@Value{i}"))})";

            using (SqlCommand command = new SqlCommand(query, connection))
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
