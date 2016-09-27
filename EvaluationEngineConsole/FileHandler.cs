// -----------------------------------------------------------------------
// <copyright file="FileHandler.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using NLog;

    /// <summary>
    /// A class that manages the reading and writing to files.
    /// </summary>
    public class FileHandler
    {
        /// <summary>
        /// A logger to keep track of all action performed.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Reads a CSV file in an optimized way.
        /// </summary>
        /// <param name="filename">The file name to read.</param>
        /// <returns>The collection of records in the file.</returns>
        public static IEnumerable<string[]> Read(string filename)
        {
            logger.Info("About to try reading to the File={0} from FileHandler.Read(filename).", filename);
            if (File.Exists(filename))
            {
                using (var fileReader = File.OpenText(filename))
                using (var csvParser = new CsvHelper.CsvParser(fileReader))
                {
                    var hasMoreRecords = true;
                    var record = csvParser.Read();
                    while (hasMoreRecords)
                    {
                        yield return record;
                        record = csvParser.Read();
                        hasMoreRecords = !(record == null);
                    }
                }

                logger.Info("Read file={0}", filename);
            }
            else
            {
                logger.Error("File={0} does not exist; tried to read from FileHandler.Read(filename).", filename);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Writes a CSV file in an optimized way.  Modifies each record by applying the function passed in by the caller.
        /// </summary>
        /// <param name="filename">The file name to create and write to.</param>
        /// <param name="records">The collection of records to write.</param>
        /// <param name="perRecordFunction">A function to modify each record.</param>
        public static void Write(string filename, IEnumerable<string[]> records, Func<string[], string[]> perRecordFunction)
        {
            logger.Info("About to try writing to the File={0} from FileHandler.Write(filename, records, perRecordFunction).", filename);
            using (var textWriter = File.CreateText(filename))
            using (var csvWriter = new CsvHelper.CsvWriter(textWriter))
            {
                foreach (var record in records)
                {
                    var updatedRecord = perRecordFunction(record); 
                    foreach (var field in updatedRecord)
                    {
                        csvWriter.WriteField(field);
                    }

                    csvWriter.NextRecord();
                }
            }

            logger.Info("Writing to the File={0} was successful.", filename);
        }

        /// <summary>
        /// Writes a CSV file in an optimized way.
        /// </summary>
        /// <param name="filename">The file name to create and write to.</param>
        /// <param name="records">The collection of records to write.</param>
        public static void Write(string filename, IEnumerable<string[]> records)
        {
            logger.Info("About to try writing to the File={0} from FileHandler.Write(filename, records).", filename);
            using (var textWriter = File.CreateText(filename))
            using (var csvWriter = new CsvHelper.CsvWriter(textWriter))
            {
                foreach (var record in records)
                {
                    foreach (var field in record)
                    {
                        csvWriter.WriteField(field);
                    }

                    csvWriter.NextRecord();
                }
            }

            logger.Info("Writing to the File={0} was successful.", filename);
        }

        /// <summary>
        /// Writes a CSV file in an optimized way. The headers are written first.
        /// </summary>
        /// <param name="filename">The file name to create and write to.</param>
        /// <param name="records">The collection of records to write.</param>
        /// <param name="headers">The headers for the file.</param>
        public static void Write(string filename, IEnumerable<string[]> records, string[] headers)
        {
            logger.Info("About to try writing to the File={0} from FileHandler.Write(filename, records, headers).", filename);
            using (var textWriter = File.CreateText(filename))
            using (var csvWriter = new CsvHelper.CsvWriter(textWriter))
            {
                foreach (var header in headers)
                {
                    csvWriter.WriteField(header);
                }

                csvWriter.NextRecord();
                foreach (var record in records)
                {
                    foreach (var field in record)
                    {
                        csvWriter.WriteField(field);
                    }

                    csvWriter.NextRecord();
                }
            }

            logger.Info("Writing to the File={0} was successful.", filename);
        }

        /// <summary>
        /// Deletes a file (if it exists in the system).
        /// </summary>
        /// <param name="filename">The name of the file to delete.</param>
        public static void DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    logger.Info("Deleted the file={0}", filename);
                }
                catch (IOException e)
                {
                    logger.Info("Could not delete the file={0} because of an exception={1}", filename, e.Message);
                }
            }
            else
            {
                logger.Info("File={0} does not exist; tried to delete from FileHandler.Delete.", filename);
            }
        }
    }
}
