using System;
using System.Globalization;
using System.Resources;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.Core.sourcecontrol
{
	public abstract class VssLocale : IVssLocale
	{
		protected CultureInfo cultureInfo;
		private ResourceManager manager;

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

		public abstract DateTimeFormatInfo CreateDateTimeInfo();

		public abstract DateTime ParseDateTime(string date, string time);
	}

	public class EnglishVssLocale : VssLocale
	{
		public EnglishVssLocale() : base(CultureInfo.InvariantCulture) {}

		public EnglishVssLocale(CultureInfo cultureInfo) : base(cultureInfo) {}

		public override DateTimeFormatInfo CreateDateTimeInfo()
		{
			DateTimeFormatInfo dateTimeFormatInfo = cultureInfo.DateTimeFormat.Clone() as DateTimeFormatInfo;
			dateTimeFormatInfo.AMDesignator = "a";
			dateTimeFormatInfo.PMDesignator = "p";
			return dateTimeFormatInfo;
		}

		public override DateTime ParseDateTime(string date, string time)
		{
			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : String.Empty;
			string dateAndTime = string.Format("{0};{1}{2}", date, time, suffix);
			return DateTime.Parse(dateAndTime, CreateDateTimeInfo());
		}
	}

	public class FrenchVssLocale : VssLocale
	{
		public FrenchVssLocale() : base(new CultureInfo("fr-FR")) {}
		
		public FrenchVssLocale(CultureInfo cultureInfo) : base(cultureInfo) {}

		public override DateTimeFormatInfo CreateDateTimeInfo()
		{
			return cultureInfo.DateTimeFormat.Clone() as DateTimeFormatInfo;
		}

		public override DateTime ParseDateTime(string date, string time)
		{
			string dateAndTime = string.Format("{0};{1}", date, time);
			return DateTime.Parse(dateAndTime, CreateDateTimeInfo());
		}
	}

	public class VssLocaleFactory
	{
		public static IVssLocale Create()
		{
			CultureInfo culture = CultureInfo.CurrentCulture;
			if (culture.TwoLetterISOLanguageName == "fr")
			{
				return new FrenchVssLocale(culture);
			}
			else
			{
				return new EnglishVssLocale(culture);
			}
		}
	}
}
