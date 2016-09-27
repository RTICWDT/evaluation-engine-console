// -----------------------------------------------------------------------
// <copyright file="EEValidationRulesTests.cs" company="">
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
    using EvaluationEngineConsole.Validation;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class EEValidationRulesTests
    {
        [Test]
        public void ACADYEAR_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { null, string.Empty, "89097", "1999", "supe", "FOUR" };

            foreach(var val in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.ACADYEAR(val);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void ACADYEAR_accepts_valid_values()
        {
            // Arrange
            var endYear = EEValidationRules.GetACADYEARendYear();

            // Arrange
            for (int year = 2010; year <= endYear; year++)
            {
                // Act
                var isValid = EEValidationRules.ACADYEAR(year.ToString());

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void ACADYEAR_accepts_valid_values_with_spaces()
        {
            // Arrange
            var validValue = "  2011   ";

            // Act
            var isValid = EEValidationRules.ACADYEAR(validValue);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void NoSpecialChars_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "su_per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "look I have spaces 'casue I'm special" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.NoSpecialChars(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void NoSpecialChars_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { null, string.Empty, "alllowercase", "ALLUPPERCASE", "MixeD", "withNumber1234" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.NoSpecialChars(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void AlphabeticSpaceOrHyphen_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "yum_", "su$per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "look I have apostrophes 'casue I'm special" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.AlphabeticSpaceOrHyphen(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void AlphabeticSpaceOrHyphen_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { null, string.Empty, "alllowercase", "ALLUPPERCASE", "MixeD" , "I have spaces", "and I - lord of strings - have hyhens"};

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.AlphabeticSpaceOrHyphen(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void AlphabeticOrSpace_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "yum_", "-nah-", "su-per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "look I have apostrophes 'casue I'm special" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.AlphabeticOrSpace(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void AlphabeticOrSpace_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { null, string.Empty, "alllowercase", "ALLUPPERCASE", "MixeD" , "I have spaces", "  Menacing WALRUS  " };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.AlphabeticOrSpace(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void Alphabetic_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "yum_", "8999", "su-per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "look I have apostrophes 'casue I'm special" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.Alphabetic(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Alphabetic_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "alllowercase", "ALLUPPERCASE", "MixeD", null, string.Empty };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.Alphabetic(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void NAME_SUFFIX_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "yum__", "89999", "su-per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "FOURS", " fours", "fOurS" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.NAME_SUFFIX(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void NAME_SUFFIX_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "Sr.", "Jr", "IV", " IIi ", null, string.Empty, "2nd", "3rd", "3's" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.NAME_SUFFIX(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void DISTRICTID_NCES_ENROLL_rejects_invalid_values()
        {
           // Arrange
            var invalidValues = new string[] { null, "yum_", "8999", "su-per", "w!", "e#  ", " ^ ^ ", " (><)", "47%", "9999999", "7900000", " 0099999 " };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.DISTRICTID_NCES_ENROLL(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void DISTRICTID_NCES_ENROLL_accepts_valid_values()
        {
            // Arrange: values ranging from smallest to largest acceptable values.
            var validValues = new string[] { "0100000", "1000000", "1111111", "7800000", "7899999" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.DISTRICTID_NCES_ENROLL(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void ZeroToOneOrNine_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { null, string.Empty, "1.1", "2", "8.9", "-1", "yum_", "8999", "su-per", "w!" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.ZeroToOneOrNine(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void ZeroToOneOrNine_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "0", "0.0001", "0.1", "0.9", " 0.9999999", "1 ", " 9 "};

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.ZeroToOneOrNine(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void GetYearMonthDay_returns_arrays_of_zeros_when_invalid_input()
        {
            // Arrange
            var invalidDates = new string[] { "2013md%%", string.Empty, " 20130215 ", "fourfour", "&*()f123", "a;fjk;alsjfd;alsjkfl;" };

            foreach (var date in invalidDates)
            {
                // Act
                var output = EEValidationRules.GetYearMonthDay(date);

                // Assert
                Assert.AreEqual(0, output[0]);
                Assert.AreEqual(0, output[1]);
                Assert.AreEqual(0, output[2]);
            }
                
        }

        [Test]
        public void GetYearMonthDay_returns_correct_array_when_input_is_valid()
        {
            // Arrange
            var inputDate = "20130215";

            // Act
            var output = EEValidationRules.GetYearMonthDay(inputDate);

            // Assert
            Assert.AreEqual(2013, output[0]);
            Assert.AreEqual(2, output[1]);
            Assert.AreEqual(15, output[2]);
        }

        [Test]
        public void IsValid_rejects_invalid_values()
        {
            // Arrange
            int[][] invalidValues = new int[][] { new int[] { 0, 0, 0 }, new int[] { 2013, 2, 31 }, new int[] { 1990, 99, 99 } };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.IsValidDate(value);

                // Assert
                Assert.IsFalse(isValid);
            }

        }

        [Test]
        public void IsValid_accepts_valid_values()
        {
            // Arrange
            int[][] validValues = new int[][] { new int[] { 2010, 3, 3 }, new int[] { 2013, 2, 13 }, new int[] { 2011, 12, 31 } };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.IsValidDate(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void BIRTHDATE_rejects_invalid_values()
        {
            // Arrange
            var maxYearPlusOne = EEValidationRules.GetBirthdayEndYear() + 1;
            var maxDatePlusOne = maxYearPlusOne + "0101";
            var invalidValues = new string[] { null, string.Empty, "19891231", maxDatePlusOne, "fourfour", "1237849014", "(*_(*_*)", "<(^ ^)>" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.BIRTHDATE(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void BIRTHDATE_accepts_valid_values()
        {
            // Arrange
            var maxDate = EEValidationRules.GetBirthdayEndYear().ToString() + "1231";
            var validValues = new string[] { "19900101", maxDate, "  19980213  " };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.BIRTHDATE(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void BetweenZeroAndTenK_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "-1", "blah", "MOREblah", "10000", null, string.Empty };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.BetweenZeroAndTenK(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void BetweenZeroAndTenK_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "0", " 100 ", "9999" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.BetweenZeroAndTenK(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void STUDENTID_STATE_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { null, string.Empty, "p(><)", " 123#four" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.STUDENTID_STATE(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void STUDENTID_STATE_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "AC37LTEM96", " E5CYCHNTBR " };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.STUDENTID_STATE(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void SCHOOL_DISTRICT_NAME_ENROLL_ASSESS_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { null, string.Empty, " &*()& ", "  $chool ", "_school_ yay " };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.SCHOOL_DISTRICT_NAME_ENROLL_ASSESS(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void SCHOOL_DISTRICT_NAME_ENROLL_ASSESS_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "E. Mt. Charter High", " cool-school ", " mary#s trip  ", " #3 is prime. In your face 6" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.SCHOOL_DISTRICT_NAME_ENROLL_ASSESS(value);

                Console.WriteLine(value + " is " + isValid);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void IsDouble_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { null, string.Empty, " &*()& ", "  $chool ", "_school_ yay ", " 9^8", "3.@", "7, 9", "1. 8" };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.IsDouble(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void IsDouble_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "3", "3.0", "0.1", "100", "  90.34 ", "3.", "9,8" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.IsDouble(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }

        [Test]
        public void NAME_FIRST_LAST_rejects_invalid_values()
        {
            // Arrange
            var invalidValues = new string[] { "12", "@#$", "0'*'", " -/ ", "Perkins." };

            foreach (var value in invalidValues)
            {
                // Act
                var isValid = EEValidationRules.NAME_FIRST_LAST(value);

                // Assert
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void NAME_FIRST_LAST_accepts_valid_values()
        {
            // Arrange
            var validValues = new string[] { "o'connor", "smyth-FERNWALL ", "  LOOK ", " i have spaces" };

            foreach (var value in validValues)
            {
                // Act
                var isValid = EEValidationRules.NAME_FIRST_LAST(value);

                // Assert
                Assert.IsTrue(isValid);
            }
        }
    }
}