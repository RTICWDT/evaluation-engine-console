// -----------------------------------------------------------------------
// <copyright file="CipherTextAndVector.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    /// <summary>
    /// A class for storing a cipher text and an initialization vector.
    /// </summary>
    public class CipherTextAndVector
    {
        /// <summary>
        /// Gets or sets the cipher text.
        /// </summary>
        public string CipherText { get; set; }

        /// <summary>
        /// Gets or sets the initialization vector.
        /// </summary>
        public string Vector { get; set; }

        /// <summary>
        /// See: answer by Marc Gravell in http://stackoverflow.com/questions/2920399/c-sharp-how-to-find-if-two-objects-are-equal
        /// </summary>
        /// <returns>the object's hash code.</returns>
        public override int GetHashCode()
        {
            int i = 0x65407627;
            i = (i * -1521134295) + this.CipherText.GetHashCode();
            i = (i * -1521134295) + this.Vector.GetHashCode();

            return i; 
        }

        /// <summary>
        /// Are two objects of this type the same?
        /// True if they have the same cipher text and the same vector.
        /// False otherwise.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects are the equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            CipherTextAndVector otherCav = obj as CipherTextAndVector;
            if (otherCav == null)
            {
                return false;
            }

            return this.CipherText.Equals(otherCav.CipherText) && this.Vector.Equals(otherCav.Vector);
        }
    }
}
