using Exortech.NetReflector;
using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pvcs")]
	public class Pvcs : ProcessSourceControl
	{
		public const string COMMAND = "run -sCruiseControlPVCS.pcli";
		const string TO_PVCS_DATE_FORMAT = "MM/dd/yyyy/HH:mm";
		public const string PVCS_INSTRUCTIONS_FILE = "CruiseControlPVCS.pcli";
		const string PVCS_TEMPFILE = "pvcstemp.txt";
		public const string PVCS_LOGOUTPUT_FILE = "pvcsout.txt";
		
		const string INSTRUCTIONS_TEMPLATE = 
@"set -vProject ""{0}""
set -vSubProject ""{1}""
run ->{2} listversionedfiles -z -aw $Project $SubProject
run -e vlog  ""-xo+e{3}"" ""-d{4}*{5}"" ""@{2}""
";
		

		private IHistoryParser _parser = new PvcsHistoryParser();
		private string _executable = "pcli";
		private string _project;
		private string _subproject;
		private string _arguments = COMMAND;
		private string _instructions = INSTRUCTIONS_TEMPLATE;
		

		[ReflectorProperty("executable")]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}
		
		[ReflectorProperty("arguments", Required=false)]
		public string Arguments 
		{
			get{ return _arguments;}
			set{ _arguments = value;}
		}

		[ReflectorProperty("instructions", Required=false)]
		public string Instructions 
		{
			get{ return _instructions;}
			set{ _instructions = value;}
		}
		
		[ReflectorProperty("project")]
		public string Project
		{
			get{ return _project;}
			set{ _project = value;}
		}
		
		[ReflectorProperty("subproject")]
		public string Subproject
		{
			get{ return _subproject;}
			set{ _subproject = value;}
		}		

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}
		
		public override ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			// required due to DayLightSavings bug in PVCS 7.5.1
			if (IsDayLightSavings()) 
			{
				from = SubtractAnHour(from);
				to = SubtractAnHour(to);
			}

			string content = CreatePcliContents(
				from.ToString(TO_PVCS_DATE_FORMAT),
				to.ToString(TO_PVCS_DATE_FORMAT)
			);

			StreamWriter stream = File.CreateText(PVCS_INSTRUCTIONS_FILE);
			stream.Write(content);
			stream.Close();

			Log.Debug(string.Format("Pvcs: {0} {1}", Executable, Arguments));
			return new ProcessInfo(Executable, Arguments);
		}

		public override ProcessInfo CreateLabelProcessInfo(string label, DateTime timeStamp) 
		{
			return null;
		}
		
		public string CreatePcliContents(string beforedate, string afterdate) 
		{
			return string.Format(
				Instructions, 
				Project, Subproject, PVCS_TEMPFILE, PVCS_LOGOUTPUT_FILE, beforedate, afterdate
			);
		}
		
		protected override ProcessResult Execute(ProcessInfo processInfo)
		{
			ProcessExecutor executor = new ProcessExecutor();
			executor.Timeout = Timeout;
			ProcessResult result = executor.Execute(processInfo);
			return new ProcessResult(GetTextReader(PVCS_LOGOUTPUT_FILE).ReadToEnd(), result.StandardError, result.ExitCode, result.TimedOut);
		}

		public static TextReader GetTextReader(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader(stream);
		}

		public Boolean IsDayLightSavings() 
		{
			TimeZone tz = TimeZone.CurrentTimeZone;
			return tz.IsDaylightSavingTime(DateTime.Now);
		}

		public DateTime SubtractAnHour(DateTime date) 
		{
			TimeSpan anHour = new TimeSpan(1, 0, 0);
			return date.Subtract(anHour);
		}
		
	}
}
