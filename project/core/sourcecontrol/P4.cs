using Exortech.NetReflector;
using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("p4")]
	public class P4 : ISourceControl
	{
		internal readonly static string COMMAND_DATE_FORMAT = "yyyy/MM/dd:HH:mm:ss";

		private string _executable = "p4";
		private string _view;
		private string _client;
		private string _user;
		private string _port;

		[ReflectorProperty("executable", Required=false)]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}

		[ReflectorProperty("view")]
		public string View
		{
			get{ return _view;}
			set{ _view = value;}
		}

		[ReflectorProperty("client", Required=false)]
		public string Client
		{
			get{ return _client;}
			set{ _client = value;}
		}

		[ReflectorProperty("user", Required=false)]
		public string User
		{
			get{ return _user;}
			set{ _user = value;}
		}

		[ReflectorProperty("port", Required=false)]
		public string Port
		{
			get{ return _port;}
			set{ _port = value;}
		}

		public string BuildCommandArguments(DateTime from, DateTime to)
		{
			StringBuilder args = new StringBuilder(BuildCommonArguments());
			args.Append("changes -s submitted ");
			args.Append(View);
			if (from==DateTime.MinValue) 
			{
				args.Append("@" + FormatDate(to));
			} 
			else 
			{
				args.Append(string.Format("@{0},@{1}", FormatDate(from), FormatDate(to)));
			}
			return args.ToString();
		}

		public virtual ProcessInfo CreateChangeListProcess(DateTime from, DateTime to) 
		{
			return new ProcessInfo(Executable, BuildCommandArguments(from, to));
		}

		public virtual ProcessInfo CreateDescribeProcess(string changes)
		{
			if (changes.Length == 0)
				throw new Exception("Empty changes list found - this should not happen");

			foreach (char c in changes)
			{
				if (! (Char.IsDigit(c) || c == ' ') )
					throw new CruiseControlException("Invalid changes list encountered");
			}

			string args = BuildCommonArguments() + "describe -s " + changes;
			return new ProcessInfo(Executable, args);
		}

		public Modification[] GetModifications(DateTime from, DateTime to) 
		{
			P4HistoryParser parser = new P4HistoryParser();
			ProcessInfo process = CreateChangeListProcess(from, to);
			string processResult = Execute(process);
			String changes = parser.ParseChanges(processResult);
			if (changes.Length == 0)
			{
				return new Modification[0];
			}
			else
			{
				process = CreateDescribeProcess(changes);
				return parser.Parse(new StringReader(Execute(process)), from, to);
			}
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}

		protected virtual string Execute(ProcessInfo p)
		{
			return new ProcessExecutor().Execute(p).StandardOutput;
		}

		private string FormatDate(DateTime date)
		{
			return date.ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}
		
		private string BuildCommonArguments() 
		{
			StringBuilder args = new StringBuilder();
			args.Append("-s "); // for "scripting" mode
			if (Client!=null) 
			{
				args.Append("-c " + Client + " ");
			}
			if (Port!=null) 
			{
				args.Append("-p " + Port + " ");
			}
			if (User!=null)
			{
				args.Append("-u " + User + " ");
			}
			return args.ToString();
		}
	}
}
