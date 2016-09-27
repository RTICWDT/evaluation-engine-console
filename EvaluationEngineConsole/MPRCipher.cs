// -----------------------------------------------------------------------
// <copyright file="MPRCipher.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// A class that manages encryption and decryption.
    /// </summary>
    public class MPRCipher
    {
        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns>An object containing both the cipher text and vector.</returns>
        public CipherTextAndVector GetCipherTextAndVector(string plainText)
        {
            RijndaelManaged cipher = this.CreateCipher();

            ICryptoTransform cryptoTransform = cipher.CreateEncryptor();

            byte[] text = Encoding.UTF8.GetBytes(plainText);
            byte[] cypherText = cryptoTransform.TransformFinalBlock(text, 0, text.Length);

            CipherTextAndVector cav = new CipherTextAndVector
            {
                CipherText = Convert.ToBase64String(cypherText),
                Vector = Convert.ToBase64String(cipher.IV)
            };

            return cav;
        }

        /// <summary>
        /// Encrypts a string using a different key than the one generated internally by this class.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <param name="clientKey">The key to use for encryption.</param>
        /// <returns>The object containing the cipher text and vector.</returns>
        public CipherTextAndVector GetCipherTextAndVector(string plainText, string clientKey)
        {
            RijndaelManaged cipher = this.CreateCipher();

            cipher.Key = Convert.FromBase64String(clientKey);

            ICryptoTransform cryptoTransform = cipher.CreateEncryptor();

            byte[] text = Encoding.UTF8.GetBytes(plainText);
            byte[] cypherText = cryptoTransform.TransformFinalBlock(text, 0, text.Length);

            CipherTextAndVector cav = new CipherTextAndVector
            {
                CipherText = Convert.ToBase64String(cypherText),
                Vector = Convert.ToBase64String(cipher.IV)
            };

            return cav;
        }

        /// <summary>
        /// Decrypts a cipher text.
        /// </summary>
        /// <param name="cipherAndVector">An object containing the ciphertext and vector.</param>
        /// <returns>The decrypted string.</returns>
        public string GetPlainText(CipherTextAndVector cipherAndVector)
        {
            if (cipherAndVector == null || string.IsNullOrEmpty(cipherAndVector.CipherText) || string.IsNullOrEmpty(cipherAndVector.Vector))
            {
                return null;
            }

            RijndaelManaged cipher = this.CreateCipher();
            cipher.IV = Convert.FromBase64String(cipherAndVector.Vector);
            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();

            byte[] cipherText = Convert.FromBase64String(cipherAndVector.CipherText);
            byte[] text = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(text);
        }

        /// <summary>
        /// Decrypts a cipher using a given key.
        /// </summary>
        /// <param name="cipherAndVector">The object containing the cipher text and vector.</param>
        /// <param name="clientKey">The key to be used.</param>
        /// <returns>The decrypted string.</returns>
        public string GetPlainText(CipherTextAndVector cipherAndVector, string clientKey)
        {
            if (cipherAndVector == null || string.IsNullOrEmpty(cipherAndVector.CipherText) || string.IsNullOrEmpty(cipherAndVector.Vector))
            {
                return null;
            }

            RijndaelManaged cipher = this.CreateCipher();
            cipher.Key = Convert.FromBase64String(clientKey);
            cipher.IV = Convert.FromBase64String(cipherAndVector.Vector);

            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();

            byte[] cipherText = Convert.FromBase64String(cipherAndVector.CipherText);
            byte[] text = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(text);
        }

        /// <summary>
        /// Generates a key that could be used for a cipher.
        /// Use to populate configuration file.
        /// </summary>
        /// <returns>A key for a cipher.</returns>
        public string GenerateKey()
        {
            RijndaelManaged cipher = this.CreateCipher();

            cipher.GenerateKey();

            return Convert.ToBase64String(cipher.Key);
        }

        /// <summary>
        /// Creates a cipher. 
        /// </summary>
        /// <returns>A RijndaelManaged cipher object.</returns>
        private RijndaelManaged CreateCipher()
        {
            RijndaelManaged cipher = new RijndaelManaged();

            cipher.KeySize = 256;
            cipher.BlockSize = 256;
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Mode = CipherMode.CBC;

            // Read the key from the config file
            byte[] key = Convert.FromBase64String(ConfigurationManager.AppSettings["AES256"]);
            cipher.Key = key;

            return cipher;
        }
    }
}
