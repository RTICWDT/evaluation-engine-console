// -----------------------------------------------------------------------
// <copyright file="OuputIdsManager.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    /// <summary>
    /// A class for handling encryption and decryptions of lists of ids using our key, or a client's.
    /// </summary>
    public class OuputIdsManager
    {
        /// <summary>
        /// An object that handles connection to the database.
        /// </summary>
        private IDataProvider dataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="OuputIdsManager" /> class. 
        /// </summary>
        /// <param name="dataProvider">An instance of a data provider to connect to the database.</param>
        public OuputIdsManager(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        /// <summary>
        /// Gets a ciphertext that uses MPR's key
        /// 1] Gets the ciphertext and vector stored in the dabase (this will be returned to the client).
        /// 2] Decrypts the ciphertext.
        /// 3] Encrypts the id with a new vector.
        /// </summary>
        /// <param name="studyId">The studyId of the real id we want to return.</param>
        /// <returns>A ciphertext and vector that can be use to retrieve the real id.</returns>
        public CipherTextAndVector GetIdEncryptedWithMPRKey(string studyId)
        {
            var outputCipherAndVector = this.dataProvider.GetCipherTextAndVectorByStudyId(studyId);

            var cipher = new MPRCipher();

            var newCipherAndVector = cipher.GetCipherTextAndVector(cipher.GetPlainText(outputCipherAndVector));

            this.dataProvider.UpdateCipherTextAndVector(outputCipherAndVector, newCipherAndVector);

            return outputCipherAndVector;
        }

        /// <summary>
        /// Gets a ciphertext that uses a client's key 
        /// 1] Gets the ciphertext and vector stored in the database.
        /// 2] Decrypts the ciphertext.
        /// 3] Encrypts the id with the client key.
        /// </summary>
        /// <param name="studyId">The studyId of the real id we want to return.</param>
        /// <param name="clientKey">The client's key.</param>
        /// <returns>A ciphertext and vector that can be use to retrieve the real id.</returns>
        public CipherTextAndVector GetIdEncryptedWithClientKey(string studyId, string clientKey)
        {
            var oldCipherAndVector = this.dataProvider.GetCipherTextAndVectorByStudyId(studyId);

            var cipher = new MPRCipher();

            var newCipherAndVector = cipher.GetCipherTextAndVector(cipher.GetPlainText(oldCipherAndVector));

            this.dataProvider.UpdateCipherTextAndVector(oldCipherAndVector, newCipherAndVector);

            return cipher.GetCipherTextAndVector(cipher.GetPlainText(oldCipherAndVector), clientKey);
        }
    }
}
