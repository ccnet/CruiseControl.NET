
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	public class LogFile
	{
		public const string FilenamePrefix = "log";
		public const string FilenameDateFormat = "yyyyMMddHHmmss";
		public static readonly Regex BuildNumber = new Regex(@"Lbuild\.(.+)\.xml");

		private readonly DateTime _date;
		private readonly string _label;
		private readonly bool _succeeded;
		private readonly IFormatProvider _formatter = CultureInfo.CurrentCulture;

		public LogFile(string filename)
		{
			ValidateFilename(filename);
			_date = ParseDate(filename);
			_label = ParseLabel(filename);
			_succeeded = IsSuccessful(filename);
		}

		public LogFile(string filename, IFormatProvider formatter) : this(filename)
		{
			_formatter = formatter;
		}

		public LogFile(IIntegrationResult result)
		{
			_date = result.StartTime;
			_label = result.Label;
			_succeeded = result.Succeeded;
		}

		public DateTime Date
		{
			get { return _date; }
		}

		public string FormattedDateString
		{
			get { return DateUtil.FormatDate(_date, _formatter); }
		}

		public string Label
		{
			get { return _label; }
		}

		public bool Succeeded
		{
			get { return _succeeded; }
		}

		public string Filename
		{
			get { return (_succeeded) ? CreateSuccessfulBuildLogFileName() : CreateFailedBuildLogFileName(); }
		}

		private string CreateFailedBuildLogFileName()
		{
			return string.Format("{0}{1}.xml", FilenamePrefix, FilenameFormattedDateString);
		}

		private string CreateSuccessfulBuildLogFileName()
		{
			return string.Format("{0}{1}Lbuild.{2}.xml", FilenamePrefix, FilenameFormattedDateString, _label);
		}

		public string FilenameFormattedDateString
		{
			get { return _date.ToString(FilenameDateFormat); }
		}

		/// <summary>
		/// Validates filename structure, throwing exceptions if badly formed.
		/// </summary>
		/// <param name="filename">The filename to validate.</param>
        /// <exception cref="ArgumentNullException">If filename is null</exception>
        /// <exception cref="ArgumentException">If filename is badly formed</exception>
		private void ValidateFilename(string filename)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");

            if (!filename.StartsWith(FilenamePrefix))
                throw new ArgumentException(string.Format(
                                                "{0} does not start with {1}.", filename, FilenamePrefix), "filename");

			if (filename.Length < FilenamePrefix.Length + FilenameDateFormat.Length)
				throw new ArgumentException(string.Format(
					"{0} does not start with {1} followed by a date in {2} format",
                    filename, FilenamePrefix, FilenameDateFormat), "filename");
		}

		private DateTime ParseDate(string filename)
		{
			string dateString = filename.Substring(FilenamePrefix.Length, FilenameDateFormat.Length);
			return DateTime.ParseExact(dateString, FilenameDateFormat, _formatter);
		}

		private string ParseLabel(string filename)
		{
			string value = BuildNumber.Match(filename).Groups[1].Value;
			if (value == null || value.Length == 0)
				return "0";

			return value;
		}

		private bool IsSuccessful(string filename)
		{
			int characterIndex = FilenamePrefix.Length + FilenameDateFormat.Length;
			return filename[characterIndex] == 'L';
		}
	}

	/// <summary>
	/// Provides utility methods for dealing with log files.
	/// </summary>
	public class LogFileUtil
	{
		public const string LogQueryString = "log";
		public const string ProjectQueryString = "project";

		/// <summary>
		/// Utility class, not intended for instantiation.
		/// </summary>
		private LogFileUtil()
		{
		}

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
			foreach (string filename in filenames)
			{
				result = Math.Max(result, GetNumericBuildNumber(new LogFile(filename).Label));
			}
			return result;
		}

		private static int GetNumericBuildNumber(string buildlabel)
		{
			return Int32.Parse(Regex.Replace(buildlabel, @"\D", string.Empty));
		}

		public static DateTime GetLastBuildDate(string[] filenames, DateTime defaultValue)
		{
			if (filenames.Length == 0)
				return defaultValue;

			ArrayList.Adapter(filenames).Sort();
			string filename = filenames[filenames.Length - 1];
			return new LogFile(filename).Date;
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
			if (filenames.Length == 0)
				return null;

			ArrayList.Adapter(filenames).Sort();
			return filenames[filenames.Length - 1];
		}

		public static string CreateUrl(string filename)
		{
			return string.Format("?{0}={1}", LogQueryString, filename);
		}

		public static string CreateUrl(string filename, string projectname)
		{
			return string.Format("{0}&{1}={2}", CreateUrl(filename), ProjectQueryString, projectname);
		}

		public static string CreateUrl(IIntegrationResult result)
		{
			return CreateUrl(new LogFile(result).Filename);
		}

		public static string CreateUrl(string urlRoot, IIntegrationResult result)
		{
			return String.Concat(urlRoot, CreateUrl(result));
		}
	}
}