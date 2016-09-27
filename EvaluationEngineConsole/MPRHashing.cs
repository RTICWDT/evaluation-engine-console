// -----------------------------------------------------------------------
// <copyright file="MPRHashing.cs" company="MPR INC">
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
    /// A class in charge of hashing strings.
    /// </summary>
    public class MPRHashing
    {
        /// <summary>
        /// Hash a string.
        /// </summary>
        /// <param name="id">The string to hash.</param>
        /// <returns>The hashed string.</returns>
        public string HashID(string id)
        {
            HMACSHA512 hashAlg = new HMACSHA512(Convert.FromBase64String(ConfigurationManager.AppSettings["Key64"]));

            byte[] text = Encoding.UTF8.GetBytes(id);
            byte[] hash = hashAlg.ComputeHash(text);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generates a study id.
        /// </summary>
        /// <param name="hashedId">A hashed string.</param>
        /// <param name="salt">A string unique for every hashed id.</param>
        /// <param name="noise">A random string.</param>
        /// <returns>A 88 character string.</returns>
        public string ComputeStudyID(string hashedId, string salt, string noise)
        {
            SHA512Managed hashAlg = new SHA512Managed();

            byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(hashedId + salt + noise));

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generates a key that can be later used to hash ids.
        /// </summary>
        /// <returns>A key in base 64.</returns>
        public string GenerateKey()
        {
            HMACSHA512 hashAlg = new HMACSHA512();

            return Convert.ToBase64String(hashAlg.Key);
        }
    }
}
