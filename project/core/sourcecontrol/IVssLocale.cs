using System;

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

		DateTime ParseDateTime(string date, string time);
		string FormatCommandDate(DateTime date);
		string ServerCulture { get; set; }
	}
}
