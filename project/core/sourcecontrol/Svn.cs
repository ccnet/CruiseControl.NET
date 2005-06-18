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
		public const string DefaultExecutable = "svn.exe";
		internal static readonly string HISTORY_COMMAND_FORMAT = "log -v -r \"{{{0}}}:{{{1}}}\" --xml --non-interactive {2}";
		internal static readonly string TAG_COMMAND_FORMAT = "copy -m \"CCNET build {0}\" \"{1}\" {2}/{0} --non-interactive";
		internal static readonly string GET_SOURCE_COMMAND_FORMAT = "update --non-interactive";
		internal static readonly string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		public Svn(ProcessExecutor executor) : base(new SvnHistoryParser(), executor)
		{}

		public Svn() : base(new SvnHistoryParser())
		{}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IModificationUrlBuilder UrlBuilder;

		[ReflectorProperty("executable")]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("trunkUrl")]
		public string TrunkUrl;

		[ReflectorProperty("workingDirectory")]
		public string WorkingDirectory;

		[ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess;

		[ReflectorProperty("tagBaseUrl", Required = false)]
		public string TagBaseUrl;

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
			return NewProcessInfo(BuildHistoryProcessArgs(from, to));
		}

		public ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			return NewProcessInfo(BuildTagProcessArgs(result.Label, result.LastChangeNumber));
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessResult result = Execute(CreateHistoryProcessInfo(from.StartTime, to.StartTime));
			Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
			if (UrlBuilder != null)
			{
				UrlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (TagOnSuccess && result.Succeeded)
			{
				Execute(NewLabelProcessInfo(result));
			}
		}

		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.AppendFormat(HISTORY_COMMAND_FORMAT, FormatCommandDate(from), FormatCommandDate(to), TrunkUrl);
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
				buffer.AppendFormat(TAG_COMMAND_FORMAT, label, WorkingDirectory.TrimEnd('\\'), TagBaseUrl);
			}
			else
			{
				buffer.AppendFormat(TAG_COMMAND_FORMAT, label, TrunkUrl, TagBaseUrl);
				buffer.AppendFormat(" --revision {0}", revision);
			}
			AppendUsernameAndPassword(buffer);

			return buffer.ToString();
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				ProcessInfo info = NewProcessInfo(BuildGetSourceArguments(result.LastChangeNumber));
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

		private ProcessInfo NewProcessInfo(string args)
		{
			return new ProcessInfo(Executable, args, WorkingDirectory);
		}
	}
}