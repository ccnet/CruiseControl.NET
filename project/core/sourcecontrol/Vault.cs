using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vault")]
	public class Vault : ProcessSourceControl
	{
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private const string COMMAND_LINE = @"history ""{0}"" -host ""{1}"" -user ""{2}"" -password ""{3}"" -repository ""{4}"" -rowlimit 0";

		[ReflectorProperty("username")]
		public string Username;

		[ReflectorProperty("password")]
		public string Password;

		[ReflectorProperty("host")]
		public string Host;

		[ReflectorProperty("repository")]
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
			string args = BuildHistoryProcessArgs(from, to);
			return new ProcessInfo(Executable, args);
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			return GetModifications(CreateHistoryProcessInfo(from, to), from, to);
		}

		public override void LabelSourceControl(string label, DateTime timeStamp)
		{
		}

		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			string args = string.Format(COMMAND_LINE, Folder, Host, Username, Password, Repository);
			if (Ssl)
			{
				args += " -ssl";
			}
			return args;
		}
	}
}