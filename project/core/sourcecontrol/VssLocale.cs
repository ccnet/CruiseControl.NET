using System;
using System.Globalization;
using System.Resources;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class VssLocale : IVssLocale
	{
		private CultureInfo cultureInfo;
		private ResourceManager manager;

		public VssLocale() : this(CultureInfo.InvariantCulture) { }

		public VssLocale(CultureInfo cultureInfo)
		{
			this.cultureInfo = cultureInfo;
			manager = new ResourceManager(typeof(VssLocale));
		}

		private string GetKeyword(string key)
		{
			return manager.GetString(key, cultureInfo);	
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

		public string CultureName
		{
			get { return cultureInfo.Name; }
			set { cultureInfo = new CultureInfo(value); }
		}

		public virtual DateTimeFormatInfo CreateDateTimeInfo()
		{
			DateTimeFormatInfo dateTimeFormatInfo = cultureInfo.DateTimeFormat.Clone() as DateTimeFormatInfo;
			dateTimeFormatInfo.AMDesignator = "a";
			dateTimeFormatInfo.PMDesignator = "p";
			return dateTimeFormatInfo;
		}

		public virtual DateTime ParseDateTime(string date, string time)
		{
			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : String.Empty;
			string dateAndTime = string.Format("{0};{1}{2}", date, time, suffix);
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
			string timePattern = info.LongTimePattern;
			bool containsAMPM = StringUtil.Contains(timePattern, "t");
			if (StringUtil.Contains(timePattern, "h") || containsAMPM)
			{
				timePattern = timePattern.Replace("H", "h");
				timePattern = timePattern.Replace("hh", "h");
				if (! containsAMPM) 
					timePattern += 't';
			}
			else
			{
				timePattern = timePattern.Replace("HH", "H");				
			}
			return string.Concat(date.ToString("d", info), ";", date.ToString(timePattern, info));
		}

		public override string ToString()
		{
			return string.Format("VssLocale culture: {0}", cultureInfo.Name);
		}

		public override bool Equals(object obj)
		{
			if (obj is VssLocale) 
				return ((VssLocale)obj).cultureInfo.Name == cultureInfo.Name;
			return false;
		}

		public override int GetHashCode()
		{
			return cultureInfo.GetHashCode();
		}
	}
}
