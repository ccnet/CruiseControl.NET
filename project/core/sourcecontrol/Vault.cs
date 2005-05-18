using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vault")]
	public class Vault : ProcessSourceControl
	{
		[ReflectorProperty("username", Required=false)]
		public string Username;

		[ReflectorProperty("password", Required=false)]
		public string Password;

		[ReflectorProperty("host", Required=false)]
		public string Host;

		[ReflectorProperty("repository", Required=false)]
		public string Repository;

		[ReflectorProperty("folder")]
		public string Folder;

		[ReflectorProperty("executable")]
		public string Executable;

		[ReflectorProperty("ssl", Required=false)]
		public bool Ssl = false;

		public Vault() : base(new VaultHistoryParser())
		{
		}

		public Vault(IHistoryParser historyParser, ProcessExecutor executor) : base(historyParser, executor)
		{
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			return CreateHistoryProcessInfo(from, to, Folder);
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to, string folder)
		{
			return new ProcessInfo(Executable, BuildHistoryProcessArgs());
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			return GetModifications(CreateHistoryProcessInfo(from.StartTime, to.StartTime), from.StartTime, to.StartTime);
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
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