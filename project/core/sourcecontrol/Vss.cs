using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
		// required environment variable name
		internal const string SS_DIR_KEY = "SSDIR";
		
		// ss history [dir] -R -Vd[now]~[lastBuild] -Y[un,pw] -I-Y -O[tempFileName]
		internal static readonly string HISTORY_COMMAND_FORMAT = 
			@"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";

		internal static readonly string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -Vd{2} -Y{3},{4} -I-Y";
		internal static readonly string LABEL_COMMAND_FORMAT_NOTIMESTAMP = @"label {0} -L{1} -Y{2},{3} -I-Y";
		
		private IHistoryParser _parser = new VssHistoryParser();
		private string _ssDir;
		
		public CultureInfo CultureInfo = CultureInfo.CurrentCulture;

		[ReflectorProperty("executable")]
		public string Executable = "ss.exe";

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("username")]
		public string Username;

		[ReflectorProperty("password")]
		public string Password;

		[ReflectorProperty("ssdir", Required=false)]
		public string SsDir
		{
			get { return _ssDir; }
			set { _ssDir = value.Trim('"'); }
		}

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] result = base.GetModifications (from, to);
			if (result.Length > 0)
			{
				Process p = CreateLabelProcess("CCNETUNVERIFIED" + to.ToString("MMddyyyyHHmmss"));
				Execute(p);
			}

			return result;
		}
		
		public override Process CreateHistoryProcess(DateTime from, DateTime until)
		{		
			return CreateProcess(BuildHistoryProcessArgs(from, until));
		}

		public Process CreateLabelProcess(string label) 
		{
			return CreateProcess(string.Format(LABEL_COMMAND_FORMAT_NOTIMESTAMP, Project, label, Username, Password));
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{ 
			return CreateProcess(string.Format(LABEL_COMMAND_FORMAT, Project, label, FormatCommandDate(timeStamp), Username, Password));
		}

		private Process CreateProcess(string args)
		{
			Log.Debug(string.Format("VSS: {0} {1}", Executable, args));
			Process process = ProcessUtil.CreateProcess(Executable, args);
			if (SsDir != null)
			{
				process.StartInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return process;
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

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{			
			return string.Format(
				HISTORY_COMMAND_FORMAT,
				Project, 
				FormatCommandDate(to),
				FormatCommandDate(from),
				Username, 
				Password);
		}
	}
}
