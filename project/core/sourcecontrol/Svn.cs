using System;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		internal readonly static string HISTORY_COMMAND_FORMAT = "log -v -r \"{{{0}}}:{{{1}}}\" --xml {2}";
		internal readonly static string TAG_COMMAND_FORMAT = "copy {0} {1}";

		internal readonly static string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		private IHistoryParser _parser = new SvnHistoryParser();

		private string _executable = "svn.exe";
		private string _trunkUrl;
		private string _workingDirectory;
		private bool _tagOnSuccess;
		private string _tagBaseUrl;

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return _executable;}
			set { _executable = value;}
		}
		
		[ReflectorProperty("trunkUrl")]
		public string TrunkUrl {
			get { return _trunkUrl;}
			set { _trunkUrl = value;}
		}

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory
		{
			get { return _workingDirectory;}
			set { _workingDirectory = value;}
		}		

		[ReflectorProperty("tagOnSuccess", Required=false)]
		public bool TagOnSuccess
		{
			get { return _tagOnSuccess;}
			set { _tagOnSuccess = value;}
		}

		[ReflectorProperty("tagBaseUrl", Required=false)]
		public string TagBaseUrl {
			get { return _tagBaseUrl;}
			set { _tagBaseUrl = value;}
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
			return ProcessUtil.CreateProcess(Executable, BuildHistoryProcessArgs(from, to), WorkingDirectory);
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{
			return ProcessUtil.CreateProcess(Executable, BuildTagProcessArgs(label));
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{		
			return string.Format(HISTORY_COMMAND_FORMAT, FormatCommandDate(from), FormatCommandDate(to), _trunkUrl);
		}		

		internal string BuildTagProcessArgs(string label) {
			string tagUrl = _tagBaseUrl + "/" + label;
			return string.Format(TAG_COMMAND_FORMAT, _trunkUrl, tagUrl);
		}
	}
}
