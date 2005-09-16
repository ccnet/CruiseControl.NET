using System;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		public const string DefaultExecutable = "svn.exe";
		internal static readonly string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		public Svn(ProcessExecutor executor, IHistoryParser parser) : base(parser, executor)
		{}

		public Svn() : base(new SvnHistoryParser())
		{}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IModificationUrlBuilder UrlBuilder;

		[ReflectorProperty("executable", Required = false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("trunkUrl", Required = false)]
		public string TrunkUrl;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;

		[ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess = false;

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

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessResult result = Execute(NewHistoryProcessInfo(from, to));
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

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				Execute(NewGetSourceProcessInfo(result));
			}
		}

		private ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.Append("update");
			AppendRevision(buffer, result.LastChangeNumber);
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), result);
		}

//		TAG_COMMAND_FORMAT = "copy --message "CCNET build label" "trunkUrl" "tagBaseUrl/label"
		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("copy");
			buffer.AppendArgument(TagMessage(result.Label));
			buffer.AppendArgument(TagSource(result.LastChangeNumber));
			buffer.AppendArgument(TagDestination(result.Label));
			AppendRevision(buffer, result.LastChangeNumber);
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), result);
		}

//		HISTORY_COMMAND_FORMAT = "log TrunkUrl --revision \"{{{StartDate}}}:{{{EndDate}}}\" --verbose --xml --non-interactive";
		private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("log");
			buffer.AppendArgument(TrunkUrl);
			buffer.AppendArgument(string.Format("-r \"{{{0}}}:{{{1}}}\"", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
			buffer.AppendArgument("--verbose --xml");
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), to);
		}

		private string TagMessage(string label)
		{
			return string.Format("-m \"CCNET build {0}\"", label);
		}

		private string TagSource(int revision)
		{
			string trunkUrl = TrunkUrl;
			if (revision == 0)
			{
				trunkUrl = WorkingDirectory.TrimEnd(Path.DirectorySeparatorChar);
			}
			return SurroundInQuotesIfContainsSpace(trunkUrl);
		}

		private string TagDestination(string label)
		{
			return SurroundInQuotesIfContainsSpace(string.Format("{0}/{1}", TagBaseUrl, label));
		}

		private void AppendCommonSwitches(ProcessArgumentBuilder buffer)
		{
			buffer.AppendArgument("--username {0}", SurroundInQuotesIfContainsSpace(Username));
			buffer.AppendArgument("--password {0}", SurroundInQuotesIfContainsSpace(Password));
			buffer.AppendArgument("--non-interactive");
		}

		private string SurroundInQuotesIfContainsSpace(string value)
		{
			if (! StringUtil.IsBlank(value) && value.IndexOf(' ') >= 0)
				return string.Format(@"""{0}""", value);
			return value;
		}

		private void AppendRevision(ProcessArgumentBuilder buffer, int revision)
		{
			buffer.AppendIf(revision > 0, "--revision {0}", revision.ToString());
		}

		private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
		{
			return new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
		}
	}
}