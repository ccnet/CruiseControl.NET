using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Helper class for dates
    /// </summary>
	public class DateUtil
	{
		// Format dates according to ISO 8601 format
		public const string DateOutputFormat = "yyyy-MM-dd HH:mm:ss";


        /// <summary>
        /// formats the date to ISO 8601 format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
		public static string FormatDate(DateTime date)
		{
			return date.ToString(DateOutputFormat);
		}

        /// <summary>
        /// formats the date to ISO 8601 format, using the specified formatter
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
		public static string FormatDate(DateTime date, IFormatProvider formatter)
		{
			return date.ToString(DateOutputFormat, formatter);
		}

        /// <summary>
        /// Returns the largest of the 2 passed dates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
		public static DateTime MaxDate(DateTime a, DateTime b)
		{
			return (a > b) ? a : b;
		}

		/// <summary>
		/// Convert a Unix timestamp into a DateTime object with local time.
		/// </summary>
		/// <param name="timestamp">The unix timestamp to convert.</param>
		/// <returns>A DateTime object in local time.</returns>
		public static DateTime ConvertFromUnixTimestamp(double timestamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
		}

		/// <summary>
		/// Convert a DateTime object into a unix timestamp.
		/// </summary>
		/// <param name="dateTime">The DateTime object to convert.</param>
		/// <returns>The unix timestamp in UTC time.</returns>
		public static double ConvertToUnixTimestamp(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}
	}
}