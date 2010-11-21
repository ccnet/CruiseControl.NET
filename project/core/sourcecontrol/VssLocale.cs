using System;
using System.Globalization;
using System.Resources;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="VssLocale" /> class.	
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        /// <remarks></remarks>
		public VssLocale(CultureInfo cultureInfo)
		{
			localCulture = serverCulture = cultureInfo;
			manager = new ResourceManager(typeof(VssLocale));
		}

		private string GetKeyword(string key)
		{
			return manager.GetString(key, serverCulture);	
		}

        /// <summary>
        /// Gets the comment keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string CommentKeyword
		{
			get { return GetKeyword("Comment"); }
		}

        /// <summary>
        /// Gets the checked in keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string CheckedInKeyword
		{
			get { return GetKeyword("CheckedIn"); }
		}

        /// <summary>
        /// Gets the added keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string AddedKeyword
		{
			get { return GetKeyword("Added"); }
		}

        /// <summary>
        /// Gets the deleted keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string DeletedKeyword
		{
			get { return GetKeyword("Deleted"); }
		}

        /// <summary>
        /// Gets the destroyed keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string DestroyedKeyword
		{
			get { return GetKeyword("Destroyed"); }
		}

        /// <summary>
        /// Gets the user keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string UserKeyword
		{
			get { return GetKeyword("User"); }
		}

        /// <summary>
        /// Gets the date keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string DateKeyword
		{
			get { return GetKeyword("Date"); }
		}

        /// <summary>
        /// Gets the time keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string TimeKeyword
		{
			get { return GetKeyword("Time"); }
		}

        /// <summary>
        /// Gets or sets the server culture.	
        /// </summary>
        /// <value>The server culture.</value>
        /// <remarks></remarks>
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

        /// <summary>
        /// Parses the date time.	
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public DateTime ParseDateTime(string date, string time)
		{
			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : string.Empty;
			string dateAndTime = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} {1}{2}", date, time, suffix);
			try
			{
				return DateTime.Parse(dateAndTime, CreateDateTimeInfo());				
			}
			catch (FormatException ex)
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to parse vss date: {0}", dateAndTime), ex);
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
				info.LongTimePattern = string.Format(System.Globalization.CultureInfo.CurrentCulture,"h{0}mmt", info.TimeSeparator);
			}
			else
			{
				info.LongTimePattern = string.Format(System.Globalization.CultureInfo.CurrentCulture,"H{0}mm", info.TimeSeparator);				
			}
			return string.Concat(date.ToString("d", info), ";", date.ToString(info.LongTimePattern, info));
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"VssLocale: local culture = {0}, server culture = {1}", localCulture.DisplayName, serverCulture.DisplayName);
		}

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override bool Equals(object obj)
		{
            var dummy = obj as IVssLocale;
			if (dummy != null) 
				return dummy.ServerCulture == ServerCulture;
			return false;
		}

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override int GetHashCode()
		{
			return serverCulture.GetHashCode();
		}
	}
}
