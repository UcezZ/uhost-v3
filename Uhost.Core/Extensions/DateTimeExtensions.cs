using System;
using System.Globalization;

namespace Uhost.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public const string DateToDbFmt = "yyyy-MM-dd";
        public const string DateToApiFmt = "dd-MM-yyyy";
        public const string DateTimeToApiFmt = "dd-MM-yyyy HH:mm:ss";
        public const string DateTimeToFileFmt = "yyyyMMdd-HHmmss";
        public const string DateTimeToLogFmt = "HH:mm:ss.fff";
        public const string TimeToApiFmt = "HH:mm";

        private static readonly string[] _dateTimeFormats = new[]
        {
            DateTimeToApiFmt,
            DateToApiFmt,
            DateToDbFmt,
            DateTimeToFileFmt
        };

        public static string ToApiFmt(this DateTime dt) => dt.ToString(DateTimeToApiFmt);

        public static string ToLogFmt(this DateTime dt) => dt.ToString(DateTimeToLogFmt);

        /// <summary>
        /// Время для имени файла
        /// </summary>
        public static string ToFileFmt(this DateTime dt) => dt.ToString(DateTimeToFileFmt);


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
