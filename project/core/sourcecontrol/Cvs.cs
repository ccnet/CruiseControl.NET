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
		internal readonly static string HISTORY_COMMAND_FORMAT = @"{0}-q log -N ""-d>{1}""{2}";

		internal readonly static string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";

		private IHistoryParser _parser = new CvsHistoryParser();
		private string _executable = "cvs.exe";
		private string _cvsRoot;
		private string _workingDirectory;
		private bool _labelOnSuccess;
        private string _restrictLogins;

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return _executable;}
			set { _executable = value;}
		}
		
		[ReflectorProperty("cvsroot", Required=false)]
		public string CvsRoot
		{
			get { return _cvsRoot;}
			set { _cvsRoot = value;}
		}
		
		[ReflectorProperty("workingDirectory")]
		public string WorkingDirectory
		{
			get { return _workingDirectory;}
			set { _workingDirectory = value;}
		}		

		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess
		{
			get { return _labelOnSuccess;}
			set { _labelOnSuccess = value;}
		}

        [ReflectorProperty("restrictLogins", Required=false)]
        public string RestrictLogins {
            get{ return _restrictLogins; }
            set{ _restrictLogins = value; }
        }

		[ReflectorProperty("branch", Required=false)]
		public string Branch;

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
			if (LabelOnSuccess)
			{
				string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
				string args = String.Format("{0} tag {1}", cvsroot, "ver-" + label);
				return ProcessUtil.CreateProcess(Executable, args, WorkingDirectory);
			}
			else
			{
				return null;
			}
		}

		internal string BuildHistoryProcessArgs(DateTime from)
		{		
			// in cvs, date 'to' is implicitly now
			// todo: if cvs will accept a 'to' date, it would be nicer to 
			// include that for some harmony with the vss version
			string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
			string branch = (Branch == null) ? String.Empty : " -r" + Branch;
			string args = String.Format(HISTORY_COMMAND_FORMAT, cvsroot, FormatCommandDate(from), branch);
            if (RestrictLogins != null) 
            {
                foreach (string login in RestrictLogins.Split(',')) {
                    args += " -w" + login;
                }
            }
			return args;
		}		
	}
}
