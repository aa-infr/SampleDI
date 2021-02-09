using System;

namespace Infrabel.ICT.Framework.Extension
{
    public static class DateTimeOffsetExtensions
    {
        private static readonly string BusinessTimePattern = @"dd/MM/yyyy hh:mm:ss";
        private static readonly string ISO8601TimePattern = @"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz";

        public static string ToBusinessTime(this DateTimeOffset date)
        {
            return date.ToLocalTime().ToString(BusinessTimePattern);
        }

        public static string ToIso8601(this DateTimeOffset date)
        {
            return date.ToString(ISO8601TimePattern);
        }

        public static string ToLocalIso8601(this DateTimeOffset date)
        {
            return date.LocalDateTime.ToString(ISO8601TimePattern);
        }

        public static string ToUtcIso8601(this DateTimeOffset date)
        {
            return date.UtcDateTime.ToString(ISO8601TimePattern);
        }

        public static bool IsBetween(this DateTimeOffset dt, DateTimeOffset dt1, DateTimeOffset dt2)
        {
            return dt >= dt1 && dt <= dt2;
        }
    }
}