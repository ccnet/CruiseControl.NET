using System;
using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace tw.ccnet.core
{

	[ReflectorType("logFile")]
	public class LogFileIncrementer : ILabelIncrementer
	{

		private static readonly Regex BuildNumber = new Regex(@"Lbuild\.([\w\d\.]+)\.xml");

		private string path;

		public LogFileIncrementer() 
		{
		}

		public LogFileIncrementer(string path) 
		{
			this.path = path;
		}


		[ReflectorProperty("logDir")]
		public string LogDir 
		{
			get { return path; }
			set { path = value; }
		}

		public string GetNextLabel() 
		{
			string [] filenames = LogFile.GetLogFileNames(path);
			return getNextLabelFromFileNames(filenames);
		}

		internal string getNextLabelFromFileNames(string[] filenames) 
		{
			string result = String.Empty;
			foreach(string filename in filenames)
			{
				result = max(result, parseBuildNumber(filename));
			}
			int dotIndex = result.LastIndexOf(".");
			string lastPart = result;
			if (dotIndex > -1) 
			{
				lastPart = result.Substring(dotIndex + 1);
			}
			try 
			{
				int buildNumber = Int32.Parse(lastPart) + 1;
				if (dotIndex > -1)
					result = result.Substring(0, dotIndex) + ".";
				else
					result = String.Empty;
				result += buildNumber.ToString();
			} 
			catch (Exception) 
			{
				// nothing to do here, if we can't increment then just return what we have
			}
			return result;
		}

		private string parseBuildNumber(string filename)
		{
			string buildNumber = BuildNumber.Match(filename).Groups[1].Value;
			if (buildNumber == null)
			{
				return String.Empty;;
			}
			return buildNumber;
		}

		private string max(string lhs, string rhs) 
		{
			return String.Compare(lhs, rhs) > 0 ? lhs : rhs;
		}
	}
}
