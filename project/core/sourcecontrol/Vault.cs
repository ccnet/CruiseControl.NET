using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vault")]
	public class Vault : ProcessSourceControl
	{
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private const string COMMAND_LINE = @"history ""{0}"" -host {1} -user {2} -password {3} -repository {4} -rowlimit 0";
		private string _username;
		private string _password;
		private string _host;
		private string _repository;
		private string _folder;
		private string _executable;
		private bool _ssl;

		#region Properties
		[ReflectorProperty("username")]
		public string Username
		{
			get
			{
				return _username;
			}
			set
			{
				_username = value;
			}
		}

		[ReflectorProperty("password")]
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		[ReflectorProperty("host")]
		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}

		[ReflectorProperty("repository")]
		public string Repository
		{
			get
			{
				return _repository;
			}
			set
			{
				_repository = value;
			}
		}

		[ReflectorProperty("folder")]
		public string Folder
		{
			get
			{
				return _folder;
			}
			set
			{
				_folder = value;
			}
		}

		[ReflectorProperty("executable")]
		public string Executable
		{
			get
			{
				return _executable;
			}
			set
			{
				_executable = value;
			}
		}

		[ReflectorProperty("ssl", Required=false)]
		public bool Ssl
		{
			get
			{
				return _ssl;
			}
			set
			{
				_ssl = value;
			}
		}
		#endregion

		public Vault() : base(new VaultHistoryParser())
		{
		}

		public Vault(IHistoryParser historyParser, ProcessExecutor executor): base(historyParser, executor)
		{
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			return CreateHistoryProcessInfo(from, to, _folder);
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to, string folder)
		{
			string args = BuildHistoryProcessArgs(from, to);
			return new ProcessInfo(_executable, args);
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
			string args = string.Format(
				COMMAND_LINE,
				_folder,
				_host,
				_username,
				_password,
				_repository);
			if(_ssl)
			{
				args += " -ssl";
			}
			return args;
		}
	}
}
