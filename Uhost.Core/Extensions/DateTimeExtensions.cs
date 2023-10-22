using System;
using System.Globalization;

namespace Uhost.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public const string DateDbFmt = "yyyy-MM-dd";
        public const string DateApiFmt = "dd-MM-yyyy";
        public const string DateTimeApiFmt = "dd-MM-yyyy HH:mm:ss";
        public const string DateTimeFileFmt = "yyyyMMdd-HHmmss";
        public const string DateTimeLogFmt = "yyyy-MM-dd HH:mm:ss.fff";
        public const string DateTimeHumanFmt = "dd.MM.yyyy HH:mm";

        private static readonly string[] _dateTimeFormats = new[]
        {
            DateTimeApiFmt,
            DateApiFmt,
            DateDbFmt,
            DateTimeFileFmt,
            DateTimeHumanFmt
        };

        public static string ToApiFmt(this DateTime dt) => dt.ToString(DateTimeApiFmt);

        public static string ToLogFmt(this DateTime dt) => dt.ToString(DateTimeLogFmt);

        /// <summary>
        /// Время для имени файла
        /// </summary>
        public static string ToFileFmt(this DateTime dt) => dt.ToString(DateTimeFileFmt);

        public static string ToHumanFmt(this DateTime dt) => dt.ToString(DateTimeHumanFmt);

        public static string ToHumanFmt(this TimeSpan ts)
        {
            string str = string.Empty;

            if (ts.TotalDays > 1)
            {
                str += $"{(int)ts.TotalDays} ";
            }
            if (ts.TotalHours > 1)
            {
                str += $"{ts.Hours}:";
            }

            str += $"{ts.Minutes.ToString().PadLeft(2, '0')}:{ts.Seconds.ToString().PadLeft(2, '0')}";

            return str;
        }

        /// <summary>
        /// Парсим строку с датой и временем
        /// </summary>
        public static bool TryParseDateTime(this string dtStr, out DateTime parsed)
        {
            foreach (string dtFmt in _dateTimeFormats)
            {
                if (DateTime.TryParseExact(dtStr, dtFmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    return true;
                }
            }

            parsed = default;
            return false;
        }
    }
}
