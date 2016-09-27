// -----------------------------------------------------------------------
// <copyright file="EEFields.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole.ExpectedFields
{
    using System.Collections.Generic;

    /// <summary>
    /// A class for managing the expected and optional list of fields.
    /// </summary>
    public class EEFields
    {
        /// <summary>
        /// A string array with the fields we expect to find in a file uploaded by a state (this includes optional fields).
        /// </summary>
        private static string[] expectedFields = new string[] { "ACADYEAR", "STUDENTID_STATE", "STUDENTID_DISTRICT", "STUDENTID_SCHOOL", "NAME_LAST", "NAME_FIRST", "NAME_MIDDLE", "NAME_SUFFIX", "DISTRICTID_NCES_ENROLL", "DISTRICTID_STATE_ENROLL", "DISTRICTNAME_ENROLL", "SCHOOLID_NCES_ENROLL", "SCHOOLID_STATE_ENROLL", "SCHOOLNAME_ENROLL", "DISTRICTID_NCES_ASSESS", "DISTRICTID_STATE_ASSESS", "DISTRICTNAME_ASSESS", "SCHOOLID_NCES_ASSESS", "SCHOOLID_STATE_ASSESS", "SCHOOLNAME_ASSESS", "ENRFAY_SCHOOL", "ENRFAY_DISTRICT", "ENRFAY_STATE", "DIPLOMA_TYPE", "WITHDRAWAL_TYPE", "ATTEND", "GPACUM", "GENDER", "RACETH", "LATINO", "AMIND", "ASIAN", "BLACK", "PACIFICISLANDER", "WHITE", "BIRTHDATE", "GRADELEVEL", "LUNCHFR", "LUNCHF", "LUNCHR", "SPECIALED", "DISABILITY", "LEP", "RFEP", "MIGRANT", "HOMELESS", "ELA1SCR", "ELA1PRF", "ELA1DAT", "ELA1SBJ", "ELA1NAM", "ELA1TYP", "ELA1VER", "ELA2SCR", "ELA2PRF", "ELA2DAT", "ELA2SBJ", "ELA2NAM", "ELA2TYP", "ELA2VER", "ELA3SCR", "ELA3PRF", "ELA3DAT", "ELA3SBJ", "ELA3NAM", "ELA3TYP", "ELA3VER", "ELA4SCR", "ELA4PRF", "ELA4DAT", "ELA4SBJ", "ELA4NAM", "ELA4TYP", "ELA4VER", "MTH1SCR", "MTH1PRF", "MTH1DAT", "MTH1SBJ", "MTH1NAM", "MTH1TYP", "MTH1VER", "MTH2SCR", "MTH2PRF", "MTH2DAT", "MTH2SBJ", "MTH2NAM", "MTH2TYP", "MTH2VER", "MTH3SCR", "MTH3PRF", "MTH3DAT", "MTH3SBJ", "MTH3NAM", "MTH3TYP", "MTH3VER", "MTH4SCR", "MTH4PRF", "MTH4DAT", "MTH4SBJ", "MTH4NAM", "MTH4TYP", "MTH4VER", "DSCODE_ENRPRV", "DSCODE_ENR1ST", "DSCODE_ENR2ND", "DSCODE_ENR3RD", "GROWTH1SBJ", "GROWTH1SCR", "GROWTH1DAT", "GROWTH2SBJ", "GROWTH2SCR", "GROWTH2DAT", "GROWTH3SBJ", "GROWTH3SCR", "GROWTH3DAT", "GROWTH4SBJ", "GROWTH4SCR", "GROWTH4DAT", "MAPLNGSCR", "MAPLNGDAT", "MAPMTHSCR", "MAPMTHDAT", "MAPREASCR", "MAPREADAT", "MAPSCISCR", "MAPSCIDAT" };

        /// <summary>
        /// A string array of optional fields. 
        /// </summary>
        private static string[] optionalFields = new string[] { "GROWTH1SBJ", "GROWTH1SCR", "GROWTH1DAT", "GROWTH2SBJ", "GROWTH2SCR", "GROWTH2DAT", "GROWTH3SBJ", "GROWTH3SCR", "GROWTH3DAT", "GROWTH4SBJ", "GROWTH4SCR", "GROWTH4DAT", "MAPLNGSCR", "MAPLNGDAT", "MAPMTHSCR", "MAPMTHDAT", "MAPREASCR", "MAPREADAT", "MAPSCISCR", "MAPSCIDAT" };

        /// <summary>
        /// Gets a HashSet with the names of the fields we expect to find in a file uploaed by a state. 
        /// </summary>
        /// <returns>A HashSet with the names of the fields we expect to find in a file uploaed by state.</returns>
        public static HashSet<string> GetSetOfExpectedFields()
        {
            return new HashSet<string>(expectedFields);
        }

        /// <summary>
        /// Gets a HashSet with the names of optional fields.
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetSetOfOptionalFields()
        {
            return new HashSet<string>(optionalFields);
        }
    }
}
