// -----------------------------------------------------------------------
// <copyright file="Dispatcher.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClosedXML.Excel;
    using EvaluationEngineConsole.DataModels;
    using EvaluationEngineConsole.ExpectedFields;
    using EvaluationEngineConsole.Validation;
    using NLog;

    /// <summary>
    /// The most important class in the project.  Handles the user request by generating a feedback report, and when appropriate, a file containing StudyIds.
    /// </summary>
    public class Dispatcher
    {
        /// <summary>
        /// A logger to help with debugging.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The object that communicates with the database.
        /// </summary>
        private readonly IDataProvider data;

        /// <summary>
        /// The id of the dispatcher. Will help to make logs more informative.
        /// </summary>
        private readonly Guid dispatchId;

        /// <summary>
        /// The options passed through the command line.
        /// </summary>
        private Options options;

        /// <summary>
        /// The name of an intermediary file; contents of this file get uploaded to a table with the state's name.
        /// </summary>
        private string tempFile;

        /// <summary>
        /// The file name for the csv report.
        /// </summary>
        private string reportCsvFile;

        /// <summary>
        /// The index of the field state student id in the headers file.
        /// </summary>
        private int indexOfStateId;

        /// <summary>
        /// A list containing all the indices of the fields that the user wants hashed.
        /// </summary>
        private List<int> indexOfFieldsToHash = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher" /> class.
        /// </summary>
        /// <param name="options">An Options object.</param>
        /// <param name="data">An object implementing the IDateProvider interface.</param>
        public Dispatcher(Options options, IDataProvider data)
        {
            this.dispatchId = Guid.NewGuid();

            logger.Info("===================================================================");
            logger.Info("=== SETTINGS of DispatchId={0} ===", this.dispatchId);
            logger.Info("===================================================================");

            this.options = options;
            this.data = data;

            this.UpdateOutputFilePathExtension();

            var date = DateTime.Now.ToString().Replace(" ", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty);
            this.tempFile = options.State + date + ".csv";
            this.reportCsvFile = "report_" + this.tempFile;

            var headers = this.GetTargetFileHeaders();
            if (headers != null && headers.Contains(this.options.StateIdField))
            {
                this.SetIndexOfFieldsToHash(headers, this.options.FieldsToHash);
            }

            logger.Info("Options for DispathId={0}, t={1}, o={2}, w={3}, s={4}, i={5}, e={6}, h={7}", this.dispatchId, this.options.TargetFile, this.options.OutputFile, this.options.TimeApplication, this.options.State, this.options.StateIdField, this.options.ValidationFile, string.Join(":", this.options.FieldsToHash));
        }

        /// <summary>
        /// Ensures that the output file and validation file bothh have the right extensions.
        /// </summary>
        public void UpdateOutputFilePathExtension()
        {
            if (!Path.GetExtension(this.options.ValidationFile).Equals(".csv"))
            {
                this.options.OutputFile = this.options.ValidationFile + ".csv";
            }

            if (!string.IsNullOrEmpty(this.options.OutputFile))
            {
                if (!Path.GetExtension(this.options.OutputFile).Equals(".csv"))
                {
                    this.options.OutputFile = this.options.OutputFile + ".csv";
                }
            }
        }

        /// <summary>
        /// In charge of uploading data to the Crosswalk database and producing a file for the Stata server.
        /// </summary>
        public void Dispatch()
        {
            logger.Info("================================================================");
            logger.Info("=== START of DispatchId={0} ===", this.dispatchId);
            logger.Info("================================================================");

            Stopwatch watch = new Stopwatch();
            watch.Start();
           
            var headers = this.GetTargetFileHeaders();

            if (headers != null && headers.Contains(this.options.StateIdField))
            {
                logger.Info("Dispatch={0} about to start validating input file", this.dispatchId);
                this.GenerateValidationReport();
                if (!string.IsNullOrEmpty(this.options.OutputFile))
                {
                    logger.Info("Dispatch={0} about to start generating output file", this.dispatchId);
                    headers = this.GenerateTempFileAndGetHeaders();
                    this.MoveToXWalkDb(headers);
                }
            }
            else
            {
                logger.Error("DispatchId={0}. The state student column i={1} is not in the file", this.dispatchId, this.options.StateIdField);
            }

            watch.Stop();
            logger.Info("=== END of DispatchId={0}. It took={1} to handle dispatch. ===", this.dispatchId, watch.Elapsed);

            if (this.options.TimeApplication)
            {
                Console.WriteLine("Time elapsed {0}", watch.Elapsed);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Sets the index for the state student id.
        /// </summary>
        /// <param name="headers">The headers in the file originally uploaded by the user.</param>
        public void SetIndexOfStateId(string[] headers)
        {
            var headersUpper = headers.Select(x => x.ToUpper()).ToArray();
            this.indexOfStateId = Array.IndexOf(headersUpper, this.options.StateIdField.ToUpper());
        }

        /// <summary>
        /// Sets the list of indices tracking which fields to hash.
        /// </summary>
        /// <param name="headers">The headers in the input file submitted by the user.</param>
        /// <param name="namesOfFieldsToHash">The names of fields to hashed as passed through the command line.</param>
        public void SetIndexOfFieldsToHash(string[] headers, IList<string> namesOfFieldsToHash)
        {
            this.SetIndexOfStateId(headers);

            this.indexOfFieldsToHash.Add(this.indexOfStateId);

            var headersUpper = headers.Select(x => x.ToUpper()).ToArray();
            foreach (var field in namesOfFieldsToHash)
            {
                var index = Array.IndexOf(headersUpper, field.ToUpper());
                if (index > -1)
                {
                    this.indexOfFieldsToHash.Add(index);
                }
            }
        }

        /// <summary>
        /// Generates a validation report for the data file submitted by the user.
        /// </summary>
        public void GenerateValidationReport()
        {
            this.GenerateCsvValidationReport();
            this.GenerateFullCsvValidationReport();

            // Clean up.
            FileHandler.DeleteFile(this.reportCsvFile);
        }

        /// <summary>
        /// Gets the headers in the file uploaded by the user.
        /// </summary>
        /// <returns>An array of headers.</returns>
        public string[] GetTargetFileHeaders()
        {
            var records = FileHandler.Read(this.options.TargetFile);

            if (records != null)
            {
                logger.Info("File={0} had headers={1}", this.options.TargetFile, string.Join(":", records.First()));
                return records.First();
            }

            logger.Info("File={0} had no headers or content", this.options.TargetFile);
            return null;
        }

        /// <summary>
        /// Gets the number of records in a file.  Skips the first row of the file because we are assuming that's the header.
        /// </summary>
        /// <returns>The number of records in the a file.</returns>
        public int GetNumberOfRecordsInTargetFile()
        {
            var records = FileHandler.Read(this.options.TargetFile);
            return records.Skip(1).Count();
        }

        /// <summary>
        /// Generates a csv file with all records that failed one or more of the validation rules.
        /// </summary>
        public void GenerateCsvValidationReport()
        {
            var records = FileHandler.Read(this.options.TargetFile);

            var headers = records.First();

            var rules = this.GetValidationRules(headers);

            var newrecords = records.Skip(1).Where(x => !this.IsRowValid(rules, x))
                                    .Select(x => 
                                    {
                                        foreach (var index in this.indexOfFieldsToHash)
                                        {
                                            x[index] = "redacted";
                                        }

                                        return x; 
                                    });

            FileHandler.Write(this.reportCsvFile, newrecords, headers);
        }

        /// <summary>
        /// Generates two feedback reports: 
        /// 1] A report containing all the records that contained invalid values.  Only invalid values are shown to the user.
        /// 2] A report with statistics: for each field, a number of invalid records is presented. Listed also are fields not validated
        /// either because they user misspelled it, or because we don't have a validation rule for it.  Finally the total number of 
        /// fields with error is displayed.
        /// </summary>
        public void GenerateFullCsvValidationReport()
        {
            var records = FileHandler.Read(this.reportCsvFile);
            var headers = records.First();
            var rules = this.GetValidationRules(headers);

            Dictionary<int, int> errorCounter = this.GetErrorCounter(headers, 0);
            var newrecords = records.Skip(1).Select(x =>
                                    {
                                        var indices = this.GetIndexOfInvalidCells(rules, x);
                                        var counter = 0;
                                        foreach (var cell in x)
                                        {
                                            if (!indices.Contains(counter))
                                            {
                                                x[counter] = string.Empty;
                                            }
                                            else
                                            {
                                                errorCounter[counter]++;
                                            }

                                            counter++;
                                        }

                                        return x;
                                    });

            FileHandler.Write(this.options.ValidationFile, newrecords, headers);

            this.GenerateCsvErrorStatistics(headers, errorCounter, rules.Keys);
        }

        /// <summary>
        /// Generates a csv file containing the number of invalid records for each field. Listed also are fields not validated
        /// either because they user misspelled it, or because we don't have a validation rule for it.  Finally the total number of 
        /// fields with error is displayed.
        /// </summary>
        /// <param name="headers">The headers in the input file.</param>
        /// <param name="errorCounter">A dictionary where the key is the index of the field, and the value is the number of errors for that field.</param>
        /// <param name="fieldsValidated">The indices for the fields we validate.</param>
        public void GenerateCsvErrorStatistics(string[] headers, Dictionary<int, int> errorCounter, IEnumerable<int> fieldsValidated)
        {
            var records = new List<string[]>();
            records.Add(this.GenerateTextRowForFeedbackReport("DATA VALIDATION STATISTICS FOR EVALUATION ENGINE DATA SUBMISSION", headers.Length));
            records.Add(new string[headers.Length]);

            records.Add(this.GenerateTextRowForFeedbackReport("Number of invalid values for each field:", headers.Length));
            records.Add(headers);
            records.Add(errorCounter.Values.Select(x => x.ToString()).ToArray());
            records.Add(new string[headers.Length]);

            records.Add(this.GenerateTextRowForFeedbackReport("Total number of records processed:", headers.Length));
            records.Add(this.GenerateTextRowForFeedbackReport(this.GetNumberOfRecordsInTargetFile().ToString(), headers.Length, 1));
            records.Add(new string[headers.Length]);

            records.Add(this.GenerateTextRowForFeedbackReport("Total number of records that contain one or more invalid fields:", headers.Length));
            records.Add(this.GenerateTextRowForFeedbackReport(errorCounter.Values.Max().ToString(), headers.Length, 1));
            records.Add(new string[headers.Length]);

            records.Add(this.GenerateTextRowForFeedbackReport("Expected fields not received:", headers.Length));
            var expectedFields = EEFields.GetSetOfExpectedFields();
            var optionalFields = EEFields.GetSetOfOptionalFields();
            foreach (var field in expectedFields.Except(headers).Except(optionalFields))
            {
                records.Add(this.GenerateTextRowForFeedbackReport(field, headers.Length, 1));
            }

            records.Add(new string[headers.Length]);
            records.Add(this.GenerateTextRowForFeedbackReport("Unexpected fields received:", headers.Length));
            foreach (var field in headers.Except(expectedFields))
            {
                records.Add(this.GenerateTextRowForFeedbackReport(field, headers.Length, 1));
            }

            var filename = Path.GetFileNameWithoutExtension(this.options.ValidationFile);
            filename = this.options.ValidationFile.Replace(filename, filename + "_statistics");
            FileHandler.Write(filename, records);
        }

        /// <summary>
        /// A helper method that outputs a string array containing text to help users understand the feedback reports.
        /// </summary>
        /// <param name="text">The text that should be displayed to the user.</param>
        /// <param name="numColumns">The number of columns the feedback report has.</param>
        /// <param name="startAtCol">The column where we should start displaying the text.</param>
        /// <returns>A string array containing text to be displayed to the user.</returns>
        public string[] GenerateTextRowForFeedbackReport(string text, int numColumns, int startAtCol = 0 )
        {
            var outputRow = new string[numColumns];
            outputRow[startAtCol] = text;
            return outputRow;
        }

        /// <summary>
        /// Generates an Excel report with all records that failed validation rules.
        /// Invalid cells are highlighted in baby blue.
        /// </summary>
        public void GenerateExcelValidationReport()
        {
            var records = FileHandler.Read(this.reportCsvFile);
            var headers = records.First();
            var rules = this.GetValidationRules(headers);

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");

            Dictionary<int, int> errorCounter = this.GetErrorCounter(headers);
            var rownumber = 1;
            foreach (var record in records)
            {
                IEnumerable<int> indices = new List<int>();

                // No need to validate the headers.
                if (rownumber != 1)
                {
                    // we need to offset by one because rows and cells in ClosedXML start at 1
                    indices = this.GetIndexOfInvalidCells(rules, record).Select(x => x + 1);
                    indices.ForEach(x => errorCounter[x]++);
                }

                var cellnumber = 1;
                foreach (var cell in record)
                {
                    ws.Row(rownumber).Cell(cellnumber).SetValue(cell);
                    if (indices.Contains(cellnumber))
                    {
                        ws.Row(rownumber).Cell(cellnumber).Style.Fill.BackgroundColor = XLColor.BabyBlue;
                    }

                    cellnumber++;
                }

                rownumber++;
            }

            this.GenerateExcelErrorStatistics(wb, headers, errorCounter, rules.Keys);
            wb.SaveAs(this.options.ValidationFile);
        }

        /// <summary>
        /// Generates an dictionary containing as many entries as there are elements in the headers array.
        /// The keys are number starting from the initKey; all values are zero.
        /// </summary>
        /// <param name="headers">A string array with file headers.</param>
        /// <param name="initKey">The starting key for the dictionary.</param>
        /// <returns>A dictionary with as many entries as there are elements in the headers array. Keys: from initKey on. Values: zero</returns>
        public Dictionary<int, int> GetErrorCounter(string[] headers, int initKey = 1)
        {
            var dictionary = new Dictionary<int, int>();
            int counter = initKey;
            foreach (var header in headers)
            {
                dictionary.Add(counter, 0);
                counter++;
            }

            return dictionary;
        }

        /// <summary>
        /// Populates a spreadsheet in the passed workbook containing statistics about the error found in the input file.
        /// </summary>
        /// <param name="workbook">The workbook where we are putting the worksheet.</param>
        /// <param name="headers">The file headers.</param>
        /// <param name="errorCounter">A dictionary containing a count of the errors found.</param>
        /// <param name="fieldsValidated">The indices of the fields validated.</param>
        public void GenerateExcelErrorStatistics(XLWorkbook workbook, string[] headers, Dictionary<int, int> errorCounter, IEnumerable<int> fieldsValidated)
        {
            var ws = workbook.Worksheets.Add("Statistics");

            fieldsValidated = fieldsValidated.Select(x => x + 1);
            var fieldNotValidated = new List<string>();
            int totalnumber = 0;
            int cellnumber = 1;
            foreach (var header in headers)
            {
                ws.Row(1).Cell(cellnumber).SetValue(header);
                ws.Row(2).Cell(cellnumber).SetValue(errorCounter[cellnumber]);

                if (errorCounter[cellnumber] > totalnumber)
                {
                    totalnumber = errorCounter[cellnumber];
                }

                if (!fieldsValidated.Contains(cellnumber))
                {
                    fieldNotValidated.Add(header);
                }

                cellnumber++;
            }

            ws.Row(3).Cell(1).SetValue("Total number of records that contain one or more invalid fields:");
            ws.Row(3).Cell(1).Style.Alignment.SetWrapText();
            ws.Row(3).Cell(2).SetValue(totalnumber);

            ws.Row(5).Cell(1).SetValue("Fields that were not validated:");
            ws.Row(5).Cell(1).Style.Alignment.SetWrapText();
            int rownumber = 5;
            foreach (var field in fieldNotValidated)
            {
                ws.Row(rownumber).Cell(2).SetValue(field);
                rownumber++;
            }
        }

        /// <summary>
        /// Determines whether a row fails any of the validation rules.
        /// </summary>
        /// <param name="rules">A dictionary where the key is the index of the cell to validate, and the value is the rule to use.</param>
        /// <param name="dataRow">The row we want to validate.</param>
        /// <returns>True if all cells pass the rules; false otherwise.</returns>
        public bool IsRowValid(Dictionary<int, Predicate<string>> rules, string[] dataRow)
        {
            foreach (var fieldIndex in rules.Keys)
            {
                if (!rules[fieldIndex](dataRow[fieldIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Produces a list with the indices of cells failing validation rules.
        /// </summary>
        /// <param name="rules">A dictionary where the key is the index of the cell to validate, and the value is the rule to use.</param>
        /// <param name="dataRow">The row we want to validate.</param>
        /// <returns>A list with the indices of cells failing validation rules.</returns>
        public List<int> GetIndexOfInvalidCells(Dictionary<int, Predicate<string>> rules, string[] dataRow)
        {
            List<int> cellsToHighlight = new List<int>(); 
            foreach (var fieldIndex in rules.Keys)
            {
                if (!rules[fieldIndex](dataRow[fieldIndex]))
                {
                    cellsToHighlight.Add(fieldIndex);
                }
            }

            return cellsToHighlight;
        }

        /// <summary>
        /// Gets the validation rules for a set of fields.
        /// </summary>
        /// <param name="headers">The fields we want to retrieve validation rules for.</param>
        /// <returns>A dictionary where the key is the index of the cell to validate, and the value is the rule to use.</returns>
        public Dictionary<int, Predicate<string>> GetValidationRules(string[] headers)
        {
            var validation = new EEValidationRules();
            return validation.GetValidationRules(headers);
        }

        /// <summary>
        /// Generates the temporary file that gets uploaded to a table with the state's name.
        /// </summary>
        /// <returns>An array headers containing the fields added (e.g., HashedId, StudyId, etc).</returns>
        public string[] GenerateTempFileAndGetHeaders()
        {
            var records = FileHandler.Read(this.options.TargetFile);
            var dataModel = new EEDataModel();
            var newRecords = records.Skip(1).Select(x => dataModel.Concat(x, this.indexOfFieldsToHash, this.indexOfStateId)); 
            FileHandler.Write(this.tempFile, newRecords);

            var headers = this.SetNameNewFieldsOutputCsv(dataModel.Concat(records.First(), new int[] { }, this.indexOfStateId));
            return headers;
        }

        /// <summary>
        /// Set the correct name for new fields added to the data originally uploaded by the user.
        /// </summary>
        /// <param name="headers">The list of original headers.</param>
        /// <returns>An updated list of headers with correct column names.</returns>
        public string[] SetNameNewFieldsOutputCsv(string[] headers)
        {
            headers[0] = "HashedId";
            headers[1] = "StudyId";
            headers[2] = "CipherText";
            headers[3] = "Vector";
            return headers;
        }

        /// <summary>
        /// Sanitizes the name of fields as the appear in the header of the input file.
        /// Field name can only contain letters, numbers and underscores.
        /// </summary>
        /// <param name="headers">An array containing the fields to sanitize.</param>
        /// <returns>An array the values of which have been sanitized.</returns>
        public string[] SanitizeColumns(string[] headers)
        {
            var sanitizedArray = new string[headers.Length];
            var approvedList = EEFields.GetSetOfExpectedFields(); 
            for (int i = 0; i < headers.Length; i++)
            {
                if (approvedList.Contains(headers[i].ToUpper()))
                {
                    sanitizedArray[i] = headers[i];
                }
                else
                {
                    sanitizedArray[i] = this.SanitizeString(headers[i]);
                }
            }

            return sanitizedArray;
        }

        /// <summary>
        /// Sanitizes a string by removing any character that is not a letter, number or underscore.
        /// </summary>
        /// <param name="input">The string we want to sanitize.</param>
        /// <returns>The sanitized string.</returns>
        public string SanitizeString(string input)
        {
            string pattern = @"\W+";
            string replacement = string.Empty;
            return Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// A list containing the names of the fields that should not be included in the output file.
        /// </summary>
        /// <returns>A list of fields.</returns>
        public IList<string> GetFieldsToOmitFromOutput()
        {
            var list = new List<string> { "CipherText", "Vector" };
            list.Add(this.options.StateIdField);

            return list;
        }

        /// <summary>
        /// Moves data to the database and produces output file for Stata file.
        /// </summary>
        /// <param name="columns">The headers that exist in the temporary file.</param>
        public void MoveToXWalkDb(string[] columns)
        {
            var tableName = this.options.State;

            this.data.DropUploadDataTable(tableName);
            this.data.CreateUploadDataTable(tableName, this.SanitizeColumns(columns));

            if (File.Exists(this.tempFile))
            {
                this.data.UploadOutputFile(tableName, Path.GetFullPath(this.tempFile));
                FileHandler.DeleteFile(this.tempFile);

                this.data.UpdateDuplicatesStudyIds(tableName);
                this.data.UpdateStudyIdsInUploadFile(tableName);
                this.data.InsertRecordsInCrosswalk(tableName);

                var headers = columns.Except(this.GetFieldsToOmitFromOutput()).ToArray();
                this.data.OutputFileForStata(this.options.OutputFile, tableName, headers);
            }
            else
            {
                logger.Error("DispatchId={0}. The temporary File={1} was not generated", this.dispatchId, this.tempFile);
            }
        }
    }
}