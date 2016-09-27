// -----------------------------------------------------------------------
// <copyright file="EEValidationRules.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This class encapsulates the validation rules for uploaded files.
    /// </summary>
    public class EEValidationRules
    {
        /// <summary>
        /// The max year for the field ACADYEAR
        /// </summary>
        private static readonly int AcadyearEndYear = 2012;

        /// <summary>
        /// The max year for the field BIRTHDAY
        /// </summary>
        private static readonly int BirthdayEndYear = 2010;

        /// <summary>
        /// An array of valid diploma types.
        /// </summary>
        private static string[] diplomaTypes = new string[] { "00000", "00806", "00807", "00808", "00809", "00810", "00811", "00812", "00813", "00814", "00815", "00816", "00818", "09999", "90000" };

        /// <summary>
        /// An array of valid withdrawal types.
        /// </summary>
        private static string[] withdrawalTypes = new string[] { "00000", "01907", "01908", "01909", "01910", "01911", "01912", "01913", "01914", "01915", "01916", "01917", "01918", "01919", "01921", "01922", "01923", "01924", "01925", "01926", "01927", "01928", "01930", "01931", "03499", "03502", "03503", "03504", "03505", "03508", "03509", "09999", "90000", "01950", "73060", "73061", "79000" };

        /// <summary>
        /// An array of valid grade levels.
        /// </summary>
        private static string[] gradeLevel = new string[] { "IT", "PR", "PK", "TK", "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "PS", "AE", "UG", "OT", "NA" };

        /// <summary>
        /// An array of valid race and ethnicities.
        /// </summary>
        private static string[] raceths = new string[] { "NAT", "ASN", "BLK", "LAT", "HPI", "WHT", "UNK" };

        /// <summary>
        /// An array with four possible values: yes, no, unknown, and missing.
        /// </summary>
        private static string[] yesNoUnknownMissing = new string[] { "Y", "N", "U", "X" };

        /// <summary>
        /// An array with three possible values: yes, no, unknown.
        /// </summary>
        private static string[] yesNoUnknown = new string[] { "Y", "N", "U" };

        /// <summary>
        /// An array with three possible genders: male, female, unkonwn.
        /// </summary>
        private static string[] genders = new string[] { "M", "F", "U" };

        /// <summary>
        /// An array with valid disabilities.
        /// </summary>
        private static string[] disabilities = new string[] { "AUT", "DB", "DD", "EMN", "HI", "MR", "MD", "OI", "OHI", "SLD", "SLI", "TBI", "VI", "NA" };

        /// <summary>
        /// An array with numbers 1 through 9.
        /// </summary>
        private static string[] oneThroughNine = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        /// <summary>
        /// An array with numbers 1, 2, 3, 9
        /// </summary>
        private static string[] oneThroughThreeOrNine = new string[] { "1", "2", "3", "9" };

        /// <summary>
        /// An array with numbers 1 - 5, and 9.
        /// </summary>
        private static string[] oneThroughFiveOrNine = new string[] { "1", "2", "3", "4", "5", "9" };

        /// <summary>
        /// An array with valid values for fields elasbj.
        /// </summary>
        private static string[] elasbj = new string[] { "13372", "13379", "00554", "00560", "13373", "01287", "99999" };

        /// <summary>
        /// An array with valid values for fields mthsbj.
        /// </summary>
        private static string[] mthsbj = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "99" };

        /// <summary>
        /// An array with valid values for eto status
        /// </summary>
        private static string[] eotstatus = new string[] { "1", "2", "8", "9" };

        /// <summary>
        /// A dictionary whose keys are the names of the fields to validate, and
        /// whose values are the predicates used to validate the field's values.
        /// </summary>
        private Dictionary<string, Predicate<string>> rules = new Dictionary<string, Predicate<string>>()
        {
            { "ACADYEAR", ACADYEAR },
            { "GENDER", GENDER },
            { "GPACUM", GPACUM },
            { "GRADELEVEL", GRADELEVEL },
            { "STUDENTID_STATE", STUDENTID_STATE },
            { "STUDENTID_DISTRICT", NoSpecialChars },
            { "STUDENTID_SCHOOL", NoSpecialChars },
            { "NAME_LAST", NAME_FIRST_LAST },
            { "NAME_FIRST", NAME_FIRST_LAST },
            { "NAME_MIDDLE", AlphabeticSpaceOrHyphen },
            { "NAME_SUFFIX", NAME_SUFFIX },
            { "DISTRICTID_NCES_ENROLL", DISTRICTID_NCES_ENROLL },
            { "DISTRICTID_STATE_ENROLL", NoSpecialChars },
            { "DISTRICTNAME_ENROLL", SCHOOL_DISTRICT_NAME_ENROLL_ASSESS },
            { "SCHOOLID_STATE_ENROLL", NoSpecialChars },
            { "SCHOOLNAME_ENROLL", SCHOOL_DISTRICT_NAME_ENROLL_ASSESS },
            { "DISTRICTID_NCES_ASSESS", DISTRICTID_NCES_ENROLL },
            { "DISTRICTID_STATE_ASSESS", NoSpecialChars },
            { "DISTRICTNAME_ASSESS", SCHOOL_DISTRICT_NAME_ENROLL_ASSESS },
            { "SCHOOLID_STATE_ASSESS", NoSpecialChars },
            { "SCHOOLNAME_ASSESS", SCHOOL_DISTRICT_NAME_ENROLL_ASSESS },
            { "SCHOOLID_NCES_ENROLL", SCHOOLID_NCES_ENROLL_ASSESS },
            { "SCHOOLID_NCES_ASSESS", SCHOOLID_NCES_ENROLL_ASSESS }, 
            { "ENRFAY_SCHOOL", ZeroToOneOrNine }, 
            { "ENRFAY_DISTRICT", ZeroToOneOrNine }, 
            { "ENRFAY_STATE", ZeroToOneOrNine }, 
            { "DIPLOMA_TYPE", DIPLOMA_TYPE },
            { "WITHDRAWAL_TYPE", WITHDRAWAL_TYPE },
            { "ATTEND", ZeroToOneOrNine },
            { "RACETH", RACETH },
            { "LATINO", YesNoUnknownMissing },
            { "AMIND", YesNoUnknownMissing },
            { "ASIAN", YesNoUnknownMissing },
            { "BLACK", YesNoUnknownMissing },
            { "PACIFICISLANDER", YesNoUnknownMissing },
            { "WHITE", YesNoUnknownMissing },
            { "BIRTHDATE", BIRTHDATE },
            { "LUNCHFR", YesNoUnknown },
            { "LUNCHF", YesNoUnknown },
            { "LUNCHR", YesNoUnknown },
            { "SPECIALED", YesNoUnknown },
            { "LEP", YesNoUnknown },
            { "RFEP", YesNoUnknownMissing },
            { "MIGRANT", YesNoUnknown },
            { "HOMELESS", YesNoUnknown },
            { "DISABILITY", DISABILITY },
            { "ELA1SCR", BetweenZeroAndTenK },
            { "ELA1PRF", OneThroughNine },
            { "ELA1DAT", ELAMTHDAT },
            { "ELA1SBJ", ELASBJ },
            { "ELA1TYP", OneThroughThreeOrNine },
            { "ELA1VER", OneThroughFiveOrNine },
            { "ELA2SCR", BetweenZeroAndTenK },
            { "ELA2PRF", OneThroughNine },
            { "ELA2DAT", ELAMTHDAT },
            { "ELA2SBJ", ELASBJ },
            { "ELA2TYP", OneThroughThreeOrNine },
            { "ELA2VER", OneThroughFiveOrNine },
            { "ELA3SCR", BetweenZeroAndTenK },
            { "ELA3PRF", OneThroughNine },
            { "ELA3DAT", ELAMTHDAT },
            { "ELA3SBJ", ELASBJ },
            { "ELA3TYP", OneThroughThreeOrNine },
            { "ELA3VER", OneThroughFiveOrNine },
            { "ELA4SCR", BetweenZeroAndTenK },
            { "ELA4PRF", OneThroughNine },
            { "ELA4DAT", ELAMTHDAT },
            { "ELA4SBJ", ELASBJ },
            { "ELA4TYP", OneThroughThreeOrNine },
            { "ELA4VER", OneThroughFiveOrNine },
            { "MTH1SCR", BetweenZeroAndTenK },
            { "MTH1PRF", OneThroughNine },
            { "MTH1DAT", ELAMTHDAT },
            { "MTH1SBJ", MTHSBJ },
            { "MTH1TYP", OneThroughThreeOrNine },
            { "MTH1VER", OneThroughFiveOrNine },
            { "MTH2SCR", BetweenZeroAndTenK },
            { "MTH2PRF", OneThroughNine },
            { "MTH2DAT", ELAMTHDAT },
            { "MTH2SBJ", MTHSBJ },
            { "MTH2TYP", OneThroughThreeOrNine },
            { "MTH2VER", OneThroughFiveOrNine },
            { "MTH3SCR", BetweenZeroAndTenK },
            { "MTH3PRF", OneThroughNine },
            { "MTH3DAT", ELAMTHDAT },
            { "MTH3SBJ", MTHSBJ },
            { "MTH3TYP", OneThroughThreeOrNine },
            { "MTH3VER", OneThroughFiveOrNine },
            { "MTH4SCR", BetweenZeroAndTenK },
            { "MTH4PRF", OneThroughNine },
            { "MTH4DAT", ELAMTHDAT },
            { "MTH4SBJ", MTHSBJ },
            { "MTH4TYP", OneThroughThreeOrNine },
            { "MTH4VER", OneThroughFiveOrNine },
            { "DSCODE_ENRPRV", NoSpecialChars },
            { "DSCODE_ENR1ST", NoSpecialChars },
            { "DSCODE_ENR2ND", NoSpecialChars },
            { "DSCODE_ENR3RD", NoSpecialChars },
            { "GROWTH1SBJ", NoSpecialChars },
            { "GROWTH1SCR", BetweenZeroAndTenK },
            { "GROWTH1DAT", ELAMTHDAT },
            { "GROWTH2SBJ", NoSpecialChars },
            { "GROWTH2SCR", BetweenZeroAndTenK },
            { "GROWTH2DAT", ELAMTHDAT },
            { "GROWTH3SBJ", NoSpecialChars },
            { "GROWTH3SCR", BetweenZeroAndTenK },
            { "GROWTH3DAT", ELAMTHDAT },
            { "GROWTH4SBJ", NoSpecialChars },
            { "GROWTH4SCR", BetweenZeroAndTenK },
            { "GROWTH4DAT", ELAMTHDAT },
            { "MAPLNGSCR", BetweenZeroAndTenK },
            { "MAPLNGDAT", ELAMTHDAT },
            { "MAPMTHSCR", BetweenZeroAndTenK },
            { "MAPMTHDAT", ELAMTHDAT },
            { "MAPREASCR", BetweenZeroAndTenK },
            { "MAPREADAT", ELAMTHDAT },
            { "MAPSCISCR", BetweenZeroAndTenK },
            { "MAPSCIDAT", ELAMTHDAT },
            { "EOT_STATUS", EOT_STATUS }
        };

        /// <summary>
        /// Gets the max value for ACADYEAR.
        /// </summary>
        /// <returns>The max year for ACADYEAR.</returns>
        public static int GetACADYEARendYear()
        {
            return AcadyearEndYear;
        }

        /// <summary>
        /// Gets the max value for BIRTHDAY.
        /// </summary>
        /// <returns>The max year for BIRTHDAY.</returns>
        public static int GetBirthdayEndYear()
        {
            return BirthdayEndYear;
        }

        /// <summary>
        /// Ensures that a value falls between 2010 and acadyearEndYear.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if value falls between 2010 and acadyearEndYear; false otherwise (null or empty strings).</returns>
        public static bool ACADYEAR(string value)
        {
            value = PreprocessValue(value);

            // has correct length?
            if (value.Length != 4)
            {
                return false;
            }

            // is numeric?
            int year;
            var hasYear = int.TryParse(value, out year);
            if (!hasYear)
            {
                return false;
            }

            // is between approved ranges?
            if ((2010 <= year) && (year <= GetACADYEARendYear()))
            {
                return true;
            }

            return false;
        }
 
        /// <summary>
        /// Does value contain only alphanumeric cases? If so, it passes the validation.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if string only contains alphanumeric characters. Null or empty strings are valid. False otherwise.  Note that a string with spaces is not valid.</returns>
        public static bool NoSpecialChars(string value)
        {
            value = PreprocessValue(value);
            var match = Regex.Match(value, @"\W+");
            if (match.Success)
            {
                return false;
            }

            return !value.Contains("_");
        }

        /// <summary>
        /// Does value contain only alphabetic characters, spaces or hyphens? If so, it passes the validation.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if string only contains alphabetic characters, spaces or hyphens. Null or empty strings are valid. False otherwise.</returns>
        public static bool AlphabeticSpaceOrHyphen(string value)
        {
            value = PreprocessValue(value);
            var noHyphen = value.Replace("-", string.Empty);
            return AlphabeticOrSpace(noHyphen);
        }

        /// <summary>
        /// Does value contain only alphabetic characters or spaces? If so, it passes the validation.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if string only contains alphabetic characters or spaces. Null or empty strings are valid. False otherwise.</returns>
        public static bool AlphabeticOrSpace(string value)
        {    
            value = PreprocessValue(value);
            var noSpaces = value.Replace(" ", string.Empty);
            return Alphabetic(noSpaces);
        }

        /// <summary>
        /// Does value contain only alphabetic characters? If so, it passes the validation.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if string only contains alphabetic characters. Null or empty strings are valid. False otherwise.</returns>
        public static bool Alphabetic(string value)
        {
            value = PreprocessValue(value);
            var match = Regex.Match(value, @"[^a-zA-Z]+");
            return !match.Success;
        }

        /// <summary>
        /// Ensures a suffix contains at most four letters, periods or numbers.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value contains at most four letters, periods or numbers; empty fields are valid. False otherwise.</returns>
        public static bool NAME_SUFFIX(string value)
        {
            value = PreprocessValue(value);
            if (value.Length > 4)
            {
                return false;
            }

            return NoSpecialChars(value.Replace(".", string.Empty).Replace("'", string.Empty));
        }

        /// <summary>
        /// Ensures that a string 
        /// 1] has exactly seven characters,
        /// 2] can be parsed as an integer,
        /// 3] when divided by 100000, the result is between 1 and 78.
        /// The range of valid values is from 0100000 to 7899999
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the above conditions are met; false otherwise.</returns>
        public static bool DISTRICTID_NCES_ENROLL(string value)
        {
            value = PreprocessValue(value);

            // correct length
            if (value.Length != 7)
            {
                return false;
            }

            // is numeric?
            int enroll;
            bool hasEnroll = int.TryParse(value, out enroll);
            if (!hasEnroll)
            {
                return false;
            }

            // valid range?
            int cal = enroll / 100000;
            if ((1 <= cal) && (cal <= 78))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ensures that a values fall between 0 and 1 or is 9.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value falls between 0 and 1 or is 9; false otherwise.</returns>
        public static bool ZeroToOneOrNine(string value)
        {
            double expected;
            bool hasExpected = double.TryParse(value, out expected);
            if (!hasExpected)
            {
                return false;
            }

            if ((0 <= expected) && (expected <= 1))
            {
                return true;
            }

            if (expected == 9)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ensures that the value field is numeric or null.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is numeric or null.  False otherwise.</returns>
        public static bool GPACUM(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return IsDouble(value);
        }

        /// <summary>
        /// Checks to see if string can be converted to a double.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the string can be converted to a double.  False otherwise.</returns>
        public static bool IsDouble(string value)
        {
            double expected;
            bool hasExpected = double.TryParse(value, out expected);
            if (!hasExpected)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks to ensure that the value
        /// 1] Has 12 characters
        /// 2] Is numeric
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value has 12 characters and is numeric.  False otherwise.</returns>
        public static bool SCHOOLID_NCES_ENROLL_ASSESS(string value)
        {
            value = PreprocessValue(value);
            if (value.Length != 12)
            {
                return false;
            }

            return IsDouble(value);
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool DIPLOMA_TYPE(string value)
        {
            return diplomaTypes.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool WITHDRAWAL_TYPE(string value)
        {
            return withdrawalTypes.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool GENDER(string value)
        {
            return genders.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool RACETH(string value)
        {
            return raceths.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool YesNoUnknownMissing(string value)
        {
            return yesNoUnknownMissing.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool YesNoUnknown(string value)
        {
            return yesNoUnknown.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Takes a value that has been preposed, and
        /// 1] Ensures it has exactly 8 characters.
        /// 2] If it can covert the first 4 characters into an integer, it puts that int in the first element of the array.
        /// 3] If it can covert the next 2 characters into an integer, it puts that int in the second element of the array.
        /// 4] If it can covert the next 2 characters into an integer, it puts that int in the third element of the array.
        /// This methods barfs if you pass a null value.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>An int array with three elements.  If the value passed is invalid, all three elements will be zero.</returns>
        public static int[] GetYearMonthDay(string value)
        {
            var output = new int[] { 0, 0, 0 };

            if (value.Length != 8)
            {
                return output;
            }

            int year;
            bool hasYear = int.TryParse(value.Substring(0, 4), out year);
            if (!hasYear)
            {
                return output;
            }

            int month;
            bool hasMonth = int.TryParse(value.Substring(4, 2), out month);
            if (!hasMonth)
            {
                return output;
            }

            int day;
            bool hasDay = int.TryParse(value.Substring(6, 2), out day);
            if (!hasDay)
            {
                return output;
            }

            output[0] = year;
            output[1] = month;
            output[2] = day;

            return output;
        }
        
        /// <summary>
        /// Determines if the date represented by a three element int array is valid.
        /// </summary>
        /// <param name="yearMonthDay">The value to be validated.</param>
        /// <returns>True if the date is valid; false otherwise.</returns>
        public static bool IsValidDate(int[] yearMonthDay)
        {
            // validate the date.
            try
            {
                var date = new DateTime(yearMonthDay[0], yearMonthDay[1], yearMonthDay[2]);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Determines if a date falls between 1/1/1990 and 12/31/birthdayEndYear
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value falls between 1/1/1990 and 12/31/birthdayEndYear; false otherwise.</returns>
        public static bool BIRTHDATE(string value)
        {
            var yearMonthDay = GetYearMonthDay(PreprocessValue(value));

            // validate the year.
            if ((yearMonthDay[0] < 1990) || (yearMonthDay[0] > GetBirthdayEndYear()))
            {
                return false;
            }

            return IsValidDate(yearMonthDay);
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool GRADELEVEL(string value)
        {
            return gradeLevel.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool DISABILITY(string value)
        {
            return disabilities.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool OneThroughNine(string value)
        {
            return oneThroughNine.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Ensures that a value is either:
        /// 1] 99999999
        /// 2] or a valid date.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True is value is either 99999999 or a valid date. False otherwise.</returns>
        public static bool ELAMTHDAT(string value)
        {
            value = PreprocessValue(value);
            if (value.Equals("99999999"))
            {
                return true;
            }

            var yearMonthDay = GetYearMonthDay(value);
            return IsValidDate(yearMonthDay);
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool ELASBJ(string value)
        {
            return elasbj.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool MTHSBJ(string value)
        {
            return mthsbj.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool OneThroughThreeOrNine(string value)
        {
            return oneThroughThreeOrNine.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool OneThroughFiveOrNine(string value)
        {
            return oneThroughFiveOrNine.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Determines whether a value falls in the range between 0 and 9999.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value falls between 0 and 9999. False otherwise.</returns>
        public static bool BetweenZeroAndTenK(string value)
        {
            double expected;
            bool hasExpected = double.TryParse(value, out expected);
            if (!hasExpected)
            {
                return false;
            }

            return (0 <= expected) && (expected <= 9999);
        }

        /// <summary>
        /// Ensures that the value
        /// 1] does not contain special characters,
        /// 2] is not null or empty.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value does not contain special character or is null or empty. False otherwise.</returns>
        public static bool STUDENTID_STATE(string value)
        {
            value = PreprocessValue(value);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return NoSpecialChars(value);
        }

        /// <summary>
        /// Ensures that the first or last name
        /// 1] is not null or empty
        /// 2] contains only alphatic characters, spaces, apostrophes or hyphens.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is NOT null or empty and contains only alphabetic characters, spaces, apostrophes or hyphens. False otherwise.</returns>
        public static bool NAME_FIRST_LAST(string value)
        {
            value = PreprocessValue(value).Replace("'", string.Empty);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return AlphabeticSpaceOrHyphen(value);
        }

        /// <summary>
        /// Ensures that values contains only:
        /// 1] Alphanumeric characters,
        /// 2] Hashes,
        /// 3] Spaces,
        /// 4] Hyphens,
        /// 5] Periods.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the string contains alphanumeric characters, hashes, spaces, periods, or hyphens. False otherwise.</returns>
        public static bool SCHOOL_DISTRICT_NAME_ENROLL_ASSESS(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            value = PreprocessValue(value).Replace(".", string.Empty)
                                          .Replace("#", string.Empty)
                                          .Replace(" ", string.Empty)
                                          .Replace("-", string.Empty);

            return NoSpecialChars(value);
        }

        /// <summary>
        /// Values are matched against a list of approved values.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is found the approved list of values. False otherwise.</returns>
        public static bool EOT_STATUS(string value)
        {
            return eotstatus.Contains(PreprocessValue(value));
        }

        /// <summary>
        /// Preprocesses a value to be validated by:
        /// 1] Trimming white space.
        /// 2] Making all characters upper case.
        /// 3] If a value is null, it returns an empty string.
        /// </summary>
        /// <param name="value">The value to preprocess.</param>
        /// <returns>If value is null, then an empty string.  Otherwise, the value trimmed and made upper case.</returns>
        public static string PreprocessValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Trim().ToUpper();
        }

        /// <summary>
        /// Gets a dictionary that contains all the predicates matching the fields in the headers' array. 
        /// </summary>
        /// <param name="headers">A string array with the fields we want to validate.</param>
        /// <returns>A dictionary (keys: name of field; value: prediates to validate field's values)</returns>
        public Dictionary<int, Predicate<string>> GetValidationRules(string[] headers)
        {
            Dictionary<int, Predicate<string>> outputRules = new Dictionary<int, Predicate<string>>();
            int counter = 0;
            foreach (var header in headers)
            {
                var tempheader = header.ToUpper().Trim();
                if (this.rules.Keys.Contains(tempheader))
                {
                    outputRules.Add(counter, this.rules[tempheader]);
                }

                counter++;
            }
            
            return outputRules;
        }
    }
}
