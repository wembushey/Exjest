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
        // Keep track of the total number of rows and processed rows
        // This is for the loading bar
        private int rowsProcessed = 0;
        private int totalRows = 0;
             
        public ImportForm()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(ImportForm_DragEnter);
            this.DragDrop += new DragEventHandler(ImportForm_DragDrop);

            // Initialize ProgressBar (Hidden until process starts)
            progressBar1.Visible = false;
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

                // Calculate the total number of rows to import
                totalRows = workbook.Worksheets.Sum(ws => ws.RowsUsed().Count() - 1); // -1 to exclude headers
                progressBar1.Maximum = totalRows;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    string tableName = worksheet.Name.ToLower();
                    var headers = worksheet.FirstRowUsed().Cells().Select(cell => cell.GetString()).ToList();

                    EnsureTableExists(tableName, headers, connection);

                    int rowCount = 0;
                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))  // Skip header row
                    {
                        List<object> values = row.Cells(1, headers.Count).Select(cell => (object)cell.GetString()).ToList();
                        InsertRow(tableName, headers, values, connection);
                        rowCount++;

                        // Report progress after each row
                        rowsProcessed++;
                        progressBar1.Value = rowsProcessed;

                        // Optional: Use Application.DoEvents() to keep UI responsive during long operations
                        Application.DoEvents();
                    }
                }
            }

            // Show message once the import is complete
            MessageBox.Show("Import Complete!");
            this.Close();
        }

        private void EnsureTableExists(string tableName, List<string> columns, SqlConnection connection)
        {
            string checkTableQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";

            using (SqlCommand checkCommand = new SqlCommand(checkTableQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@TableName", tableName);

                if ((int)checkCommand.ExecuteScalar() == 0)
                {
                    StringBuilder createTableQuery = new StringBuilder($"CREATE TABLE [{tableName}] (");

                    foreach (var column in columns)
                    {
                        createTableQuery.Append($"[{column}] NVARCHAR(MAX),");
                    }

                    createTableQuery.Length--;
                    createTableQuery.Append(");");

                    using (SqlCommand createCommand = new SqlCommand(createTableQuery.ToString(), connection))
                    {
                        createCommand.ExecuteNonQuery();
                        Console.WriteLine($"Table '{tableName}' created successfully :) ");
                    }
                }
            }
        }

        private void InsertRow(string tableName, List<string> columns, List<object> values, SqlConnection connection)
        {
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
