using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vault")]
	public class Vault : ProcessSourceControl
	{
		public const string DefaultExecutable = @"C:\Program Files\SourceGear\Vault Client\vault.exe";

		public Vault() : base(new VaultHistoryParser())
		{}

		public Vault(IHistoryParser historyParser, ProcessExecutor executor) : base(historyParser, executor)
		{}

		[ReflectorProperty("username", Required=false)]
		public string Username;

		[ReflectorProperty("password", Required=false)]
		public string Password;

		[ReflectorProperty("host", Required=false)]
		public string Host;

		[ReflectorProperty("repository", Required=false)]
		public string Repository;

		[ReflectorProperty("folder", Required=false)]
		public string Folder = "$";

		[ReflectorProperty("executable", Required=false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("ssl", Required=false)]
		public bool Ssl = false;

		[ReflectorProperty("autoGetSource", Required=false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("applyLabel", Required=false)]
		public bool ApplyLabel = false;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Log.Info(string.Format("Checking for modifications in Vault from {0} to {1}", from.StartTime, to.StartTime));
			return GetModifications(ForHistoryProcessInfo(from), from.StartTime, to.StartTime);
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (! ApplyLabel) return;

			Log.Info("Applying label to Vault");
			Execute(LabelProcessInfo(result));
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (! AutoGetSource) return;

			Log.Info("Getting source from Vault");
			Execute(GetSourceProcessInfo(result));
		}

		private ProcessInfo GetSourceProcessInfo(IIntegrationResult result)
		{
			string args = string.Format(@"get ""{0}"" -destpath ""{1}"" -merge overwrite -performdeletions removeworkingcopy -setfiletime checkin -makewritable", Folder, result.WorkingDirectory);
			return ProcessInfoFor(args, result);
		}

		private ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
			string args = string.Format(@"label ""{0}"" ""{1}""", Folder, result.Label);
			return ProcessInfoFor(args, result);
		}

		private ProcessInfo ForHistoryProcessInfo(IIntegrationResult result)
		{
			return ProcessInfoFor(BuildHistoryProcessArgs(), result);
		}

		private ProcessInfo ProcessInfoFor(string args, IIntegrationResult result)
		{
			return new ProcessInfo(Executable, args, result.WorkingDirectory);
		}

		// "history ""{0}"" -host ""{1}"" -user ""{2}"" -password ""{3}"" -repository ""{4}"" -rowlimit 0"
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private string BuildHistoryProcessArgs()
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddInQuotes("history", Folder);
			builder.AddInQuotes("-host", Host);
			builder.AddInQuotes("-user", Username);
			builder.AddInQuotes("-password", Password);
			builder.AddInQuotes("-repository", Repository);
			builder.AppendArgument("-rowlimit 0");
			builder.AppendIf(Ssl, "-ssl");
			return builder.ToString();
		}
	}
}