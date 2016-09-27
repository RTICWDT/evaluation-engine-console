// -----------------------------------------------------------------------
// <copyright file="IDataProvider.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace EvaluationEngineConsole
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An interface for a class that connects the application to a database. 
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Creates a table in the database where data uploaded by the user are temporarily stored.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="tableColumns">The name of the columns for the table.</param>
        void CreateUploadDataTable(string tableName, string[] tableColumns);

        /// <summary>
        /// Drops a table from the Crosswalk database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        void DropUploadDataTable(string tableName);

        /// <summary>
        /// Gets the ciphertext and vector that correspond to a hashed id.
        /// </summary>
        /// <param name="hashedId">The hashed id.</param>
        /// <returns>The CipherTextAndVector object corresponding to the hashed id.</returns>
        CipherTextAndVector GetCipherTextAndVectorByHashedId(string hashedId);

        /// <summary>
        /// Gets the ciphertext and vector that correspond to a study id.
        /// </summary>
        /// <param name="studyId">The study id</param>
        /// <returns>The CipherTextAndVector object corresponding to the study id.</returns>
        CipherTextAndVector GetCipherTextAndVectorByStudyId(string studyId);

        /// <summary>
        /// Gets a set of all the hashed id that have duplicates in a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A set with repeated hashed ids.</returns>
        HashSet<string> GetDuplicateHashedIds(string tableName);

        /// <summary>
        /// Gets a hashed id that corresponds to study id.
        /// </summary>
        /// <param name="studyId">The study id.</param>
        /// <returns>The hashed id if the record exists in the database; NULL otherwise.</returns>
        string GetHashedIdByStudyId(string studyId);

        /// <summary>
        /// Gets the number of records in the crosswalk database.
        /// </summary>
        /// <returns>An integer that corresponds to the number of records in the database; -1 if something went wrong.</returns>
        int GetNumberOfRecordsInXWalk();

        /// <summary>
        /// Gets the study id that corresponds to hashed id.
        /// </summary>
        /// <param name="hashedId">The hashed id.</param>
        /// <returns>A study id if the record exists in the database; NULL otherwise.</returns>
        string GetStudyIdByHashedId(string hashedId);

        /// <summary>
        /// Inserts a record into the crosswalk database.  A record consists of:
        /// 1) a hashed id, 2) a cipher text, 3) a vector, and 4) a study id.
        /// </summary>
        /// <param name="hashedId">The hashed id to insert.</param>
        /// <param name="cipherAndVector">The object containing the ciphertext and vector.</param>
        /// <param name="studyId">The study id.</param>
        void InsertCrosswalkRow(string hashedId, CipherTextAndVector cipherAndVector, string studyId);

        /// <summary>
        /// Inserts study ids that exist in the user's data table (but not in the Crosswalk) into the Crosswalk table.
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        void InsertRecordsInCrosswalk(string tableName);

        /// <summary>
        /// Generates a file containing the correct study ids to be passed to the Stata server.
        /// Note that this file gets generated in the machine hosting the database.
        /// </summary>
        /// <param name="outputFile">The absolute file path for the file.</param>
        /// <param name="tableName">The name of the table containing the records.</param>
        /// <param name="headers">The fields that the output file should contain.</param>
        void OutputFileForStata(string outputFile, string tableName, string[] headers);

        /// <summary>
        /// Truncates a table in the Crosswalk database.
        /// </summary>
        /// <param name="tableName">The name of the table to truncate.</param>   
        void TruncateTable(string tableName);

        /// <summary>
        /// Updates the values of a cipher text and its initialization vector.
        /// </summary>
        /// <param name="oldcav">The cipher text and vector we want to replace.</param>
        /// <param name="newcav">The cipher text and vector we want to insert in the database.</param>
        void UpdateCipherTextAndVector(CipherTextAndVector oldcav, CipherTextAndVector newcav);

        /// <summary>
        /// Ensures that all duplicate records (i.e., records with the same HashedId) have the same StudyId
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        void UpdateDuplicatesStudyIds(string tableName);

        /// <summary>
        /// Updates the study ids of records in the table where user data were uploaded to.
        /// So, the study id in the Crosswalk table (if it exists) is used to populate the target table.
        /// </summary>
        /// <param name="tableName">The name of the table where the user's data were uploaded to.</param>
        void UpdateStudyIdsInUploadFile(string tableName);

        /// <summary>
        /// Uploads a csv file (the one submitted by the user) into a table.  This is done using a bulk upload.
        /// Note that the csv file is sent from the client to the server.  The client does not have to be in the same
        /// machine as the database. 
        /// </summary>
        /// <param name="tableName">The name of the table where we want to  insert the data.</param>
        /// <param name="dataFile">The path to the csv file we wan to upload.</param>
        void UploadOutputFile(string tableName, string dataFile);
    }
}
