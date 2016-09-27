// -----------------------------------------------------------------------
// <copyright file="OutputIdsManagerIntegrationTest.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OutputIdsManagerIntegrationTest
    {
        string uploadedFile = "TestDataIntegration.csv";
        string stataFile = "OutputIntegration.csv";
        string outputFile = "IdFile.csv";
        string idRecordOne = "XJT98VEO0PI";
        string idRecordTwo = "VRS88HHG2EE";

        [SetUp]
        public void SetUp()
        {
            FileHandler.DeleteFile(stataFile);
            FileHandler.DeleteFile(uploadedFile);
            FileHandler.DeleteFile(outputFile);
            FileHandler.AppendLine(uploadedFile, "id,name,last_name");
            FileHandler.AppendLine(uploadedFile, "XJT98VEO0PI,Donovan,Buchanan");
            FileHandler.AppendLine(uploadedFile, "VRS88HHG2EE,Dominic,Larsen");
        }

        [Test]
        public void Can_output_ciphertext_and_vector_with_MPR_key()
        {
            // Arrange
            DataProvider data = new DataProvider();
            (new DocumentParser()).Parse(uploadedFile, stataFile);

            var studyIdsList = new List<string>();
            foreach (var line in FileHandler.ReadLines(stataFile))
            {
                studyIdsList.Add(line.Split(',')[0].Trim());
            }

            var hashing = new MPRHashing();
            var cavFirstRecord = data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordOne));
            var cavSecondRecord = data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordTwo));

            // Act
            var outputManager = new OuputIdsManager(data);
            outputManager.GetIdsUsingMPRKey(studyIdsList, outputFile); 

            // Assert

            // Ciphertext and vector should have been changed in the database
            Assert.AreNotEqual(cavFirstRecord, data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordOne)));
            Assert.AreNotEqual(cavSecondRecord, data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordTwo)));

            // Client should be able to retrieve original ids
            var cipher = new MPRCipher();
            var ciphertextOne = FileHandler.ReadLines(outputFile).ElementAt(0).Split(',');
            var ciphertextTwo = FileHandler.ReadLines(outputFile).ElementAt(1).Split(',');

            Assert.AreEqual(idRecordOne, cipher.GetPlainText(new CipherTextAndVector { CipherText = ciphertextOne[0].Trim(), Vector = ciphertextOne[1].Trim() }));

            Assert.AreEqual(idRecordTwo, cipher.GetPlainText(new CipherTextAndVector { CipherText = ciphertextTwo[0].Trim(), Vector = ciphertextTwo[1].Trim() }));

        }

        [Test]
        public void Can_output_ciphertext_and_vector_with_Client_key()
        {
            // Arrange
            DataProvider data = new DataProvider();
            (new DocumentParser()).Parse(uploadedFile, stataFile);

            var studyIdsList = new List<string>();
            foreach (var line in FileHandler.ReadLines(stataFile))
            {
                studyIdsList.Add(line.Split(',')[0].Trim());
            }

            var hashing = new MPRHashing();
            var cavFirstRecord = data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordOne));
            var cavSecondRecord = data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordTwo));

            var cipher = new MPRCipher();
            var clientKey = cipher.GenerateKey();

            // Act
            var outputManager = new OuputIdsManager(data);
            outputManager.GetIdsUsingClientKey(studyIdsList, outputFile, clientKey);

            // Assert

            // Ciphertext and vector should have been changed in the database
            Assert.AreNotEqual(cavFirstRecord, data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordOne)));
            Assert.AreNotEqual(cavSecondRecord, data.GetCipherTextAndVectorByHashedId(hashing.HashID(idRecordTwo)));

            // Client should be able to retrieve original ids
            var ciphertextOne = FileHandler.ReadLines(outputFile).ElementAt(0).Split(',');
            var ciphertextTwo = FileHandler.ReadLines(outputFile).ElementAt(1).Split(',');

            Assert.AreEqual(idRecordOne, cipher.GetPlainText(new CipherTextAndVector { CipherText = ciphertextOne[0].Trim(), Vector = ciphertextOne[1].Trim() }, clientKey));

            Assert.AreEqual(idRecordTwo, cipher.GetPlainText(new CipherTextAndVector { CipherText = ciphertextTwo[0].Trim(), Vector = ciphertextTwo[1].Trim() }, clientKey));
        }
    }
}
