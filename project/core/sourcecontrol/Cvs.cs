using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		internal readonly static string HISTORY_COMMAND_FORMAT = @"{0}-q log -N ""-d>{1}""{2}";

		internal readonly static string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";

		private string _executable = "cvs.exe";
		private string _cvsRoot;
		private string _workingDirectory;
		private bool _labelOnSuccess;
        private string _restrictLogins;
		private IUrlBuilder _urlBuilder;
 
		public Cvs() : base(new CvsHistoryParser()) { }
 
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
        public string RestrictLogins 
		{
            get{ return _restrictLogins; }
            set{ _restrictLogins = value; }
        }

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required=false)]
		public IUrlBuilder UrlBuilder
		{
			get { return _urlBuilder; }
			set { _urlBuilder = value; }
		}

		[ReflectorProperty("branch", Required=false)]
		public string Branch;
		
		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] modifications = GetModifications(CreateHistoryProcessInfo(from, to), from, to);
			if ( _urlBuilder != null ) 
			{
				_urlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
			if (LabelOnSuccess)
			{
				Execute(CreateLabelProcessInfo(label, timeStamp));
			}
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			return new ProcessInfo(Executable, BuildHistoryProcessInfoArgs(from), WorkingDirectory);
		}

		public ProcessInfo CreateLabelProcessInfo(string label, DateTime timeStamp) 
		{
			string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
			string args = string.Format("{0} tag {1}", cvsroot, "ver-" + label);
			return new ProcessInfo(Executable, args, WorkingDirectory);
		}

		internal string BuildHistoryProcessInfoArgs(DateTime from)
		{		
			// in cvs, date 'to' is implicitly now
			// todo: if cvs will accept a 'to' date, it would be nicer to 
			// include that for some harmony with the vss version
			string cvsroot = (CvsRoot == null) ? String.Empty : "-d " + CvsRoot + " ";
			string branch = (Branch == null) ? String.Empty : " -r" + Branch;
			string args = string.Format(HISTORY_COMMAND_FORMAT, cvsroot, FormatCommandDate(from), branch);
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
