using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core 
{
	public class LogFile
	{
		public const string FilenamePrefix ="log";
		public const string LogQueryString = "log";
		public const string DateFormat = "yyyyMMddHHmmss";
		public static readonly Regex BuildNumber = new Regex(@"Lbuild\.(\d+)\.xml"); 

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
			Verify(filename);
			return filename.Substring(FilenamePrefix.Length, DateFormat.Length);
		}
		
		public static bool IsSuccessful(string filename)
		{
			int characterIndex = FilenamePrefix.Length + DateFormat.Length;
			return filename[characterIndex] == 'L';
		}
		
		public static string CreateFailedBuildLogFileName(DateTime date)
		{
			return String.Format("{0}{1}.xml", FilenamePrefix,date.ToString(DateFormat));
		}
		
		public static string CreateSuccessfulBuildLogFileName(DateTime date, string label)
		{
			return String.Format("{0}{1}Lbuild.{2}.xml",
				FilenamePrefix,
				date.ToString(DateFormat),
				label
				);
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
			{
				return GetLatestBuildNumber(GetLogFileNames(path));
			}
			else 
			{
				return 0;
			}
		}
		
		public static int GetLatestBuildNumber(string[] filenames)
		{
			int result = 0;
			foreach(string filename in filenames)
			{
				result = Math.Max(result, ParseBuildNumber(filename));
			}
			return result;
		}
		
		public static DateTime GetLastBuildDate(string[] filenames, DateTime defaultValue)
		{
			if (filenames.Length == 0)
			{
				return defaultValue;
			}
			ArrayList.Adapter(filenames).Sort();
			string filename = filenames[filenames.Length-1];
			return ParseForDate(ParseForDateString(filename));			
		}

		public static DateTime GetLastBuildDate(string path, DateTime defaultValue)
		{
			if (Directory.Exists(path))
			{
				return GetLastBuildDate(GetLogFileNames(path), defaultValue);
			}
			else 
			{
				return defaultValue;
			}
		}

		//TODO: refactor other getLatest methods to use this one
		public static string GetLatestLogFileName(string path)
		{
			if (! Directory.Exists(path))
			{
				return null;
			}
			
			string[] filenames = GetLogFileNames(path);
			if (filenames.Length == 0)
			{
				return null;
			}

			ArrayList.Adapter(filenames).Sort();
			return filenames[filenames.Length-1];
		}
		
		public static int ParseBuildNumber(string filename)
		{
			string value = BuildNumber.Match(filename).Groups[1].Value;
			if (value == null || value.Length == 0)
			{
				return 0;
			}
			return Int32.Parse(value);
		}
		
		private static void Verify(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (filename.StartsWith(FilenamePrefix) == false)
			{
				throw new ArgumentException(String.Format(
					"{0} does not start with {1}.", filename, FilenamePrefix));
			}
			if (filename.Length < FilenamePrefix.Length + DateFormat.Length) 
			{
				throw new ArgumentException(String.Format(
					"{0} does not start with {1} followed by a date in {2} format",
					filename, FilenamePrefix, DateFormat));
			}
		}

		public static string CreateUrl(string filename)
		{
			return String.Format("?{0}={1}", LogQueryString, filename);
		}
		
		public static string CreateUrl(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Success)
				return CreateUrl(CreateSuccessfulBuildLogFileName(result.LastModificationDate, result.Label));
			else
				return CreateUrl(CreateFailedBuildLogFileName(result.LastModificationDate));
		}

		public static string CreateUrl(string urlRoot, IntegrationResult result)
		{
			return String.Concat(urlRoot, CreateUrl(result));
		}
	}
}
