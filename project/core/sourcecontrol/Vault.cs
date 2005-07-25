using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vault")]
	public class Vault : ProcessSourceControl
	{
		public const string DefaultExecutable = @"C:\Program Files\SourceGear\Vault Client\vault.exe";
		public const string DefaultHistoryArgs = "-excludeactions label -rowlimit 0";

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

		[ReflectorProperty("historyArgs", Required=false)]
		public string HistoryArgs = DefaultHistoryArgs;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Log.Info(string.Format("Checking for modifications in Vault from {0} to {1}", from.StartTime, to.StartTime));
			return GetModifications(ForHistoryProcessInfo(from, to), from.StartTime, to.StartTime);
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
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("get", Folder);
			builder.AddArgument("-destpath", result.WorkingDirectory);
			builder.AppendArgument("-merge overwrite -performdeletions removeworkingcopy -setfiletime checkin -makewritable");
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder.ToString(), result);
		}

		private ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("label", Folder);
			builder.AddArgument(result.Label);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder.ToString(), result);
		}

		private ProcessInfo ForHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo info = ProcessInfoFor(BuildHistoryProcessArgs(from.StartTime, to.StartTime), from);
			Log.Debug("Vault History command: " + info.ToString());
			return info;
		}

		private ProcessInfo ProcessInfoFor(string args, IIntegrationResult result)
		{
			return new ProcessInfo(Executable, args, result.WorkingDirectory);
		}

		// "history ""{0}"" -excludeactions label -rowlimit 0 -begindate {1:s} -enddate {2:s}
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("history", Folder);
			builder.AppendArgument(HistoryArgs);
			builder.AddArgument("-begindate", from.ToString("s"));
			builder.AddArgument("-enddate", to.ToString("s"));
			AddCommonOptionalArguments(builder);
			return builder.ToString();
		}

		private void AddCommonOptionalArguments(ProcessArgumentBuilder builder)
		{
			builder.AddArgument("-host", Host);
			builder.AddArgument("-user", Username);
			builder.AddArgument("-password", Password);
			builder.AddArgument("-repository", Repository);
			builder.AppendIf(Ssl, "-ssl");
		}
	}
}