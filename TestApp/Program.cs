using System;
using TestApp.Models;

namespace TestApp
{
    public class Program
    {
        private const int ErrorExitCode = 16;

        public static int Main(string[] args)
        {
            Console.WriteLine($"Started: {DateTime.Now}");
            try
            {
                // Expect to have two command line arguments: input and output file paths
                if ((args?.Length ?? 0) != 2)
                {
                    Console.WriteLine("Please specify paths to input and output data files\n");
                    return ErrorExitCode;
                }

                Console.WriteLine(
                    $"Started: {DateTime.Now} - input data file: {args?[0]} - output data file: {args?[1]}\n");

                // Do all work here
                new QuoteData().ProcessData(args?[0], args?[1]);

                Console.WriteLine($"\n\nFinished: {DateTime.Now}");
            }
            catch (Exception e)
            {
                const string auditMsg = "Failed to run/complete";

                //TODO: Add error logging
                Console.WriteLine($"\n{auditMsg}");
                Console.WriteLine($"\n{e.Message}");
                return ErrorExitCode;
            }

            return 0;
        }
    }
}