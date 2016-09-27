// -----------------------------------------------------------------------
// <copyright file="EEDataModel.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CsvHelper;
    using CsvHelper.Configuration;

    /// <summary>
    /// Specifies the columns in the files uploaded by the states
    /// </summary>
    public class EEDataModel
    {
        /// <summary>
        /// Gets or sets the HashedId
        /// </summary>
        public string HashedId { get; set; }

        /// <summary>
        /// Gets or sets the StudyId
        /// </summary>
        public string StudyId { get; set; }

        /// <summary>
        /// Gets or sets the CipherText
        /// </summary>
        public string CipherText { get; set; }

        /// <summary>
        /// Gets or sets the Initialization Vector
        /// </summary>
        public string Vector { get; set; }

        /// <summary>
        /// Sets and then gets the actual values for HashedId, StudyId, CipherText and Vector.
        /// </summary>
        /// <param name="stateId">The state student id.</param>
        /// <returns>An array containing the values for HashedId, StudyId, CipherText and Vector.</returns>
        public string[] GetFields(string stateId)
        {
            this.PopulateFields(stateId);
            return new string[] { this.HashedId, this.StudyId, this.CipherText, this.Vector };
        }

        /// <summary>
        /// Sets the values for HashedId, StudyId, CipherText, and Vector.
        /// </summary>
        /// <param name="stateId">The state student id.</param>
        public void PopulateFields(string stateId)
        {
            MPRCipher cipher = new MPRCipher();
            MPRHashing hashing = new MPRHashing();

            var hashedId = hashing.HashID(stateId);
            var noise = "+(ups{Cap#";
            var salt = Guid.NewGuid().ToString();
            var studyId = hashing.ComputeStudyID(hashedId, salt, noise);
            var cipherAndVector = cipher.GetCipherTextAndVector(stateId);

            this.HashedId = hashedId;
            this.StudyId = studyId;
            this.CipherText = cipherAndVector.CipherText;
            this.Vector = cipherAndVector.Vector;
        }

        /// <summary>
        /// Hashes additional fields.
        /// </summary>
        /// <param name="otherFields">A complete student record.</param>
        /// <param name="indexOfFieldsToHash">A list with the indices of the fields to hash.</param>
        /// <returns>The student record with the additional fields hashed.</returns>
        public string[] HashAdditionalFields(string[] otherFields, IList<int> indexOfFieldsToHash)
        {
            MPRHashing hashing = new MPRHashing();
            foreach (var index in indexOfFieldsToHash)
            {
                otherFields[index] = hashing.HashID(otherFields[index]); 
            }

            return otherFields;
        }

        /// <summary>
        /// Extends the student data record by adding four new fields: HashedId, StudyId, CipherText and Vector.
        /// It also hashes additional fields in the student record.
        /// </summary>
        /// <param name="otherFields">The student record (a row in the csv file uploaded by the user.  This is a string array.</param>
        /// <param name="indexOfFieldsToHash">A list with the indices of the fields to hash.</param>
        /// <param name="indexStateId">The index of the state student id.</param>
        /// <returns>An augmented student record containing four new fields: HashedId, StudyId, CipherText and Vector.</returns>
        public string[] Concat(string[] otherFields, IList<int> indexOfFieldsToHash, int indexStateId)
        {
            var dataArray = this.GetFields(otherFields[indexStateId]);

            // Note that this will hash the student state id.
            otherFields = this.HashAdditionalFields(otherFields, indexOfFieldsToHash);

            if (otherFields != null)
            {
                var dataArrayOldLength = dataArray.Length;
                Array.Resize<string>(ref dataArray, otherFields.Length + dataArray.Length);
                Array.Copy(otherFields, 0, dataArray, dataArrayOldLength, otherFields.Length);
                return dataArray;
            }

            return dataArray;
        }
    }
}
