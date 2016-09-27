// -----------------------------------------------------------------------
// <copyright file="DispatcherIntegrationTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole.Test
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class DispatcherIntegrationTests
    {
        private Options options = new Options();

        [SetUp]
        public void SetUp()
        {
            options.State = "NM";
            options.OutputFile = @"C:\Users\davidj\Documents\Visual Studio 2010\Projects\EvaluationEngineConsole\EvaluationEngineConsole.Test\CsvTestFiles\TestOutputFile.csv";
            options.StateIdField = "STUDENTID_STATE";
            options.TimeApplication = false;
            options.TargetFile = @"..\..\CsvTestFiles\MockStateData_validation_1k.csv";
            options.ValidationFile = @"C:\Users\davidj\Documents\Visual Studio 2010\Projects\EvaluationEngineConsole\EvaluationEngineConsole.Test\CsvTestFiles\TestValidationFile.csv";
            options.FieldsToHash = new string[] { };
        }

        [Test]
        public void Can_generate_stata_file()
        {
            // Arrange: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");

            // Act: generate target file.
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Assert
            Assert.IsTrue(File.Exists(this.options.OutputFile));
        }

        /// <summary>
        /// Assumes no duplicates.
        /// </summary>
        [Test]
        public void Output_file_has_correct_number_of_records()
        {
            // Arrange: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");

            // Act: Generate target file.
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            var expectedNumber = FileHandler.Read(this.options.TargetFile).Count();
            var actualNumber = FileHandler.Read(this.options.OutputFile).Count();

            // Assert
            Assert.AreEqual(expectedNumber, actualNumber);
        }

        /// <summary>
        /// Start with an empty database.
        /// Do the study ids in the file match those in the database?
        /// </summary>
        [Test]
        public void Has_correct_ids_with_xwalkdb_empty()
        {
            // Arrange: determine number of records in target file
            // skip the first row because it's the header.
            var numrecords = FileHandler.Read(this.options.TargetFile).Skip(1).Count();

            // Arrange: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");

            // Act: Generate target file.
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            // Assert: output has same number of records as input.
            Assert.AreEqual(numrecords, records.Skip(1).Count());

            // Assert: database has same number of records as input.
            Assert.AreEqual(numrecords, data.GetNumberOfRecordsInXWalk());
        }

        /// <summary>
        /// Upload data. Then reload it.  
        /// Do the study ids in the file match those in the database?
        /// </summary>
        [Test]
        public void Has_correct_ids_with_walkdb_populated()
        {
           // Arrange: determine number of records in target file
            // skip the first row because it's the header.
            var numrecords = FileHandler.Read(this.options.TargetFile).Skip(1).Count();

            // Arrange: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");

            // Arrange: load data.
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: reload the data and generate output file
            dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            // Assert: output has same number of records as input.
            Assert.AreEqual(numrecords, records.Skip(1).Count());

            // Assert: database has same number of records as input.
            Assert.AreEqual(numrecords, data.GetNumberOfRecordsInXWalk());
        }

        /// <summary>
        /// The 1k file has 500 records that do NOT exist in the 9k file.
        /// The other 500 records overlap.
        /// The 9k file actually has 9500 records.
        /// At the end the database should have 10,000 records.
        /// </summary>
        [Test]
        public void Has_correct_study_ids()
        {
            // Arrage: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");  // 

            // Arrange: upload the 10k records
            this.options.TargetFile = this.options.TargetFile.Replace("1k", "9k");
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: upload 1k records
            this.options.TargetFile = this.options.TargetFile.Replace("9k", "1k");
            dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            Assert.AreEqual(10000, data.GetNumberOfRecordsInXWalk());
        }

        /// <summary>
        /// Ensure the process does not fail simply because the user failed to use the proper case.
        /// </summary>
        [Test]
        public void Handles_headers_with_upper_and_lower_case()
        {
            // Arrange: determine number of records in target file
            // skip the first row because it's the header.
            this.options.TargetFile = this.options.TargetFile.Replace("1k", "lower");
            var numrecords = FileHandler.Read(this.options.TargetFile).Skip(1).Count();

            // Arrage: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");  // 

            // Arrange: upload file with lower case headers. 
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            // Assert: output has same number of records as input.
            Assert.AreEqual(numrecords, records.Skip(1).Count());

            // Assert: database has same number of records as input.
            Assert.AreEqual(numrecords, data.GetNumberOfRecordsInXWalk());
        }

        /// <summary>
        /// The file MockStateData_validation_single_duplicate.cvs contains 100 records.  
        /// They all have the same state student id (the records are different otherwise).
        /// The application should generate a file for the Stata server where all records have the same StudyId.
        /// </summary>
        [Test]
        public void Handles_single_duplicate()
        {
            // Arrange: determine number of records in target file
            // skip the first row because it's the header.
            this.options.TargetFile = this.options.TargetFile.Replace("1k", "single_duplicate");
            var numrecords = FileHandler.Read(this.options.TargetFile).Skip(1).Count();

            // Arrage: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");  // 

            // Arrange: upload file. 
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            // Assert: output has same number of records as input.
            Assert.AreEqual(numrecords, records.Skip(1).Count());

            // Assert: database has same number of records as input.
            Assert.AreEqual(1, data.GetNumberOfRecordsInXWalk());
        }

        /// <summary>
        /// The file MockStateData_validation_multiple_duplicates.cvs contains 100 records.  
        /// The first 32 share the same state student id (z).
        /// The next 33 also share a state student id (y).
        /// The remaining 35 records all have distinct ids.
        /// In all there are 37 distinct state student ids.
        /// </summary>
        [Test]
        public void Handles_multiple_duplicates()
        {
            // Arrange: determine number of records in target file
            // skip the first row because it's the header.
            this.options.TargetFile = this.options.TargetFile.Replace("1k", "multiple_duplicates");
            var numrecords = FileHandler.Read(this.options.TargetFile).Skip(1).Count();

            // Arrage: Truncate the xwalk table.
            var data = new DataProvider();
            data.TruncateTable("Crosswalk");  // 

            // Arrange: upload file. 
            var dispatcher = new Dispatcher(this.options, data);
            dispatcher.Dispatch();

            // Act: create comparisons
            var records = FileHandler.Read(this.options.OutputFile);

            // Assert
            foreach (var record in records.Skip(1))
            {
                var studyId = data.GetStudyIdByHashedId(record[0]);
                if (studyId == null)
                {
                    Console.WriteLine(record[0]);
                }
                Assert.AreEqual(studyId, record[1]);
            }

            // Assert: output has same number of records as input.
            Assert.AreEqual(numrecords, records.Skip(1).Count());

            // Assert: database has same number of records as input.
            Assert.AreEqual(37, data.GetNumberOfRecordsInXWalk());
        }

        [TearDown]
        public void TearDown()
        {
            FileHandler.DeleteFile(this.options.OutputFile);
            FileHandler.DeleteFile(this.options.ValidationFile);
            FileHandler.DeleteFile(this.options.ValidationFile.Replace("TestValidationFile", "TestValidationFile_statistics"));
        }
    }
}
