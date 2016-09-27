// -----------------------------------------------------------------------
// <copyright file="DataProvider.cs" company="MPR INC">
// See: http://npgsql.projects.pgfoundry.org/docs/manual/UserManual.html
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NLog;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// A class for running commands, and moving data to and from the PostgreSQL database.
    /// </summary>
    public class DataProvider : EvaluationEngineConsole.IDataProvider 
    {
        /// <summary>
        /// A logger to help with debugging.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the connection string to connect to the database;
        /// </summary>
        private string CrosswalkConnectionString 
        { 
            get { return ConfigurationManager.ConnectionStrings["crosswalk"].ToString(); } 
        }
       
        /// <summary>
        /// Gets the study id that corresponds to hashed id.
        /// </summary>
        /// <param name="hashedId">The hashed id.</param>
        /// <returns>A study id if the record exists in the database; NULL otherwise.</returns>
        public string GetStudyIdByHashedId(string hashedId)
        {
            string studyId = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"select ""StudyId"" from ""Crosswalk"" where ""HashedId"" = :key", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("key", NpgsqlDbType.Text));
                    command.Prepare();
                    command.Parameters[0].Value = hashedId;
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            studyId = dr[0].ToString();
                        }
                    }
                }
            }

            return studyId;
        }

        /// <summary>
        /// Gets a hashed id that corresponds to study id.
        /// </summary>
        /// <param name="studyId">The study id.</param>
        /// <returns>The hashed id if the record exists in the database; NULL otherwise.</returns>
        public string GetHashedIdByStudyId(string studyId)
        {
            string hashedId = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"select ""HashedId"" from ""Crosswalk"" where ""StudyId"" = :studyId", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("studyId", NpgsqlDbType.Text));
                    command.Prepare();
                    command.Parameters[0].Value = studyId;
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            studyId = dr[0].ToString();
                        }
                    }
                }
            }

            return hashedId;
        }

        /// <summary>
        /// Gets the number of records in the crosswalk database.
        /// </summary>
        /// <returns>An integer that corresponds to the number of records in the database; -1 if something went wrong.</returns>
        public int GetNumberOfRecordsInXWalk()
        {
            int count = -1;
            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"select count(*) from ""Crosswalk"";", conn))
                {
                    command.Prepare();
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            count = Convert.ToInt32(dr[0].ToString());
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets a set of all the hashed id that have duplicates in a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A set with repeated hashed ids.</returns>
        public HashSet<string> GetDuplicateHashedIds(string tableName)
        {
            HashSet<string> repeatedIds = new HashSet<string>();
            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                var sqlCommand = string.Format(@"select ""HashedId"" from ""{0}"" group by ""HashedId"" having count(*) > 1;", tableName);
                using (NpgsqlCommand command = new NpgsqlCommand(sqlCommand, conn))
                {
                    command.Prepare();
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            repeatedIds.Add(dr[0].ToString());
                        }
                    }
                }
            }

            return repeatedIds;
        }

        /// <summary>
        /// Gets the ciphertext and vector that correspond to a study id.
        /// </summary>
        /// <param name="studyId">The study id</param>
        /// <returns>The CipherTextAndVector object corresponding to the study id.</returns>
        public CipherTextAndVector GetCipherTextAndVectorByStudyId(string studyId)
        {
            CipherTextAndVector cav = new CipherTextAndVector();

            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"select ""CipherText"", ""Vector"" from ""Crosswalk"" where ""StudyId"" = :studyId;", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("studyId", NpgsqlDbType.Text));
                    command.Prepare();
                    command.Parameters[0].Value = studyId;
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cav.CipherText = dr["CipherText"].ToString();
                            cav.Vector = dr["Vector"].ToString();
                        }
                    }
                }
            }

            return cav;
        }

        /// <summary>
        /// Gets the ciphertext and vector that correspond to a hashed id.
        /// </summary>
        /// <param name="hashedId">The hashed id.</param>
        /// <returns>The CipherTextAndVector object corresponding to the hashed id.</returns>
        public CipherTextAndVector GetCipherTextAndVectorByHashedId(string hashedId)
        {
            CipherTextAndVector cav = new CipherTextAndVector();

            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(@"select ""CipherText"", ""Vector"" from ""Crosswalk"" where ""HashedId"" = :key;", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("key", NpgsqlDbType.Text));
                    command.Prepare();
                    command.Parameters[0].Value = hashedId;
                    using (NpgsqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cav.CipherText = dr["CipherText"].ToString();
                            cav.Vector = dr["Vector"].ToString();
                        }
                    }
                }
            }

            return cav;
        }

        /// <summary>
        /// Updates the values of a cipher text and its initialization vector.
        /// </summary>
        /// <param name="oldcav">The cipher text and vector we want to replace.</param>
        /// <param name="newcav">The cipher text and vector we want to insert in the database.</param>
        public void UpdateCipherTextAndVector(CipherTextAndVector oldcav, CipherTextAndVector newcav)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                conn.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(@"update ""Crosswalk"" set ""CipherText"" = :newCipherText, ""Vector"" = :newVector where ""CipherText"" = :oldCipherText and ""Vector"" = :oldVector;", conn))
                {
                    // Now add the parameter to the parameter collection of the command specifying its type.
                    command.Parameters.Add(new NpgsqlParameter("oldCipherText", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("oldVector", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("newCipherText", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("newVector", NpgsqlDbType.Text));

                    // Now, add a value to it and later execute the command as usual.
                    command.Parameters["oldCipherText"].Value = oldcav.CipherText;
                    command.Parameters["oldVector"].Value = oldcav.Vector;
                    command.Parameters["newCipherText"].Value = newcav.CipherText;
                    command.Parameters["newVector"].Value = newcav.Vector;

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a record into the crosswalk database.  A record consists of:
        /// 1) a hashed id, 2) a cipher text, 3) a vector, and 4) a study id.
        /// </summary>
        /// <param name="hashedId">The hashed id to insert.</param>
        /// <param name="cipherAndVector">The object containing the ciphertext and vector.</param>
        /// <param name="studyId">The study id.</param>
        public void InsertCrosswalkRow(string hashedId, CipherTextAndVector cipherAndVector, string studyId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(this.CrosswalkConnectionString))
            {
                connection.Open();
                var sqlQuery = @"insert into ""Crosswalk"" (""HashedId"", ""CipherText"", ""Vector"", ""StudyId"") values (:hashedId, :cipherText, :vector, :studyId)";
                using (NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection))
                {
                    // Now add the parameter to the parameter collection of the command specifying its type.
                    command.Parameters.Add(new NpgsqlParameter("hashedId", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("cipherText", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("vector", NpgsqlDbType.Text));
                    command.Parameters.Add(new NpgsqlParameter("studyId", NpgsqlDbType.Text));

                    command.Prepare();

                    // Now, add a value to it and later execute the command as usual.
                    command.Parameters["hashedId"].Value = hashedId;
                    command.Parameters["cipherText"].Value = cipherAndVector.CipherText;
                    command.Parameters["vector"].Value = cipherAndVector.Vector;
                    command.Parameters["studyId"].Value = studyId;

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Creates a table in the database where data uploaded by the user are temporarily stored.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="tableColumns">The name of the columns for the table.</param>
        public void CreateUploadDataTable(string tableName, string[] tableColumns)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(@"""");
            sb.Append(tableName);
            sb.Append(@""" (");
            foreach (var column in tableColumns)
            {
                sb.Append(@"""");
                sb.Append(column);
                sb.Append(@"""");
                sb.Append(" text NULL,");
            }

            sb.Append(@" CONSTRAINT """);
            sb.Append(tableName);
            sb.Append("PrimaryKey");
            sb.Append(@""" PRIMARY KEY ");
            sb.Append(@"(""Vector""))");
            this.GenericSqlCommand(sb.ToString());
        }

        /// <summary>
        /// Truncates a table in the Crosswalk database.
        /// </summary>
        /// <param name="tableName">The name of the table to truncate.</param>
        public void TruncateTable(string tableName)
        {
            var templateQuery = @"TRUNCATE TABLE ""@tableName"";";
            var sqlQuery = templateQuery.Replace("@tableName", tableName);
            this.GenericSqlCommand(sqlQuery);
        }

        /// <summary>
        /// Drops a table from the Crosswalk database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public void DropUploadDataTable(string tableName)
        {
            string sqlCommand = @"DROP TABLE IF EXISTS """ + tableName + @""";";
            this.GenericSqlCommand(sqlCommand);
        }

        /// <summary>
        /// Uploads a csv file (the one submitted by the user) into a table.  This is done using a bulk upload.
        /// Note that the csv file is sent from the client to the server.  The client does not have to be in the same
        /// machine as the database. 
        /// </summary>
        /// <param name="tableName">The name of the table where we want to  insert the data.</param>
        /// <param name="dataFile">The path to the csv file we wan to upload.</param>
        public void UploadOutputFile(string tableName, string dataFile)
        {
            NpgsqlConnection connection = new NpgsqlConnection(this.CrosswalkConnectionString);
            connection.Open();

            var stream = File.Open(dataFile, FileMode.Open);

            var sqlQuery = @"copy """ + tableName + @""" from STDIN WITH CSV";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
            NpgsqlCopyIn copyIn = new NpgsqlCopyIn(command, connection);
            try
            {
                copyIn.Start();
                Stream copyInStream = copyIn.CopyStream;
                byte[] buf = new byte[9];
                int i;
                while ((i = stream.Read(buf, 0, buf.Length)) > 0)
                {
                    copyInStream.Write(buf, 0, i);
                }

                copyInStream.Close(); // or cin.End(), if you wish

                logger.Info("The sqlCommand={0} was completed successfully", sqlQuery);
            }
            catch (NpgsqlException e)
            {
                logger.Error("The sqlCommand={0} failed; error={1}", sqlQuery, e.Message);
                try
                {
                    copyIn.Cancel("Undo Copy");
                }
                catch (NpgsqlException e2)
                {
                    // we should get an error in response to our cancel request:
                    if (!(string.Empty + e2).Contains("Undo copy"))
                    {
                        logger.Error("Failed to cancel copy; error={0}", e2.Message);
                    }
                }
            }
            finally
            {
                stream.Close();
                connection.Close();
                logger.Info("The file stream and database connection were properly closed after sqlCommand={0}", sqlQuery);
            }
        }

        /// <summary>
        /// Ensures that all duplicate records (i.e., records with the same HashedId) have the same StudyId
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        public void UpdateDuplicatesStudyIds(string tableName)
        {
            var templateQuery = @"update ""@tableName"" set ""StudyId"" = ""subquery"".""StudyId"" from (select distinct on(""HashedId"") ""HashedId"", ""StudyId"" from ""@tableName"") AS ""subquery"" where ""@tableName"".""HashedId"" = ""subquery"".""HashedId"";";
            var sqlQuery = templateQuery.Replace("@tableName", tableName);
            this.GenericSqlCommand(sqlQuery);
        }

        /// <summary>
        /// Updates the study ids of records in the table where user data were uploaded to.
        /// So, the study id in the Crosswalk table (if it exists) is used to populate the target table.
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        public void UpdateStudyIdsInUploadFile(string tableName)
        {
            var templateQuery = @"update ""@tableName"" set ""StudyId"" = ""cw"".""StudyId"" from ""Crosswalk"" ""cw"" where ""@tableName"".""HashedId"" = ""cw"".""HashedId""; ";
            var sqlQuery = templateQuery.Replace("@tableName", tableName);
            this.GenericSqlCommand(sqlQuery);
        }

        /// <summary>
        /// Inserts study ids that exist in the user's data table (but not in the Crosswalk) into the Crosswalk table.
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        public void InsertRecordsInCrosswalk(string tableName)
        {
            var templateQuery = @"insert into ""Crosswalk"" (""HashedId"", ""StudyId"", ""CipherText"", ""Vector"") 
                                select distinct on (""HashedId"") ""HashedId"", ""StudyId"", ""CipherText"", ""Vector"" from ""@tableName"" 
                                where not exists 
                                (select * from ""Crosswalk"" where ""Crosswalk"".""HashedId"" = ""@tableName"".""HashedId"");";

            var sqlQuery = templateQuery.Replace("@tableName", tableName);

            this.GenericSqlCommand(sqlQuery);
        }

        /// <summary>
        /// Generates a file containing the correct study ids to be passed to the Stata server.
        /// Note that this file gets generated in the machine hosting the database.
        /// </summary>
        /// <param name="outputFile">The absolute file path for the file.</param>
        /// <param name="tableName">The name of the table containing the records.</param>
        /// <param name="headers">The fields that the output file should contain.</param>
        public void OutputFileForStata(string outputFile, string tableName, string[] headers)
        {
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append(@"copy (select ");

            foreach (var header in headers.Take(headers.Length - 1))
            {
                sqlQuery.Append(@"""");
                sqlQuery.Append(header);
                sqlQuery.Append(@""",");
            }

            sqlQuery.Append(@"""");
            sqlQuery.Append(headers.Last());
            sqlQuery.Append(@"""");

            sqlQuery.Append(string.Format(@" from ""{0}"") to '{1}' WITH CSV HEADER;", tableName, outputFile));

            this.GenericSqlCommand(sqlQuery.ToString());
        }

        /// <summary>
        /// Executes SQL commands.
        /// </summary>
        /// <param name="sqlQuery">A sql query.</param>
        private void GenericSqlCommand(string sqlQuery)
        {
            logger.Info("About to attempt the sqlCommand={0}", sqlQuery);

            NpgsqlConnection connection = new NpgsqlConnection(this.CrosswalkConnectionString);
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);

            try
            {
                command.ExecuteNonQuery();
                logger.Info("The sqlCommand={0} was completed successfully", sqlQuery);
            }
            catch (NpgsqlException e)
            {
                logger.Error("The sqlCommand={0} failed; error={1}", sqlQuery, e.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
