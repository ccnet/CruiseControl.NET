using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
{
	// ss history [dir] -R -Vd[now]~[lastBuild] -Y[login] -I-N -O[tempFileName]
	
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
		// required environment variable name
		internal static readonly string SS_DIR_KEY = "SSDIR";
		
		// ss history [dir] -R -Vd[now]~[lastBuild] -Y[un,pw] -I-Y -O[tempFileName]
		internal static readonly string HISTORY_COMMAND_FORMAT = 
			@"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";

		internal static readonly string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -Vd{2} -Y{3},{4} -I-Y";
		
		// 12-31-2002;8:00P
		internal static readonly string DATE_FORMAT = "MM-dd-yyyy;h:mmt";

		private IHistoryParser _parser = new VssHistoryParser();
		
		private string _project;
		private string _username;
		private string _password;		
		private string _ssDir;
		private string _executable = "ss.exe";

		[ReflectorProperty("executable")]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}

		[ReflectorProperty("project")]
		public string Project
		{
			get { return _project; }
			set { _project = value; }
		}

		[ReflectorProperty("username")]
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		[ReflectorProperty("password")]
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		[ReflectorProperty("ssdir", Required=false)]
		public string SsDir
		{
			get { return _ssDir; }
			set { _ssDir = value; }
		}

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}
		
		public override Process CreateHistoryProcess(DateTime from, DateTime until)
		{		
			string args = BuildHistoryProcessArgs(from, until);
			Process process = ProcessUtil.CreateProcess(Executable, args);
			process.StartInfo.EnvironmentVariables[SS_DIR_KEY] = _ssDir;
			return process;
			
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{ 
			string args = String.Format(LABEL_COMMAND_FORMAT, Project, label, FormatCommandDate(timeStamp), Username, Password);
			Process process = ProcessUtil.CreateProcess(Executable, args);
			process.StartInfo.EnvironmentVariables[SS_DIR_KEY] = _ssDir;
			return process;
		}

		internal string FormatCommandDate(DateTime date)
		{
			return date.ToString(DATE_FORMAT);
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{			
			return String.Format(
				HISTORY_COMMAND_FORMAT,
				Project, 
				FormatCommandDate(to),
				FormatCommandDate(from),
				Username, 
				Password);
		}
		
	}
}
