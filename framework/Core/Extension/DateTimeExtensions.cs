using System;
using System.Globalization;

namespace Infrabel.ICT.Framework.Extension
{
    public static class DateTimeExtensions
    {
        private static readonly string BusinessDatePattern = @"dd/MM/yyyy";
        private static readonly string BusinessTimePattern = @"dd/MM/yyyy hh:mm:ss";
        private static readonly string ISO8601TimePattern = @"o";
        private static readonly string LiteralTimePattern = @"yyyy-MMddHHmmssfff";

        public static bool IsBetween(this DateTime dt, DateTime dt1, DateTime dt2)
        {
            return dt >= dt1 && dt <= dt2;
        }

        public static string ToBusinessDate(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = ToCategorizedDateTime(date, unspecifiedAsUtc);
            return result.Kind == DateTimeKind.Local ? result.ToString(BusinessDatePattern) : result.ToLocalTime().ToString(BusinessDatePattern);
        }

        public static string ToBusinessTime(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = ToCategorizedDateTime(date, unspecifiedAsUtc);
            return result.Kind == DateTimeKind.Local ? result.ToString(BusinessTimePattern) : result.ToLocalTime().ToString(BusinessTimePattern);
        }

        public static string ToIso8601(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToCategorizedDateTime(unspecifiedAsUtc);

            return result.ToString(ISO8601TimePattern);
        }

        public static string ToLiteral(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToCategorizedDateTime(unspecifiedAsUtc);
            return result.ToString(LiteralTimePattern, CultureInfo.InvariantCulture);
        }

        public static DateTime? ToLocalDateTime(this DateTime? date, bool unspecifiedAsUtc)
        {
            if (!date.HasValue)
                return null;
            return ToLocalDateTime(date.Value, unspecifiedAsUtc);
        }

        public static DateTime ToLocalDateTime(this DateTime date, bool unspecifiedAsUtc)
        {
            return ToCategorizedDateTime(date, unspecifiedAsUtc).ToLocalTime();
        }

        public static string ToLocalIso8601(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToLocalDateTime(unspecifiedAsUtc);

            return result.ToString(ISO8601TimePattern);
        }

        public static string ToLocalIso8601(this DateTime? date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToLocalDateTime(unspecifiedAsUtc);

            return result?.ToString(ISO8601TimePattern);
        }

        public static DateTime ToUtcDateTime(this DateTime date, bool unspecifiedAsUtc)
        {
            return ToCategorizedDateTime(date, unspecifiedAsUtc).ToUniversalTime();
        }

        public static DateTime? ToUtcDateTime(this DateTime? date, bool unspecifiedAsUtc)
        {
            if (!date.HasValue)
                return null;
            return ToUtcDateTime(date.Value, unspecifiedAsUtc);
        }

        public static string ToUtcIso8601(this DateTime date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToUtcDateTime(unspecifiedAsUtc);

            return result.ToString(ISO8601TimePattern);
        }

        public static string ToUtcIso8601(this DateTime? date, bool unspecifiedAsUtc = true)
        {
            var result = date.ToUtcDateTime(unspecifiedAsUtc);

            return result?.ToString(ISO8601TimePattern);
        }

        private static DateTime ToCategorizedDateTime(this DateTime date, bool unspecifiedAsUtc)
        {
            var result = date;
            if (date.Kind != DateTimeKind.Unspecified)
                return result;

            result = unspecifiedAsUtc ? new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc)
                                      : new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Local);

            return result;
        }
    }
}