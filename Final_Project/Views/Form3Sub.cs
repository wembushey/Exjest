using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Windows.Forms;

namespace Views
{
    public partial class Form3Sub : Form
    {
        public Form3Sub(DataRow incidentRow)
        {
            InitializeComponent();
            PopulateGrid(incidentRow);
        }

        private void PopulateGrid(DataRow row)
        {
            // Create a DataTable to hold the single row
            DataTable table = new DataTable();

            // Add columns dynamically based on the row's columns
            foreach (DataColumn column in row.Table.Columns)
            {
                table.Columns.Add(column.ColumnName, column.DataType);
            }

            // Add the row to the new DataTable
            table.Rows.Add(row.ItemArray);

            // Set the DataTable as the DataSource for the DataGridView
            dataGridView1.DataSource = table;

            // Adjust column widths for better readability
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
