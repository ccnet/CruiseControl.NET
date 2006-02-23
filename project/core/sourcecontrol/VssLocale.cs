using System;
using System.Globalization;
using System.Resources;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// VssLocale takes responsibility for handling locale-specific parsing of dates and keywords produced by the VSS client (ss.exe)
	/// Keywords are output in the locale of the server and dates are output in the locale of the client, which is, in this case, 
	/// the current culture for the build server.  Generally these two locales will be the same; however, if they are different, users
	/// can specify the server locale using the Vss.Culture property.
	/// </summary>
	public class VssLocale : IVssLocale
	{
		private CultureInfo localCulture;
		private CultureInfo serverCulture;
		private ResourceManager manager;

		public VssLocale(CultureInfo cultureInfo)
		{
			localCulture = serverCulture = cultureInfo;
			manager = new ResourceManager(typeof(VssLocale));
		}

		private string GetKeyword(string key)
		{
			return manager.GetString(key, serverCulture);	
		}

		public string CommentKeyword
		{
			get { return GetKeyword("Comment"); }
		}

		public string CheckedInKeyword
		{
			get { return GetKeyword("CheckedIn"); }
		}

		public string AddedKeyword
		{
			get { return GetKeyword("Added"); }
		}

		public string DeletedKeyword
		{
			get { return GetKeyword("Deleted"); }
		}

		public string DestroyedKeyword
		{
			get { return GetKeyword("Destroyed"); }
		}

		public string UserKeyword
		{
			get { return GetKeyword("User"); }
		}

		public string DateKeyword
		{
			get { return GetKeyword("Date"); }
		}

		public string TimeKeyword
		{
			get { return GetKeyword("Time"); }
		}

		public string ServerCulture
		{
			get { return serverCulture.Name; }
			set { serverCulture = new CultureInfo(value); }
		}

		private DateTimeFormatInfo CreateDateTimeInfo()
		{
			DateTimeFormatInfo dateTimeFormatInfo = localCulture.DateTimeFormat.Clone() as DateTimeFormatInfo;
			dateTimeFormatInfo.AMDesignator = "a";
			dateTimeFormatInfo.PMDesignator = "p";
			return dateTimeFormatInfo;
		}

		public DateTime ParseDateTime(string date, string time)
		{
			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : string.Empty;
			string dateAndTime = string.Format("{0} {1}{2}", date, time, suffix);
			try
			{
				return DateTime.Parse(dateAndTime, CreateDateTimeInfo());				
			}
			catch (FormatException ex)
			{
				throw new CruiseControlException(string.Format("Unable to parse vss date: {0}", dateAndTime), ex);
			}
		}

		/// <summary>
		/// Format the date in a format appropriate for the VSS command-line.  The date should not contain any spaces as VSS would treat it as a separate argument.
		/// The trailing 'M' in 'AM' or 'PM' is also removed.
		/// </summary>
		/// <param name="date"></param>
		/// <returns>Date string formatted for the specified locale as expected by the VSS command-line.</returns>
		public string FormatCommandDate(DateTime date)
		{
			DateTimeFormatInfo info = CreateDateTimeInfo();
			if (info.LongTimePattern.IndexOf('h') >= 0 || info.LongTimePattern.IndexOf('t') >= 0)
			{
				info.LongTimePattern = string.Format("h{0}mmt", info.TimeSeparator);
			}
			else
			{
				info.LongTimePattern = string.Format("H{0}mm", info.TimeSeparator);				
			}
			return string.Concat(date.ToString("d", info), ";", date.ToString(info.LongTimePattern, info));
		}

		public override string ToString()
		{
			return string.Format("VssLocale: local culture = {0}, server culture = {1}", localCulture.DisplayName, serverCulture.DisplayName);
		}

		public override bool Equals(object obj)
		{
			if (obj is IVssLocale) 
				return ((IVssLocale)obj).ServerCulture == ServerCulture;
			return false;
		}

		public override int GetHashCode()
		{
			return serverCulture.GetHashCode();
		}
	}
}
