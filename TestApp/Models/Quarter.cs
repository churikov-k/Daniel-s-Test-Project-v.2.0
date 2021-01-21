using System;
using System.Text.RegularExpressions;

namespace TestApp.Models
{
    public class Quarter : IEquatable<Quarter>, IComparable
    {
        private int Year { get; }
        private int QuarterIndex { get; }
        private const string ShorthandRegex = @"Q(?<index>\d)_(?<year>\d\d)";
        public bool IsValid { get; }
        public Quarter(string shorthand)
        {
            IsValid = true;
            var regex = new Regex(ShorthandRegex);
            var groups = regex.Match(shorthand).Groups;

            if (!int.TryParse(groups["index"].Value, out var parseResult))
            {
                IsValid = false;
                return;
            }
            
            QuarterIndex = parseResult;

            if (!int.TryParse(groups["year"].Value, out parseResult))
            {
                IsValid = false;
                return;
            }
            Year = parseResult;
            IsValid = QuarterIndex >= 1 && QuarterIndex <= 4 && Year != 0;
        }

        public override string ToString() => $"Q{QuarterIndex}_{Year:D2}";

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherQuarter = obj as Quarter;
            if (otherQuarter == null)
            {
                throw new ArgumentException("Object is not a Quarter");    
            }

            if (otherQuarter.Year == Year)
            {
                return QuarterIndex - otherQuarter.QuarterIndex;
            }

            return Year - otherQuarter.Year;
        }

        public bool Equals(Quarter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Year == other.Year && QuarterIndex == other.QuarterIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Quarter) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, QuarterIndex);
        }

        public static bool operator ==(Quarter left, Quarter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Quarter left, Quarter right)
        {
            return !Equals(left, right);
        }

        public static Quarter FromDate(DateTime date)
        {
            var quarterIndex = 1 + (date.Month - 1) / 3;
            var year = int.Parse(date.ToString("yy"));
            var shorthand = $"Q{quarterIndex}_{year:D2}";
            return new Quarter(shorthand);
        }
    }
}