using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using datahold;

namespace Excel_Import
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += ImportForm_DragEnter;
            this.DragDrop += ImportForm_DragDrop;
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
        }

        private void ImportExcelData(string filePath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            using (XLWorkbook workbook = new XLWorkbook(filePath))
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    DataTable data = GetDataTableFromWorksheet(worksheet);

                    if (worksheet.Name.Equals("CALLS", StringComparison.OrdinalIgnoreCase))
                    {
                        InsertDataIntoIncidentTable(data, connection);
                    }




                    /*
                    else if (worksheet.Name.Equals("MOBILE_DETAILS", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateIncidentTableFromMobileDetails(data, connection);
                    }
                    */



                    else if (worksheet.Name.Equals("COMPANIES", StringComparison.OrdinalIgnoreCase))
                    {
                        InsertDataIntoCompanyTable(data, connection);
                    }
                    else if (worksheet.Name.Equals("RAILROADS", StringComparison.OrdinalIgnoreCase))
                    {
                        InsertDataIntoRailroadTable(data, connection);
                    }
                    else if (worksheet.Name.Equals("INCIDENT_COMMONS", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateIncidentTableFromCommons(data, connection);
                    }
                    else if (worksheet.Name.Equals("INCIDENT_DETAILS", StringComparison.OrdinalIgnoreCase)) // New block
                    {
                        UpdateIncidentTableFromDetails(data, connection);
                    }
                    else if (worksheet.Name.Equals("TRAINS_DETAIL", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateRailroadAndIncidentTables(data, connection);
                        UpdateTrainAndIncidentTables(data, connection);
                        UpdateTrainCarAndIncidentTables(data, connection);
                    }







                    if (worksheet.Name.Equals("CALLS", StringComparison.OrdinalIgnoreCase))
                    {
                        InsertDataIntoIncidentTable(data, connection); // Existing function for incident data
                        UpdateCompanyAndIncidentTables(data, connection); // New function for company and incident linking
                    }




                }
            }

            // Close the form after processing
            this.Close();
        }




        private DataTable GetDataTableFromWorksheet(IXLWorksheet worksheet)
        {
            DataTable dataTable = new DataTable();

            var headers = worksheet.FirstRowUsed().Cells().Select(cell => cell.GetString().Trim()).ToList();
            foreach (var header in headers)
            {
                dataTable.Columns.Add(header);
            }

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var rowData = row.Cells(1, headers.Count).Select(cell => (object)cell.GetString().Trim()).ToArray();
                dataTable.Rows.Add(rowData);
            }

            return dataTable;
        }

        private void InsertDataIntoIncidentTable(DataTable data, SqlConnection connection)
        {
            // Enable IDENTITY_INSERT for the 'incident' table
            using (SqlCommand enableIdentityInsert = new SqlCommand("SET IDENTITY_INSERT incident ON;", connection))
            {
                enableIdentityInsert.ExecuteNonQuery();
            }

            foreach (DataRow row in data.Rows)
            {
                // Check if the record already exists
                string checkQuery = "SELECT COUNT(1) FROM incident WHERE seqnos = @seqnos";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(data, row, "SEQNOS"));
                    int recordExists = (int)checkCommand.ExecuteScalar();

                    // Skip the insert if the record already exists
                    if (recordExists > 0)
                    {
                        continue;
                    }
                }

                string insertQuery = @"
        INSERT INTO incident (seqnos, date_time_received, date_time_complete, call_type, 
                              responsible_city, responsible_state, responsible_zip, 
                              description_of_incident, type_of_incident, incident_cause, 
                              injury_count, hospitalization_count, fatality_count)
        VALUES (@seqnos, @date_time_received, @date_time_complete, @call_type, 
                @responsible_city, @responsible_state, @responsible_zip, 
                @description_of_incident, @type_of_incident, @incident_cause, 
                @injury_count, @hospitalization_count, @fatality_count);";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@seqnos", GetSafeValue(data, row, "SEQNOS"));
                    command.Parameters.AddWithValue("@date_time_received", ParseDateTime(GetSafeValue(data, row, "DATE_TIME_RECEIVED")));
                    command.Parameters.AddWithValue("@date_time_complete", ParseDateTime(GetSafeValue(data, row, "DATE_TIME_COMPLETE")));
                    command.Parameters.AddWithValue("@call_type", GetSafeValue(data, row, "CALLTYPE"));
                    command.Parameters.AddWithValue("@responsible_city", GetSafeValue(data, row, "RESPONSIBLE_CITY"));
                    command.Parameters.AddWithValue("@responsible_state", GetSafeValue(data, row, "RESPONSIBLE_STATE"));
                    command.Parameters.AddWithValue("@responsible_zip", GetSafeValue(data, row, "RESPONSIBLE_ZIP"));
                    command.Parameters.AddWithValue("@description_of_incident", GetSafeValue(data, row, "DESCRIPTION_OF_INCIDENT"));

                    object typeOfIncidentValue = GetSafeValue(data, row, "TYPE_OF_INCIDENT");
                    command.Parameters.AddWithValue("@type_of_incident", typeOfIncidentValue != DBNull.Value ? typeOfIncidentValue : "UNKNOWN");

                    object incidentCauseValue = GetSafeValue(data, row, "INCIDENT_CAUSE");
                    command.Parameters.AddWithValue("@incident_cause", incidentCauseValue != DBNull.Value ? incidentCauseValue : "UNSPECIFIED");

                    command.Parameters.AddWithValue("@injury_count", GetSafeValue(data, row, "NUMBER_INJURED"));
                    command.Parameters.AddWithValue("@hospitalization_count", GetSafeValue(data, row, "NUMBER_HOSPITALIZED"));
                    command.Parameters.AddWithValue("@fatality_count", DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }

            // Disable IDENTITY_INSERT for the 'incident' table
            using (SqlCommand disableIdentityInsert = new SqlCommand("SET IDENTITY_INSERT incident OFF;", connection))
            {
                disableIdentityInsert.ExecuteNonQuery();
            }
        }
        private void UpdateIncidentTableFromCommons(DataTable commonsData, SqlConnection connection)
        {
            foreach (DataRow row in commonsData.Rows)
            {
                // Fetch the existing record for the corresponding sequence number
                string fetchQuery = "SELECT * FROM incident WHERE seqnos = @seqnos";
                DataTable existingRecord = new DataTable();

                using (SqlCommand fetchCommand = new SqlCommand(fetchQuery, connection))
                {
                    fetchCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(commonsData, row, "SEQNOS"));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(fetchCommand))
                    {
                        adapter.Fill(existingRecord);
                    }
                }

                if (existingRecord.Rows.Count == 0)
                {
                    // No matching record, skip update
                    continue;
                }

                DataRow existingRow = existingRecord.Rows[0];

                // Prepare an update query that only updates fields where current database values are NULL
                string updateQuery = @"
            UPDATE incident
            SET 
                description_of_incident = CASE WHEN description_of_incident IS NULL THEN @description_of_incident ELSE description_of_incident END,
                type_of_incident = CASE WHEN type_of_incident IS NULL THEN @type_of_incident ELSE type_of_incident END,
                incident_cause = CASE WHEN incident_cause IS NULL THEN @incident_cause ELSE incident_cause END
            WHERE seqnos = @seqnos;";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@seqnos", GetSafeValue(commonsData, row, "SEQNOS"));
                    command.Parameters.AddWithValue("@description_of_incident", GetSafeValue(commonsData, row, "DESCRIPTION_OF_INCIDENT"));
                    command.Parameters.AddWithValue("@type_of_incident", GetSafeValue(commonsData, row, "TYPE_OF_INCIDENT"));
                    command.Parameters.AddWithValue("@incident_cause", GetSafeValue(commonsData, row, "INCIDENT_CAUSE"));

                    command.ExecuteNonQuery();
                }
            }
        }


        private void UpdateIncidentTableFromDetails(DataTable detailsData, SqlConnection connection)
        {
            foreach (DataRow row in detailsData.Rows)
            {
                // Fetch the existing record for the corresponding sequence number
                string fetchQuery = "SELECT * FROM incident WHERE seqnos = @seqnos";
                DataTable existingRecord = new DataTable();

                using (SqlCommand fetchCommand = new SqlCommand(fetchQuery, connection))
                {
                    fetchCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(detailsData, row, "SEQNOS"));
                    using (SqlDataAdapter adapter = new SqlDataAdapter(fetchCommand))
                    {
                        adapter.Fill(existingRecord);
                    }
                }

                if (existingRecord.Rows.Count == 0)
                {
                    // No matching record, skip update
                    continue;
                }

                DataRow existingRow = existingRecord.Rows[0];

                // Prepare an update query that only updates fields where current database values are NULL
                string updateQuery = @"
        UPDATE incident
        SET 
            injury_count = CASE WHEN injury_count IS NULL THEN @injury_count ELSE injury_count END,
            hospitalization_count = CASE WHEN hospitalization_count IS NULL THEN @hospitalization_count ELSE hospitalization_count END,
            fatality_count = CASE WHEN fatality_count IS NULL THEN @fatality_count ELSE fatality_count END
        WHERE seqnos = @seqnos;";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@seqnos", GetSafeValue(detailsData, row, "SEQNOS"));
                    command.Parameters.AddWithValue("@injury_count", GetSafeValue(detailsData, row, "NUMBER_INJURED"));
                    command.Parameters.AddWithValue("@hospitalization_count", GetSafeValue(detailsData, row, "NUMBER_HOSPITALIZED"));
                    command.Parameters.AddWithValue("@fatality_count", GetSafeValue(detailsData, row, "NUMBER_FATALITIES"));

                    command.ExecuteNonQuery();
                }
            }
        }


        /*
        private void UpdateIncidentTableFromMobileDetails(DataTable mobileDetailsData, SqlConnection connection)
        {
            foreach (DataRow row in mobileDetailsData.Rows)
            {
                // Check if SEQNOS exists in the incident table
                string fetchQuery = "SELECT incident_train_id FROM incident WHERE seqnos = @seqnos";
                object existingTrainId = null;

                using (SqlCommand fetchCommand = new SqlCommand(fetchQuery, connection))
                {
                    fetchCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(mobileDetailsData, row, "SEQNOS"));
                    existingTrainId = fetchCommand.ExecuteScalar();
                }

                // Skip updating if the incident_train_id already exists
                if (existingTrainId != null && existingTrainId != DBNull.Value)
                {
                    continue;
                }

                // Update incident_train_id with VEHICLE_NUMBER from MOBILE_DETAILS
                string updateQuery = @"
        UPDATE incident
        SET 
            incident_train_id = @incident_train_id
        WHERE seqnos = @seqnos;";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@seqnos", GetSafeValue(mobileDetailsData, row, "SEQNOS"));
                    command.Parameters.AddWithValue("@incident_train_id", GetSafeValue(mobileDetailsData, row, "VEHICLE_NUMBER"));
                    command.ExecuteNonQuery();
                }
            }
        }
        */




        private void UpdateTrainCarAndIncidentTables(DataTable derailedUnitsData, SqlConnection connection)
        {
            foreach (DataRow row in derailedUnitsData.Rows)
            {
                try
                {
                    // Step 1: Fetch the incident_train_id based on TRAIN_NAME_NUMBER and TRAIN_TYPE
                    string fetchTrainIdQuery = @"
            SELECT train_id 
            FROM incident_train 
            WHERE name_number = @name_number AND train_type = @train_type";

                    int incidentTrainId = 0;
                    object nameNumber = GetSafeValue(derailedUnitsData, row, "TRAIN_NAME_NUMBER") ?? "0";

                    using (SqlCommand fetchTrainIdCommand = new SqlCommand(fetchTrainIdQuery, connection))
                    {
                        fetchTrainIdCommand.Parameters.AddWithValue("@name_number", nameNumber);
                        fetchTrainIdCommand.Parameters.AddWithValue("@train_type", GetSafeValue(derailedUnitsData, row, "TRAIN_TYPE"));
                        object result = fetchTrainIdCommand.ExecuteScalar();
                        incidentTrainId = result != null ? Convert.ToInt32(result) : 0;
                    }

                    if (incidentTrainId == 0)
                    {
                        Console.WriteLine($"Skipping row: TRAIN_NAME_NUMBER={nameNumber}, no matching train_id found.");
                        continue; // Skip if no matching train_id
                    }

                    // Step 2: Insert into incident_train_car
                    string insertTrainCarQuery = @"
            INSERT INTO incident_train_car (car_number, car_content, position_in_train, car_type, incident_train_id)
            VALUES (@car_number, @car_content, @position_in_train, @car_type, @incident_train_id)";

                    using (SqlCommand insertTrainCarCommand = new SqlCommand(insertTrainCarQuery, connection))
                    {
                        var carNumber = GetSafeValue(derailedUnitsData, row, "CAR_NUMBER");
                        var carContent = GetSafeValue(derailedUnitsData, row, "CAR_CONTENT");
                        var positionInTrain = GetSafeValue(derailedUnitsData, row, "POSITION_IN_TRAIN");
                        var carType = GetSafeValue(derailedUnitsData, row, "DERAILED_TYPE");

                        // Ensure no nulls for NOT NULL columns
                        if (carNumber == DBNull.Value || carContent == DBNull.Value || carType == DBNull.Value)
                        {
                            Console.WriteLine($"Skipping row due to null values: CAR_NUMBER={carNumber}, CAR_CONTENT={carContent}, CAR_TYPE={carType}");
                            continue;
                        }

                        // Set default values for nullable columns if needed
                        positionInTrain = positionInTrain == DBNull.Value ? 0 : positionInTrain;

                        // Log values being inserted
                        Console.WriteLine($"Inserting: CAR_NUMBER={carNumber}, CAR_CONTENT={carContent}, POSITION_IN_TRAIN={positionInTrain}, CAR_TYPE={carType}, INCIDENT_TRAIN_ID={incidentTrainId}");

                        insertTrainCarCommand.Parameters.AddWithValue("@car_number", carNumber);
                        insertTrainCarCommand.Parameters.AddWithValue("@car_content", carContent);
                        insertTrainCarCommand.Parameters.AddWithValue("@position_in_train", positionInTrain);
                        insertTrainCarCommand.Parameters.AddWithValue("@car_type", carType);
                        insertTrainCarCommand.Parameters.AddWithValue("@incident_train_id", incidentTrainId);

                        insertTrainCarCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row: {row["CAR_NUMBER"]}. Exception: {ex.Message}");
                }
            }
        }













        private void UpdateTrainAndIncidentTables(DataTable trainsDetailData, SqlConnection connection)
        {
            foreach (DataRow row in trainsDetailData.Rows)
            {
                // Step 1: Retrieve the railroad_id based on the RAILROAD_NAME column
                string fetchRailroadIdQuery = @"
            SELECT railroad_id 
            FROM railroad 
            WHERE railroad_name = @railroad_name";

                int railroadId = 0;
                using (SqlCommand fetchRailroadIdCommand = new SqlCommand(fetchRailroadIdQuery, connection))
                {
                    fetchRailroadIdCommand.Parameters.AddWithValue("@railroad_name", GetSafeValue(trainsDetailData, row, "RAILROAD_NAME"));
                    object result = fetchRailroadIdCommand.ExecuteScalar();
                    railroadId = result != null ? Convert.ToInt32(result) : 0;
                }

                if (railroadId == 0)
                {
                    // Skip this row if no matching railroad_id is found
                    continue;
                }

                // Step 2: Insert into the incident_train table if it doesn't already exist
                string insertTrainQuery = @"
            IF NOT EXISTS (
                SELECT 1 
                FROM incident_train 
                WHERE name_number = @name_number 
                  AND train_type = @train_type 
                  AND railroad_id = @railroad_id
            )
            BEGIN
                INSERT INTO incident_train (name_number, train_type, railroad_id) 
                OUTPUT INSERTED.train_id 
                VALUES (@name_number, @train_type, @railroad_id)
            END
            ELSE
            BEGIN
                SELECT train_id 
                FROM incident_train 
                WHERE name_number = @name_number 
                  AND train_type = @train_type 
                  AND railroad_id = @railroad_id
            END";

                int trainId = 0;

                // Handle null or empty TRAIN_NAME_NUMBER
                object nameNumber = GetSafeValue(trainsDetailData, row, "TRAIN_NAME_NUMBER");
                if (nameNumber == DBNull.Value || string.IsNullOrWhiteSpace(nameNumber.ToString()))
                {
                    nameNumber = 0; // Default to 0
                }

                using (SqlCommand insertTrainCommand = new SqlCommand(insertTrainQuery, connection))
                {
                    insertTrainCommand.Parameters.AddWithValue("@name_number", nameNumber);
                    insertTrainCommand.Parameters.AddWithValue("@train_type", GetSafeValue(trainsDetailData, row, "TRAIN_TYPE"));
                    insertTrainCommand.Parameters.AddWithValue("@railroad_id", railroadId);

                    object result = insertTrainCommand.ExecuteScalar();
                    trainId = result != null ? Convert.ToInt32(result) : 0;
                }

                // Step 3: Update the incident table with the train_id
                if (trainId > 0)
                {
                    string updateIncidentQuery = @"
                UPDATE incident
                SET incident_train_id = @train_id
                WHERE seqnos = @seqnos";

                    using (SqlCommand updateIncidentCommand = new SqlCommand(updateIncidentQuery, connection))
                    {
                        updateIncidentCommand.Parameters.AddWithValue("@train_id", trainId);
                        updateIncidentCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(trainsDetailData, row, "SEQNOS"));

                        updateIncidentCommand.ExecuteNonQuery();
                    }
                }
            }
        }


























        private void UpdateCompanyAndIncidentTables(DataTable callsData, SqlConnection connection)
        {
            foreach (DataRow row in callsData.Rows)
            {
                // Insert into the company table if it doesn't already exist
                string insertCompanyQuery = @"
        IF NOT EXISTS (SELECT 1 FROM company WHERE company_name = @company_name AND org_type = @org_type)
        BEGIN
            INSERT INTO company (company_name, org_type) OUTPUT INSERTED.company_id 
            VALUES (@company_name, @org_type)
        END
        ELSE
        BEGIN
            SELECT company_id FROM company WHERE company_name = @company_name AND org_type = @org_type
        END";

                int companyId;

                using (SqlCommand insertCompanyCommand = new SqlCommand(insertCompanyQuery, connection))
                {
                    insertCompanyCommand.Parameters.AddWithValue("@company_name", GetSafeValue(callsData, row, "RESPONSIBLE_COMPANY"));
                    insertCompanyCommand.Parameters.AddWithValue("@org_type", GetSafeValue(callsData, row, "RESPONSIBLE_ORG_TYPE"));

                    object result = insertCompanyCommand.ExecuteScalar();
                    companyId = result != null ? Convert.ToInt32(result) : 0;
                }

                // Update the incident table with the company_id
                string updateIncidentQuery = @"
        UPDATE incident
        SET company_id = @company_id
        WHERE seqnos = @seqnos";

                using (SqlCommand updateIncidentCommand = new SqlCommand(updateIncidentQuery, connection))
                {
                    updateIncidentCommand.Parameters.AddWithValue("@company_id", companyId);
                    updateIncidentCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(callsData, row, "SEQNOS"));

                    updateIncidentCommand.ExecuteNonQuery();
                }
            }
        }







        private void UpdateRailroadAndIncidentTables(DataTable trainsDetailData, SqlConnection connection)
        {
            foreach (DataRow row in trainsDetailData.Rows)
            {
                // Insert into the railroad table if it doesn't already exist
                string insertRailroadQuery = @"
        IF NOT EXISTS (SELECT 1 FROM railroad WHERE railroad_name = @railroad_name)
        BEGIN
            INSERT INTO railroad (railroad_name) OUTPUT INSERTED.railroad_id VALUES (@railroad_name)
        END
        ELSE
        BEGIN
            SELECT railroad_id FROM railroad WHERE railroad_name = @railroad_name
        END";

                int railroadId;

                using (SqlCommand insertRailroadCommand = new SqlCommand(insertRailroadQuery, connection))
                {
                    insertRailroadCommand.Parameters.AddWithValue("@railroad_name", GetSafeValue(trainsDetailData, row, "RAILROAD_NAME"));

                    object result = insertRailroadCommand.ExecuteScalar();
                    railroadId = result != null ? Convert.ToInt32(result) : 0;
                }

                // Update the incident table with the railroad_id
                string updateIncidentQuery = @"
        UPDATE incident
        SET railroad_id = @railroad_id
        WHERE seqnos = @seqnos";

                using (SqlCommand updateIncidentCommand = new SqlCommand(updateIncidentQuery, connection))
                {
                    updateIncidentCommand.Parameters.AddWithValue("@railroad_id", railroadId);
                    updateIncidentCommand.Parameters.AddWithValue("@seqnos", GetSafeValue(trainsDetailData, row, "SEQNOS"));

                    updateIncidentCommand.ExecuteNonQuery();
                }
            }
        }










































        private object ParseDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return DBNull.Value; // Use NULL in the database if the value is missing
            }

            DateTime parsedDate;
            // Attempt to parse the value into a DateTime object
            if (DateTime.TryParse(value.ToString(), out parsedDate))
            {
                return parsedDate;
            }

            // Handle invalid date values by setting them to NULL or a default date
            return DBNull.Value; // Use a default value like DateTime.Now if required
        }





        private void InsertDataIntoCompanyTable(DataTable data, SqlConnection connection)
        {
            foreach (DataRow row in data.Rows)
            {
                string query = @"
                INSERT INTO company (company_name, org_type)
                VALUES (@company_name, @org_type);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@company_name", GetSafeValue(data, row, "RESPONSIBLE_COMPANY"));
                    command.Parameters.AddWithValue("@org_type", GetSafeValue(data, row, "RESPONSIBLE_ORG_TYPE"));

                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertDataIntoRailroadTable(DataTable data, SqlConnection connection)
        {
            foreach (DataRow row in data.Rows)
            {
                string query = @"
                INSERT INTO railroad (railroad_name)
                VALUES (@railroad_name);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@railroad_name", GetSafeValue(data, row, "RAILROAD_NAME"));

                    command.ExecuteNonQuery();
                }
            }
        }

        private object GetSafeValue(DataTable data, DataRow row, string columnName)
        {
            return data.Columns.Contains(columnName) && row[columnName] != DBNull.Value ? row[columnName] : DBNull.Value;
        }
    }
}
