using System;
using System.Globalization;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// TODO: make paths relative to working directory
	/// </summary>
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		internal static readonly string HISTORY_COMMAND_FORMAT = "log -v -r \"{{{0}}}:{{{1}}}\" --xml --non-interactive {2}";
		internal static readonly string TAG_COMMAND_FORMAT = "copy -m \"CCNET build {0}\" \"{1}\" {2}/{0} --non-interactive";
		internal static readonly string GET_SOURCE_COMMAND_FORMAT = "update --non-interactive";
		internal static readonly string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		private string executable = "svn.exe";
		private string trunkUrl;
		private string workingDirectory;
		private bool tagOnSuccess;
		private string tagBaseUrl;
		private IModificationUrlBuilder urlBuilder;

		public Svn(ProcessExecutor executor) : base(new SvnHistoryParser(), executor)
		{}

		public Svn() : base(new SvnHistoryParser())
		{}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IModificationUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return executable; }
			set { executable = value; }
		}

		[ReflectorProperty("trunkUrl")]
		public string TrunkUrl
		{
			get { return trunkUrl; }
			set { trunkUrl = value; }
		}

		[ReflectorProperty("workingDirectory")]
		public string WorkingDirectory
		{
			get { return workingDirectory; }
			set { workingDirectory = value; }
		}

		[ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess
		{
			get { return tagOnSuccess; }
			set { tagOnSuccess = value; }
		}

		[ReflectorProperty("tagBaseUrl", Required = false)]
		public string TagBaseUrl
		{
			get { return tagBaseUrl; }
			set { tagBaseUrl = value; }
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

		public ProcessInfo CreateLabelProcessInfo(IIntegrationResult result)
		{
			return new ProcessInfo(Executable, BuildTagProcessArgs(result.Label, result.LastChangeNumber));
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessResult result = Execute(CreateHistoryProcessInfo(from.StartTime, to.StartTime));
			Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
			if (urlBuilder != null)
			{
				urlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (TagOnSuccess && result.Succeeded)
			{
				Execute(CreateLabelProcessInfo(result));
			}
		}

		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat(HISTORY_COMMAND_FORMAT, FormatCommandDate(from), FormatCommandDate(to), trunkUrl);
			AppendUsernameAndPassword(buffer);
			return buffer.ToString();
		}

		private void AppendUsernameAndPassword(StringBuilder buffer)
		{
			if (! StringUtil.IsWhitespace(Username)) buffer.AppendFormat(@" --username ""{0}""", Username);
			if (! StringUtil.IsWhitespace(Password)) buffer.AppendFormat(@" --password ""{0}""", Password);
		}

		private string BuildTagProcessArgs(string label, int revision)
		{
			StringBuilder buffer = new StringBuilder();

			if (revision == 0)
			{
				buffer.AppendFormat(TAG_COMMAND_FORMAT, label, workingDirectory, tagBaseUrl);
			}
			else
			{
				buffer.AppendFormat(TAG_COMMAND_FORMAT, label, trunkUrl, tagBaseUrl);
				buffer.AppendFormat(" --revision {0}", revision);
			}
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