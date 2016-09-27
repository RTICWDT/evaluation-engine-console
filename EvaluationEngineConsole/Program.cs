// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="MPR INC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace EvaluationEngineConsole
{
    using System;
    using CommandLine;

    /// <summary>
    /// Entry point to the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The options to run the application.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser cliParser = new CommandLineParser();
            if (cliParser.ParseArguments(args, options))
            {
                var data = new DataProvider();
                var dispatcher = new Dispatcher(options, data);
                dispatcher.Dispatch();
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
        }
    }
}