using Exortech.NetReflector;
using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		internal readonly static string HISTORY_COMMAND_FORMAT = "log -v -r \"{{{0}}}:{{{1}}}\" --xml {2}";
		internal readonly static string TAG_COMMAND_FORMAT = "copy -m \"CCNET build {0}\" {1} {2}";

		internal readonly static string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		private string _executable = "svn.exe";
		private string _trunkUrl;
		private string _workingDirectory;
		private bool _tagOnSuccess;
		private string _tagBaseUrl;
		private IUrlBuilder _urlBuilder;

		public Svn(ProcessExecutor executor) : base(new SvnHistoryParser(), executor) { }

		public Svn(): base (new SvnHistoryParser())
		{
		}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IUrlBuilder UrlBuilder
		{
			get { return _urlBuilder; }
			set { _urlBuilder = value; }
		}

		[ReflectorProperty("executable")] 
		public string Executable
		{
			get { return _executable; }
			set { _executable = value; }
		}

		[ReflectorProperty("trunkUrl")] 
		public string TrunkUrl
		{
			get { return _trunkUrl; }
			set { _trunkUrl = value; }
		}

		[ReflectorProperty("workingDirectory", Required = false)] 
		public string WorkingDirectory
		{
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		[ReflectorProperty("tagOnSuccess", Required = false)] 
		public bool TagOnSuccess
		{
			get { return _tagOnSuccess; }
			set { _tagOnSuccess = value; }
		}

		[ReflectorProperty("tagBaseUrl", Required = false)] 
		public string TagBaseUrl
		{
			get { return _tagBaseUrl; }
			set { _tagBaseUrl = value; }
		}

		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			return new ProcessInfo(Executable, BuildHistoryProcessArgs(from, to), WorkingDirectory);
		}

		public ProcessInfo CreateLabelProcessInfo(string label, DateTime timeStamp)
		{
			return new ProcessInfo(Executable, BuildTagProcessArgs(label));
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			ProcessResult result = Execute(CreateHistoryProcessInfo(from, to));
			return ParseModifications(result, from, to);
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
			if (TagOnSuccess)
			{
				Execute(CreateLabelProcessInfo(label, timeStamp));
			}
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			return string.Format(HISTORY_COMMAND_FORMAT, FormatCommandDate(from), FormatCommandDate(to), _trunkUrl);
		}

		internal string BuildTagProcessArgs(string label)
		{
			string tagUrl = _tagBaseUrl + "/" + label;
			return string.Format(TAG_COMMAND_FORMAT, label, _trunkUrl, tagUrl);
		}
	}
}