using System;
using System.Globalization;

namespace soen390_team01.Data
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Returns a DateTime's string representation using the local time zone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateTimeLocalString(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local).ToString(CultureInfo.InvariantCulture);
        }

    }
}
