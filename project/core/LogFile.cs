using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Provides utility methods for dealing with log files.
	/// </summary>
	public class LogFileUtil
	{
		#region Constants

		public const string FilenamePrefix ="log";
		public const string LogQueryString = "log";
		public const string DateFormat = "yyyyMMddHHmmss";
		public static readonly Regex BuildNumber = new Regex(@"Lbuild\.(.+)\.xml");

		#endregion

		#region Private constructor

		/// <summary>
		/// Utility class, not intended for instantiation.
		/// </summary>
		private LogFileUtil()
		{ }

		#endregion

		#region Extracting data from log filename

		public static string GetFormattedDateString(string filename)
		{
			return DateUtil.FormatDate(ParseForDate(ParseForDateString(filename)));
		}

		public static DateTime ParseForDate(string dateString)
		{
			return DateTime.ParseExact(dateString, DateFormat, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
		}
		
		public static string ParseForDateString(string filename)
		{
			ValidateFilename(filename);
			return filename.Substring(FilenamePrefix.Length, DateFormat.Length);
		}
		
		public static bool IsSuccessful(string filename)
		{
			int characterIndex = FilenamePrefix.Length + DateFormat.Length;
			return filename[characterIndex] == 'L';
		}
		
		public static string ParseBuildNumber(string filename)
		{
			string value = BuildNumber.Match(filename).Groups[1].Value;

			if (value==null || value.Length==0)
				return "0";

			return value;
		}

		#endregion

		#region Creating log file names
		
		public static string CreateFailedBuildLogFileName(DateTime date)
		{
			return string.Format("{0}{1}.xml", FilenamePrefix,date.ToString(DateFormat));
		}
		
		public static string CreateSuccessfulBuildLogFileName(DateTime date, string label)
		{
			return string.Format("{0}{1}Lbuild.{2}.xml",
				FilenamePrefix,
				date.ToString(DateFormat),
				label
				);
		}

		#endregion

		#region Getting all log filenames at a particular path

		public static string[] GetLogFileNames(string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			FileInfo[] files = dir.GetFiles("log*.xml");

			string[] filenames = new string[files.Length];
			for (int i = 0; i < filenames.Length; i++)
			{
				filenames[i] = files[i].Name;
			}
			return filenames;
		}

		#endregion

		#region Getting data about the latest build
		
		public static int GetLatestBuildNumber(string path)
		{
			if (Directory.Exists(path))
				return GetLatestBuildNumber(GetLogFileNames(path));
			else 
				return 0;
		}
		
		public static int GetLatestBuildNumber(string[] filenames)
		{
			int result = 0;
			foreach(string filename in filenames)
			{
				result = Math.Max(result, GetNumericBuildNumber(ParseBuildNumber(filename)));
			}
			return result;
		}

		private static int GetNumericBuildNumber(string buildlabel)
		{
			return Int32.Parse(Regex.Replace(buildlabel, @"\D", ""));
		}

		public static DateTime GetLastBuildDate(string[] filenames, DateTime defaultValue)
		{
			if (filenames.Length == 0)
				return defaultValue;

			ArrayList.Adapter(filenames).Sort();
			string filename = filenames[filenames.Length-1];
			return ParseForDate(ParseForDateString(filename));			
		}

		public static DateTime GetLastBuildDate(string path, DateTime defaultValue)
		{
			if (Directory.Exists(path))
				return GetLastBuildDate(GetLogFileNames(path), defaultValue);
			else 
				return defaultValue;
		}

		// TODO refactor other GetLatest methods to use this one
		public static string GetLatestLogFileName(string path)
		{
			if (!Directory.Exists(path))
				return null;

			string[] filenames = GetLogFileNames(path);
			return GetLatestLogFileName(filenames);
		}

		// TODO refactor other GetLatest methods to use this one
		public static string GetLatestLogFileName(string[] filenames)
		{
			if (filenames.Length==0)
				return null;

			ArrayList.Adapter(filenames).Sort();
			return filenames[filenames.Length-1];
		}

		#endregion

		#region Creating URLs from filenames

		public static string CreateUrl(string filename)
		{
			return string.Format("?{0}={1}", LogQueryString, filename);
		}
		
		public static string CreateUrl(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Success)
				return CreateUrl(CreateSuccessfulBuildLogFileName(result.StartTime, result.Label));
			else
				return CreateUrl(CreateFailedBuildLogFileName(result.StartTime));
		}

		public static string CreateUrl(string urlRoot, IntegrationResult result)
		{
			return String.Concat(urlRoot, CreateUrl(result));
		}

		#endregion

		#region Private helper methods
        		
		/// <summary>
		/// Validates filename structure, throwing exceptions if badly formed.
		/// </summary>
		/// <param name="filename">The filename to validate.</param>
		/// <exception cref="ArgumentNullException">If <see cref="filename"/> is null</exception>
		/// <exception cref="ArgumentException">If <see cref="filename"/> is badly formed</exception>
		private static void ValidateFilename(string filename)
		{
			if (filename==null)
				throw new ArgumentNullException("filename");
			
			if (!filename.StartsWith(FilenamePrefix))
				throw new ArgumentException(string.Format(
					"{0} does not start with {1}.", filename, FilenamePrefix));
			
			if (filename.Length < FilenamePrefix.Length + DateFormat.Length) 
				throw new ArgumentException(string.Format(
					"{0} does not start with {1} followed by a date in {2} format",
					filename, FilenamePrefix, DateFormat));
		}


		#endregion
	}
}
