using System;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
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
		
		public override Process CreateHistoryProcess(DateTime from, DateTime to)
		{
			string content = CreatePcliContents(
				from.ToString(TO_PVCS_DATE_FORMAT),
				to.ToString(TO_PVCS_DATE_FORMAT)
			);

			StreamWriter stream = File.CreateText(PVCS_INSTRUCTIONS_FILE);
			stream.Write(content);
			stream.Close();
			
			return ProcessUtil.CreateProcess(Executable, Arguments);
		}
		
		public string CreatePcliContents(string beforedate, string afterdate) 
		{
			return String.Format(
				Instructions, 
				Project, Subproject, PVCS_TEMPFILE, PVCS_LOGOUTPUT_FILE, beforedate, afterdate
			);
		}
		
		protected override TextReader Execute(Process process)
		{
			process.Start();	
			process.WaitForExit(Timeout);
			return ProcessUtil.GetTextReader(PVCS_LOGOUTPUT_FILE);
		}
		
	}
}
