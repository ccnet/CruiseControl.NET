using System;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public interface IVssLocale
	{
		string CommentKeyword { get; }
		string CheckedInKeyword { get; }
		string AddedKeyword { get; }
		string DeletedKeyword { get; }
		string DestroyedKeyword { get; }
		string UserKeyword { get; }
		string DateKeyword { get; }
		string TimeKeyword { get; }

		DateTimeFormatInfo CreateDateTimeInfo();
		DateTime ParseDateTime(string date, string time);
		string FormatCommandDate(DateTime date);
		string CultureName { get; set; }
	}
}
