using System;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		internal readonly static string HISTORY_COMMAND_FORMAT = "{0}-q log -N \"-d>{1}\"";

		internal readonly static string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";

		private IHistoryParser _parser = new CvsHistoryParser();
		private string _executable = "cvs.exe";
		private string _cvsRoot;
		private string _workingDirectory;

		[ReflectorProperty("executable")]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}
		
		[ReflectorProperty("cvsroot", Required=false)]
		public string CvsRoot
		{
			get{ return _cvsRoot;}
			set{ _cvsRoot = value;}
		}
		
		[ReflectorProperty("workingDirectory")]
		public string WorkingDirectory
		{
			get{ return _workingDirectory;}
			set{ _workingDirectory = value;}
		}		

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}
		
		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT);
		}

		public override Process CreateHistoryProcess(DateTime from, DateTime to)
		{
			return ProcessUtil.CreateProcess(Executable, BuildHistoryProcessArgs(from), WorkingDirectory);
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{
			string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
			string args = String.Format("{0} tag {1}", cvsroot, "ver-" + label);
			return ProcessUtil.CreateProcess(Executable, args, WorkingDirectory);
		}

		internal string BuildHistoryProcessArgs(DateTime from)
		{		
			// in cvs, date 'to' is implicitly now
			// todo: if cvs will accept a 'to' date, it would be nicer to 
			// include that for some harmony with the vss version
			string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
			string args = String.Format(HISTORY_COMMAND_FORMAT, 
				cvsroot,
				FormatCommandDate(from));
			return args;
		}
		
	}
}
