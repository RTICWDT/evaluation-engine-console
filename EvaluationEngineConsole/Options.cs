// -----------------------------------------------------------------------
// <copyright file="Options.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// A class to model the options that can be passed to the application from the command line.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the file path for the file uploaded by the state user.
        /// </summary>
        [Option("t", "target_file", Required = true, HelpText = "The path for the file you want to process")]
        public string TargetFile { get; set; }

        /// <summary>
        /// Gets or sets the file path for the file that gets passed to the Stata server.
        /// </summary>
        [Option("o", "output_file", DefaultValue = "", HelpText = "The absolute path of the csv file you want to output in the database server")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we want to time the execution of the application.
        /// </summary>
        [Option("w", "time_process", DefaultValue = false, HelpText = "Show in the console how long it takes to process the file")]
        public bool TimeApplication { get; set; }

        /// <summary>
        /// Gets or sets the state to which the target file belongs to.
        /// </summary>
        [Option("s", "state", DefaultValue = "NM", HelpText = "State Abbreviation")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the name of the fields in the file containing the state student id.
        /// </summary>
        [Option("i", "state_id", DefaultValue = "STUDENTID_STATE", HelpText = "The name of the field for the student's state id")]
        public string StateIdField { get; set; }

        /// <summary>
        /// Gets or sets the path to the Excel file that will be returned to the user.
        /// </summary>
        [Option("e", "path_to_feedback_report", Required = true, HelpText = "The path to the csv file that will be returned to the user as feedback. Another csv file, with '_statistics' appended to the filename passed in this argument, will also be generated.")]
        public string ValidationFile { get; set; }

        /// <summary>
        /// Gets or sets additional fields to hash.
        /// </summary>
        [OptionList("h", "hash", Separator = ':', DefaultValue = new string[] { "STUDENTID_DISTRICT", "STUDENTID_SCHOOL" })]
        public IList<string> FieldsToHash { get; set; }

        /// <summary>
        /// Describes how to use the application to the user.
        /// </summary>
        /// <returns>A string that looks pretty in the command line describing how to use the application.</returns>
        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Application to hash ids", "0.0.0.1"),
                Copyright = new CopyrightInfo("MPR INC", 2013),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddOptions(this);

            return help;
        }
    }
}