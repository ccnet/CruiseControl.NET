using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class DateUtil
	{
		public const string DateOutputFormat = "dd MMM yyyy HH:mm:ss";

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
