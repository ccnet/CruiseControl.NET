using System;

namespace tw.ccnet.core.util
{
	public class DateUtil
	{
		public const string DateOutputFormat = "dd MMM yyyy HH:mm";

		public static string FormatDate(DateTime date)
		{
			return date.ToString(DateOutputFormat);
		}

		public static DateTime MaxDate(DateTime a, DateTime b)
		{
			return (a > b) ? a : b;
		}
	}
}
