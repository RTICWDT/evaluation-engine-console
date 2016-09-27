// -----------------------------------------------------------------------
// <copyright file="MPRHashingTest.cs" company="">
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
    [TestFixture]
    public class MPRHashingTest
    {
        [Test]
        public void Can_generate_unique_keys()
        {
            // Arrange
            MPRHashing h = new MPRHashing();

            // Act
            var key1 = h.GenerateKey();
            var key2 = h.GenerateKey();

            // Assert
            Assert.AreNotEqual(key1, key2);
        }
    }
}
