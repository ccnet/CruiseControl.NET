using System;
using System.Globalization;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		internal static readonly string HISTORY_COMMAND_FORMAT = "log -v -r \"{{{0}}}:{{{1}}}\" --xml --non-interactive {2}";
		internal static readonly string TAG_COMMAND_FORMAT = "copy -m \"CCNET build {0}\" {1} {2}/{0} --non-interactive";
		internal static readonly string GET_SOURCE_COMMAND_FORMAT = "update --non-interactive";

		internal static readonly string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		private string _executable = "svn.exe";
		private string _trunkUrl;
		private string _workingDirectory;
		private bool _tagOnSuccess;
		private string _tagBaseUrl;
		private IModificationUrlBuilder _urlBuilder;

		public Svn(ProcessExecutor executor) : base(new SvnHistoryParser(), executor)
		{
		}

		public Svn() : base(new SvnHistoryParser())
		{
		}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IModificationUrlBuilder UrlBuilder
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

		[ReflectorProperty("workingDirectory")]
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

		[ReflectorProperty("username", Required = false)]
		public string Username;

		[ReflectorProperty("password", Required = false)]
		public string Password;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

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
			Modification[] modifications = ParseModifications(result, from, to);
			if (_urlBuilder != null)
			{
				_urlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
			if (TagOnSuccess)
			{
				Execute(CreateLabelProcessInfo(label, timeStamp));
			}
		}

		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat(HISTORY_COMMAND_FORMAT, FormatCommandDate(from), FormatCommandDate(to), _trunkUrl);
			AppendUsernameAndPassword(buffer);
			return buffer.ToString();
		}

		private void AppendUsernameAndPassword(StringBuilder buffer)
		{
			if (! StringUtil.IsWhitespace(Username)) buffer.AppendFormat(@" --username ""{0}""", Username);
			if (! StringUtil.IsWhitespace(Password)) buffer.AppendFormat(@" --password ""{0}""", Password);
		}

		private string BuildTagProcessArgs(string label)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat(TAG_COMMAND_FORMAT, label, _trunkUrl, _tagBaseUrl);
			AppendUsernameAndPassword(buffer);
			return buffer.ToString();
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				ProcessInfo info = new ProcessInfo(Executable, BuildGetSourceArguments(result.LastChangeNumber), WorkingDirectory);
				Log.Info(string.Format("Getting source from Subversion: {0} {1}", info.FileName, info.Arguments));
				Execute(info);
			}
		}

		private string BuildGetSourceArguments(int revision)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(GET_SOURCE_COMMAND_FORMAT);
			if (revision > 0)
			{
				buffer.Append(" -r");
				buffer.Append(revision);
			}
			AppendUsernameAndPassword(buffer);
			return buffer.ToString();
		}
	}
}