using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
{
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
		// required environment variable name
		internal static readonly string SS_DIR_KEY = "SSDIR";
		
		// ss history [dir] -R -Vd[now]~[lastBuild] -Y[un,pw] -I-Y -O[tempFileName]
		internal static readonly string HISTORY_COMMAND_FORMAT = 
			@"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";

		internal static readonly string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -Vd{2} -Y{3},{4} -I-Y";
		
		private IHistoryParser _parser = new VssHistoryParser();
		
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
		public string SsDir;

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}
		
		public override Process CreateHistoryProcess(DateTime from, DateTime until)
		{		
			string args = BuildHistoryProcessArgs(from, until);
			LogUtil.Log("VSSPublisher", string.Format("{0} {1}", Executable, args));
			Process process = ProcessUtil.CreateProcess(Executable, args);
			process.StartInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			return process;
			
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{ 
			string args = string.Format(LABEL_COMMAND_FORMAT, Project, label, FormatCommandDate(timeStamp), Username, Password);
			Process process = ProcessUtil.CreateProcess(Executable, args);
			process.StartInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			return process;
		}

		internal string FormatCommandDate(DateTime date)
		{
			return string.Concat(date.ToString("d", CultureInfo), ";", date.ToString("t", CultureInfo));
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
