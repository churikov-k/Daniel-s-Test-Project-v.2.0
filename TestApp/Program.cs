using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                DoWork(args?[0], args?[1]);

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

        public static void DoWork(string inputPath, string outputPath)
        {
            var inputData = ReadData(inputPath);
            var resultData = TransformData(inputData);
            SaveData(resultData, outputPath);
        }

        private static void SaveData(IEnumerable<string> resultData, string outputPath)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

            File.WriteAllLines(file, resultData);
        }

        // Transform the input data from the tabular format to the grid format
        private static List<string> TransformData(List<InputDataModel> inputData)
        {
            // Get input data grouped and ordered by ObservationDate 
            var groupedByDate = inputData
                .GroupBy(line => line.ObservationDate,
                    line => new InnerGroupModel {Shorthand = line.Shorthand, Price = line.Price})
                .Select(lineGroup => new ProcessingDataModel
                {
                    ObservationDate = lineGroup.Key, InnerGroup = lineGroup.OrderBy(i => i.Shorthand).ToList()
                })
                .OrderBy(i => i.ObservationDate);

            // Get ordered list of all unique shorthands
            var shorthandList = inputData.Select(i => i.Shorthand).Distinct().OrderBy(i => i).ToList();

            // Add the title line
            var result = new List<string> {$",{string.Join(",", shorthandList.Select(i => i))}"};
            // Add all transformed data
            // It can be made via loops if complex LINQ expressions are not acceptable 
            result.AddRange(from line in groupedByDate
                let prices = shorthandList.Select(shorthand =>
                        line.InnerGroup.FirstOrDefault(i => i.Shorthand == shorthand)?.Price)
                    .Aggregate("", (current, price) => current + $",{price:F4}")
                select $"{line.ObservationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}{prices}");

            return result;
        }

        private static List<InputDataModel> ReadData(string path)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), path);

            var rawData = File.ReadAllLines(file);

            var inputData = rawData.Skip(1).Select(ConvertLine).Where(i => i != null).ToList();

            return inputData;
        }

        // Convert string line from the input file
        // into InputDataModel with checking ObservationDate and Shorthand.
        // If ObservationDate not present
        // or Shorthand not present and cannot be restored 
        // then return null to skip this line from further processing
        private static InputDataModel ConvertLine(string line)
        {
            var items = line.Split(',');

            // Skip not formerly formatted line 
            if (items.Length != 5) return null;

            var rawObservationDate = items[0];
            var rawShorthand = items[1];
            var rawFrom = items[2];
            var rawTo = items[3];
            var rawPrice = items[4];

            var observationDate = rawObservationDate.ToDate();
            // There is no way to get observationDate from another data
            if (observationDate == null) return null;

            var shorthand = new Quarter(rawShorthand);
            // Try to get shorthand from To or From date
            if (!shorthand.IsValid)
            {
                var from = rawFrom.ToDate();
                var to = rawTo.ToDate();
                var dateToTry = from ?? to;
                if (dateToTry == null) return null;

                shorthand = Quarter.FromDate(dateToTry.Value);
                // Skip line if not valid
                if (!shorthand.IsValid) return null;
            }

            // Uses null to leave missed price empty
            decimal? price = null;
            if (decimal.TryParse(rawPrice, out decimal parsedPrice))
            {
                price = parsedPrice;
            }

            return new InputDataModel
            {
                ObservationDate = observationDate.Value,
                Shorthand = shorthand,
                Price = price
            };
        }
    }
}