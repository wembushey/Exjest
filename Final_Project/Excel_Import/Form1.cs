using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Excel_Import
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Enable drag-and-drop on the form
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        // Event handler that does a file type check
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
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

        // Event handler for DragDrop to handle the dropped file
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                string filePath = files[0];
                ImportExcelData(filePath);
            }
        }

        // Method to import data from the Excel file
        private void ImportExcelData(string filePath)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Iterate through rows and cells to read values
                foreach (Row row in sheetData.Elements<Row>())
                {
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        string cellValue = GetCellValue(cell, workbookPart);
                        // Process or display the cell value as needed
                        Console.WriteLine(cellValue); // Example: output to console
                    }
                }
            }
        }

        // Helper method to get the cell value as text
        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            string value = cell.CellValue?.Text;
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                return workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
            }
            return value;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
