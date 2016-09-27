// -----------------------------------------------------------------------
// <copyright file="OutputIdsManagerTest.cs" company="">
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
    using Moq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class OutputIdsManagerTest
    {
        Mock<IDataProvider> mockDataProvider;
        CipherTextAndVector startCipherTextAndVector;
        string clientKey;
        string testId = "myTestId153456";

        [SetUp]
        public void SetUp()
        {
            var cipher = new MPRCipher();
            startCipherTextAndVector = cipher.GetCipherTextAndVector(testId);
            clientKey = cipher.GenerateKey();

            mockDataProvider = new Mock<IDataProvider>();
        }

        [Test]
        public void CipherText_and_vector_get_updated_after_decryption_using_MPR_key()
        {
            // Arrange - data provider
            mockDataProvider.Setup(d => d.GetCipherTextAndVectorByStudyId(It.IsAny<string>())).Returns(startCipherTextAndVector);
            mockDataProvider.Setup(d => d.UpdateCipherTextAndVector(startCipherTextAndVector, It.IsAny<CipherTextAndVector>()));

            // Arrange - test object
            var outputManager = new OuputIdsManager(mockDataProvider.Object);

            // Act
            var testCypherTextAndVector = outputManager.GetIdEncryptedWithMPRKey("fakeStudyId");

            // Assert
            // The output ciphertext and vector should be what's stored in the database
            Assert.AreEqual(testCypherTextAndVector, startCipherTextAndVector);

            // The data provider should retrieve the ciphertext and vector from the database
            mockDataProvider.Verify(d => d.GetCipherTextAndVectorByStudyId(It.IsAny<string>()), Times.Once());

            // The method should update the ciphertext and vector by calling the UpdateCipherTextAndVector once.
            mockDataProvider.Verify(d => d.UpdateCipherTextAndVector(startCipherTextAndVector, It.IsAny<CipherTextAndVector>()), Times.Once());
        }

        [Test]
        public void CipherText_and_vector_get_updated_after_decryption_using_Client_key()
        {
            // Arrange - data provider
            mockDataProvider.Setup(d => d.GetCipherTextAndVectorByStudyId(It.IsAny<string>())).Returns(startCipherTextAndVector);
            mockDataProvider.Setup(d => d.UpdateCipherTextAndVector(startCipherTextAndVector, It.IsAny<CipherTextAndVector>()));

            // Arrange - test object
            var outputManager = new OuputIdsManager(mockDataProvider.Object);

            // Act
            var testCipherTextAndVector = outputManager.GetIdEncryptedWithClientKey("fakeStudyId", clientKey);

            // Assert
            // The output ciphertext and vector should NOT be what's stored in the database
            Assert.AreNotEqual(testCipherTextAndVector, startCipherTextAndVector);

            // The data provider should retrieve the ciphertext and vector from the database
            mockDataProvider.Verify(d => d.GetCipherTextAndVectorByStudyId(It.IsAny<string>()), Times.Once());

            // The method should update the ciphertext and vector by calling the UpdateCipherTextAndVector once.
            mockDataProvider.Verify(d => d.UpdateCipherTextAndVector(startCipherTextAndVector, It.IsAny<CipherTextAndVector>()), Times.Once());

        }
    }
}
