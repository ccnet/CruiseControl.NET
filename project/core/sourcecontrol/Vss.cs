using System;
using System.IO;
using System.Collections;
using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
		// required environment variable name
		internal const string SS_DIR_KEY = "SSDIR";
		internal const string SS_REGISTRY_PATH = @"Software\\Microsoft\\SourceSafe";
		internal const string SS_REGISTRY_KEY = "SCCServerPath";
		internal const string SS_EXE = "ss.exe";

		// ss history [dir] -R -Vd[now]~[lastBuild] -Y[un,pw] -I-Y -O[tempFileName]
		internal static readonly string HISTORY_COMMAND_FORMAT = @"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";

		internal static readonly string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -Vd{2} -Y{3},{4} -I-Y";
		internal static readonly string LABEL_COMMAND_FORMAT_NOTIMESTAMP = @"label {0} -L{1} -Y{2},{3} -I-Y";

		private string _ssDir;
		private string _executable;

		public CultureInfo CultureInfo = CultureInfo.CurrentCulture;

		public Vss(): base(new VssHistoryParser())
		{
		}

		public Vss(IHistoryParser historyParser, ProcessExecutor executor): base(historyParser, executor)
		{
		}

		[ReflectorProperty("executable", Required = false)] 
		public string Executable
		{
			get
			{
				if (_executable == null)
					_executable = GetExecutable(new Registry());
				return _executable;
			}
			set { _executable = value; }
		}

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("username")]
		public string Username;

		[ReflectorProperty("password")]
		public string Password;

		[ReflectorProperty("ssdir", Required = false)] 
		public string SsDir
		{
			get { return _ssDir; }
			set { _ssDir = value.Trim('"'); }
		}

		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] modifications = GetModifications(CreateHistoryProcessInfo(from, to), from, to);
			if (modifications.Length > 0 && ApplyLabel)
			{
				string label = "CCNETUNVERIFIED" + to.ToString("MMddyyyyHHmmss");
				LabelSourceControl(label, to);
			}
			return modifications;
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
			if (ApplyLabel)
			{
				Execute(CreateLabelProcessInfo(label, timeStamp));
			}
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime until)
		{
			return CreateProcessInfo(BuildHistoryProcessInfoArgs(from, until));
		}

		public ProcessInfo CreateLabelProcessInfo(string label)
		{
			return CreateProcessInfo(string.Format(LABEL_COMMAND_FORMAT_NOTIMESTAMP, Project, label, Username, Password));
		}

		public ProcessInfo CreateLabelProcessInfo(string label, DateTime timeStamp)
		{
			return CreateProcessInfo(string.Format(LABEL_COMMAND_FORMAT, Project, label, FormatCommandDate(timeStamp), Username, Password));
		}

		private ProcessInfo CreateProcessInfo(string args)
		{
			Log.Debug(string.Format("VSS: {0} {1}", Executable, args));
			ProcessInfo ProcessInfo = new ProcessInfo(Executable, args);
			if (SsDir != null)
			{
				ProcessInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return ProcessInfo;
		}

		/// <summary>
		/// Format the date in a format appropriate for the VSS command-line.  The date should not contain any spaces as VSS would treat it as a separate argument.
		/// The trailing 'M' in 'AM' or 'PM' is also removed.
		/// </summary>
		/// <param name="date"></param>
		/// <returns>Date string formatted for the specified locale as expected by the VSS command-line.</returns>
		internal string FormatCommandDate(DateTime date)
		{
			return string.Concat(date.ToString("d", CultureInfo), ";", date.ToString("t", CultureInfo)).Replace(" ", string.Empty).TrimEnd('M', 'm');
		}

		internal string BuildHistoryProcessInfoArgs(DateTime from, DateTime to)
		{
			return string.Format(HISTORY_COMMAND_FORMAT, Project, FormatCommandDate(to), FormatCommandDate(from), Username, Password);
		}

		internal string GetExecutable(IRegistry registry)
		{
			string comServerPath = registry.GetLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
			return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
		}
	}
}