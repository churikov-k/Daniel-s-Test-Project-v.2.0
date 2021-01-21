using System;
using System.Globalization;

namespace TestApp
{
    public static class Extensions
    {
        // Extension method parsing a date string to a DateTime?
        public static DateTime? ToDate(this string dateTimeStr)
        {
            string[] dateFmt = { "d/M/yyyy","dd/M/yyyy","d/MM/yyyy","dd/MM/yyyy","MM/d/yyyy","dd.MM.yyyy","dd.M.yyyy","d.MM.yyyy","MM.dd.yyyy"};
            var result = DateTime.TryParseExact(dateTimeStr, dateFmt, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, out var dt) ? dt : null as DateTime?;
            return result;
        }
    }
}