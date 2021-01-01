using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class Parse
    {
        private static readonly Dictionary<string, string> MonthReplacements = new Dictionary<string, string>();

        static Parse()
        {
            MonthReplacements.Add("März", "Mar");
            MonthReplacements.Add("Mär", "Mar");
            MonthReplacements.Add("Mai", "May");
            MonthReplacements.Add("Juni", "Jun");
            MonthReplacements.Add("Juli", "Jul");
            MonthReplacements.Add("Okt", "Oct");
            MonthReplacements.Add("Dez", "Dec");
        }

        public static string ToString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return Prepare(value);
        }

        public static DateTime? ToDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var sanitized = Prepare(value);

            var result = default(DateTime);

            if (DateTime.TryParseExact(sanitized, "dd.MM.yyyy, HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            foreach (var kvp in MonthReplacements)
            {
                sanitized = sanitized.Replace(kvp.Key, kvp.Value);
            }

            if (DateTime.TryParseExact(sanitized, "d MMM yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            Context.GetCurrentLogger().Information("Could not parse value {Sanitized} / {Value} to DateTime", sanitized, value);

            return null;
        }

        public static long? ToLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var sanitized = Prepare(value);

            var result = default(long);

            if (long.TryParse(sanitized, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            Context.GetCurrentLogger().Information("Could not parse value {Sanitized} / {Value} to Long", sanitized, value);

            return null;
        }

        public static decimal? ToDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var sanitized = Prepare(value);

            sanitized = sanitized.Replace(".-", ".00");
            sanitized = sanitized.Replace("'", string.Empty);

            var result = default(decimal);

            if (decimal.TryParse(sanitized, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            Context.GetCurrentLogger().Information("Could not parse value {Sanitized} / {Value} to Decimal", sanitized, value);

            return null;
        }

        private static string Prepare(string value)
        {
            value = value.Trim();
            value = HttpUtility.HtmlDecode(value);
            return value;
        }
    }
}
