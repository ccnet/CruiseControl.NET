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
	}

	public class EnglishVssLocale : IVssLocale
	{
		private CultureInfo cultureInfo;

		public EnglishVssLocale() : this(CultureInfo.InvariantCulture) {}

		public EnglishVssLocale(CultureInfo cultureInfo)
		{
			this.cultureInfo = cultureInfo;
		}

		public string CommentKeyword
		{
			get { return "Comment"; }
		}

		public string CheckedInKeyword
		{
			get { return "Checked in"; }
		}

		public string AddedKeyword
		{
			get { return "added"; }
		}

		public string DeletedKeyword
		{
			get { return "deleted"; }
		}

		public string DestroyedKeyword
		{
			get { return "destroyed"; }
		}

		public string UserKeyword
		{
			get { return "User"; }
		}

		public string DateKeyword
		{
			get { return "Date"; }
		}

		public string TimeKeyword
		{
			get { return "Time"; }
		}

		public DateTimeFormatInfo CreateDateTimeInfo()
		{
			DateTimeFormatInfo dateTimeFormatInfo = cultureInfo.DateTimeFormat.Clone() as DateTimeFormatInfo;
			dateTimeFormatInfo.AMDesignator = "a";
			dateTimeFormatInfo.PMDesignator = "p";
			return dateTimeFormatInfo;
		}

		public DateTime ParseDateTime(string date, string time)
		{
			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : String.Empty;
			string dateAndTime = string.Format("{0};{1}{2}", date, time, suffix);
			return DateTime.Parse(dateAndTime, CreateDateTimeInfo());
		}
	}

	public class FrenchVssLocale : IVssLocale
	{
		private CultureInfo cultureInfo;

		public FrenchVssLocale() : this(new CultureInfo("fr-FR")) {}

		public FrenchVssLocale(CultureInfo cultureInfo)
		{
			this.cultureInfo = cultureInfo;
		}

		public string CommentKeyword
		{
			get { return "Commentaire"; }
		}

		public string CheckedInKeyword
		{
			get { return "Archivé dans"; }
		}

		public string AddedKeyword
		{
			get { return "ajouté"; }
		}

		public string DeletedKeyword
		{
			get { return "supprimé"; }
		}

		public string DestroyedKeyword
		{
			get { return "détruit"; }
		}

		public string UserKeyword
		{
			get { return "Utilisateur"; }
		}

		public string DateKeyword
		{
			get { return "Date"; }
		}

		public string TimeKeyword
		{
			get { return "Heure"; }
		}

		public DateTimeFormatInfo CreateDateTimeInfo()
		{
			return cultureInfo.DateTimeFormat.Clone() as DateTimeFormatInfo;
		}

		public DateTime ParseDateTime(string date, string time)
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
