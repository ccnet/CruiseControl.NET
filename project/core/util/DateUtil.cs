using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class DateUtil
	{
		// Format dates according to ISO 8601 format
		public const string DateOutputFormat = "yyyy-MM-dd HH:mm:ss";

		public static string FormatDate(DateTime date)
		{
			return date.ToString(DateOutputFormat);
		}

		public static string FormatDate(DateTime date, IFormatProvider formatter)
		{
			return date.ToString(DateOutputFormat, formatter);
		}

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