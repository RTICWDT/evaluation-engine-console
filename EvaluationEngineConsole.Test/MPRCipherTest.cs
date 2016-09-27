// -----------------------------------------------------------------------
// <copyright file="MPRCipherTest.cs" company="">
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
    using System.Security.Cryptography;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class MPRCipherTest
    {
        [Test]
        public void Can_encrypt_and_decrypt()
        {
            // Arrange
            MPRCipher e = new MPRCipher();
            string secret = "This is a very secret message; seriously... Don't try to mess with it...";

            // Act
            var cav = e.GetCipherTextAndVector(secret);
            var target = e.GetPlainText(cav);

            // Assert
            Assert.AreNotEqual(secret, cav.CipherText);
            Assert.AreNotEqual(secret, cav.Vector);

            Assert.AreEqual(secret, target);
        }

        [Test]
        public void Can_encrypt_and_decrypt_with_client_key()
        {
            // Arrange
            var cipher = new MPRCipher();
            var secret = "This is a very secret message; seriously... Don't try to mess with it...";

            var clientKey = cipher.GenerateKey();

            // Act
            var cav = cipher.GetCipherTextAndVector(secret, clientKey);
            var target = cipher.GetPlainText(cav, clientKey);

            // Assert
            Assert.AreNotEqual(secret, cav.CipherText);
            Assert.AreNotEqual(secret, cav.Vector);

            Assert.AreEqual(secret, target);

        }

        [Test]
        public void Can_not_decrypt_with_wrong_vector()
        {
            // Arrange
            MPRCipher e = new MPRCipher();
            string secret = "This is a very secret message; seriously... Don't try to mess with it...";

            // Act - encrypt message
            var cav = e.GetCipherTextAndVector(secret);

            // Act - create new IV
            var cipher = new RijndaelManaged();
            cipher.BlockSize = 256;
            cipher.GenerateIV();
            var newVector = cipher.IV; 
            cav.Vector = Convert.ToBase64String(newVector); 

            // Act - decrypt
            var target = e.GetPlainText(cav);

            // Assert
            Assert.AreNotEqual(secret, target); 
        }

        [Test]
        public void Can_not_decrypt_with_wrong_key()
        {
            // Arrange
            MPRCipher e = new MPRCipher();
            string secret = "This is a very secret message; seriously... Don't try to mess with it...";

            // Act - encrypt message
            var cav = e.GetCipherTextAndVector(secret);

            // Act - create new cipher 
            var cipher = new RijndaelManaged();
            cipher.KeySize = 256;
            cipher.BlockSize = 256;
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Mode = CipherMode.CBC;

            // Act - use real IV
            cipher.IV = Convert.FromBase64String(cav.Vector);

            // Act - decrypt
            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();

            byte[] cipherText = Convert.FromBase64String(cav.CipherText);
            string target = null;
            try
            {
                byte[] text = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                target = Encoding.UTF8.GetString(text);
            }
            catch (System.Security.Cryptography.CryptographicException exception)
            {
                target = "Not the real seceret";
            }

            // Assert
            Assert.AreNotEqual(secret, target); 
        }

        [Test]
        public void Can_not_decrypt_with_wrong_key_and_wrong_vector()
        {
            // Arrange
            MPRCipher e = new MPRCipher();
            string secret = "This is a very secret message; seriously... Don't try to mess with it...";

            // Act - encrypt message
            var cav = e.GetCipherTextAndVector(secret);

            // Act - create new cipher 
            var cipher = new RijndaelManaged();
            cipher.KeySize = 256;
            cipher.BlockSize = 256;
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Mode = CipherMode.CBC;

            // Act - decrypt
            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();

            byte[] cipherText = Convert.FromBase64String(cav.CipherText);
            string target = null;
            try
            {
                byte[] text = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                target = Encoding.UTF8.GetString(text);
            }
            catch (System.Security.Cryptography.CryptographicException exception)
            {
                target = "Not the real seceret";
            }

            // Assert
            Assert.AreNotEqual(secret, target); 
        }

        [Test]
        public void Can_generate_unique_keys()
        {
            // Arrange
            MPRCipher e = new MPRCipher();

            // Act
            var key1 = e.GenerateKey();
            var key2 = e.GenerateKey();

            // Assert 
            Assert.AreNotEqual(key1, key2);
        }
    }
}
