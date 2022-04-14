using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TestApp.Models
{
    public class QuoteData
    {
        private const string HeaderTemplate = "ObservationDate,Shorthand,From,To,Price";
        private List<InputDataModel> InputData { get; set; }
        private List<string> OutputData { get; set; }

        public QuoteData()
        {
            InputData = new List<InputDataModel>();
            OutputData = new List<string>();
        }

        public void ProcessData(string inputPath, string outputPath)
        {
            ReadData(inputPath);
            TransformData();
            SaveData(outputPath);
        }

        private void ReadData(string path)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), path);

            var rawData = File.ReadAllLines(file);
            var hasCorrectHeader = rawData.Take(1).Where(i => string.Equals(i, HeaderTemplate)).Select(i => i).Count() == 1;
            if (!hasCorrectHeader)
            {
                throw new InvalidDataException("Input file has wrong header.");
            }

            InputData = rawData.Skip(1).Select(ConvertLine).Where(i => i != null).ToList();
        }

        // Transform the input data from the tabular format to the grid format
        private void TransformData()
        {
            // Get input data grouped and ordered by ObservationDate 
            var groupedByDate = InputData
                .GroupBy(line => line.ObservationDate,
                    line => new InnerGroupModel { Shorthand = line.Shorthand, Price = line.Price })
                .Select(lineGroup => new ProcessingDataModel
                {
                    ObservationDate = lineGroup.Key, InnerGroup = lineGroup.OrderBy(i => i.Shorthand).ToList()
                })
                .OrderBy(i => i.ObservationDate);

            // Get ordered list of all unique shorthands
            var shorthandList = InputData.Select(i => i.Shorthand).Distinct().OrderBy(i => i).ToList();

            // Add the title line
            var result = new List<string> { $",{string.Join(",", shorthandList.Select(i => i))}" };
            // Add all transformed data
            // It can be made via loops if complex LINQ expressions are not acceptable 
            result.AddRange(from line in groupedByDate
                let prices = shorthandList.Select(shorthand =>
                        line.InnerGroup.FirstOrDefault(i => i.Shorthand == shorthand)?.Price)
                    .Aggregate(new StringBuilder(), (current, price) => current.Append($",{price:F4}")).ToString()
                select $"{line.ObservationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}{prices}");

            OutputData = result;
        }

        private void SaveData(string outputPath)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

            File.WriteAllLines(file, OutputData);
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