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
		public const string TO_PVCS_DATE_FORMAT = "MM/dd/yyyy/HH:mm";
		public const string PVCS_INSTRUCTIONS_FILE = "CruiseControlPVCS.pcli";
 		public const string PVCS_PRETEMPFILE = "pvcspretemp.txt";
 		public const string PVCS_TEMPFILE = "pvcstemp.txt";
		public const string PVCS_LOGOUTPUT_FILE = "pvcsout.txt";

		const string PRE_INSTRUCTIONS_TEMPLATE = 
@"set -vProject ""{0}""
set -vSubProject ""{1}""
run ->{2} listversionedfiles -z -aw $Project $SubProject
";		
		const string POST_INSTRUCTIONS_TEMPLATE = 
			@"run -e vlog  ""-xo+e{0}"" ""-d{1}*{2}"" ""@{3}"" ";

//		const string INSTRUCTIONS_TEMPLATE = @"set -vProject ""{0}""
//set -vSubProject ""{1}""
//run ->{2} listversionedfiles -z -aw $Project $SubProject
//run -e vlog  ""-xo+e{3}"" ""-d{4}*{5}"" ""@{2}""
//";

		private TimeZone _currentTimeZone = TimeZone.CurrentTimeZone;

		public Pvcs() : this(new PvcsHistoryParser(), new ProcessExecutor()) { }

		public Pvcs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor) { }

		[ReflectorProperty("executable")]
		public string Executable = "pcli";

		[ReflectorProperty("arguments", Required=false)]
		public string Arguments = COMMAND;

//		[ReflectorProperty("instructions", Required=false)]
//		public string Instructions = INSTRUCTIONS_TEMPLATE;

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("subproject")]
		public string Subproject;

		public TimeZone CurrentTimeZone
		{
			set { _currentTimeZone = value; }
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			CreateVersionedFileList();
			TransformVersionedFileList();
			ExecuteVLog(from, to);
			return ParseModifications(from, to);
		}

		private void CreateVersionedFileList()
		{
			Execute(CreatePcliContentsForGeneratingPvcsTemp());
		}

		private void ExecuteVLog(DateTime from, DateTime to)
		{
			// required due to DayLightSavings bug in PVCS 7.5.1
			from = AdjustForDayLightSavingsBug(from);
			to = AdjustForDayLightSavingsBug(to);

			Execute(CreatePcliContentsForCreatingVLog(from.ToString(TO_PVCS_DATE_FORMAT), to.ToString(TO_PVCS_DATE_FORMAT)));			
		}

		private void Execute(string pcliContent)
		{
			CreatePVCSInstructionFile(pcliContent);
			Execute(CreatePVCSProcessInfo());			
		}

		private void CreatePVCSInstructionFile(string content)
		{
			using (StreamWriter stream = File.CreateText(PVCS_INSTRUCTIONS_FILE))
			{
				stream.Write(content);
			}			
		}

		internal ProcessInfo CreatePVCSProcessInfo()
		{
			Log.Debug(string.Format("Pvcs: {0} {1}", Executable, Arguments));
			return new ProcessInfo(Executable, Arguments);
		}

		internal string CreatePcliContentsForGeneratingPvcsTemp()
		{
			return string.Format(PRE_INSTRUCTIONS_TEMPLATE, Project, Subproject, PVCS_PRETEMPFILE);
		}

		public string CreatePcliContentsForCreatingVLog(string beforedate, string afterdate)
		{
			return string.Format(POST_INSTRUCTIONS_TEMPLATE, PVCS_LOGOUTPUT_FILE, beforedate, afterdate, PVCS_TEMPFILE);
		}

		private TextReader GetTextReader(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader(stream);
		}

		internal DateTime AdjustForDayLightSavingsBug(DateTime date)
		{
			if (_currentTimeZone.IsDaylightSavingTime(DateTime.Now))
			{
				TimeSpan anHour = new TimeSpan(1, 0, 0);
				return date.Subtract(anHour);
			}
			return date;
		}

		private Modification[] ParseModifications(DateTime from, DateTime to)
		{
			using (TextReader reader = GetTextReader(PVCS_LOGOUTPUT_FILE))
			{
				return ParseModifications(reader, from, to);
			}
		}

		private void TransformVersionedFileList()
		{
			using (TextReader reader = GetTextReader(PVCS_PRETEMPFILE))
			{
				using (TextWriter writer = File.CreateText(PVCS_TEMPFILE))
				{
					TransformVersionedFileList(reader, writer);
				}
			}
		}

		internal void TransformVersionedFileList(TextReader reader, TextWriter writer)
		{
			while (reader.Peek() != -1)
			{
				string line = reader.ReadLine();
				if (line.StartsWith(@"""\\"))
				{
					writer.Write(@"""\\\");
					writer.WriteLine(line.Substring(3));
				}
			}
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
		}
	}
}