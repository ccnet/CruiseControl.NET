using Exortech.NetReflector;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
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
			StringBuilder args = new StringBuilder();
			args.Append("-s ");
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
			args.Append("changes -s submitted ");
			args.Append(View);
			if (from==DateTime.MinValue) 
			{
				args.Append("@" + formatDate(to));
			} 
			else 
			{
				args.Append(String.Format("@{0},@{1}", formatDate(from), formatDate(to)));
			}
			return args.ToString();
		}

		public virtual Process CreateChangeListProcess(DateTime from, DateTime to) 
		{
			return ProcessUtil.CreateProcess(Executable, BuildCommandArguments(from, to));
		}

		public virtual Process CreateDescribeProcess(string changes)
		{
			if (changes.Length == 0)
				throw new Exception("Empty changes list found - this should not happen");

			foreach (char c in changes)
			{
				if (! (Char.IsDigit(c) || c == ' ') )
					throw new CruiseControlException("Invalid changes list encountered");
			}

			string args = "-s describe -s " + changes;
			return ProcessUtil.CreateProcess(Executable, args);
		}

		public Modification[] GetModifications(DateTime from, DateTime to) 
		{
			P4HistoryParser parser = new P4HistoryParser();
			Process process = CreateChangeListProcess(from, to);
			string processResult = execute(process);
			String changes = parser.ParseChanges(processResult);
			if (changes.Length == 0)
			{
				return new Modification[0];
			}
			else
			{
				process = CreateDescribeProcess(changes);
				return parser.Parse(new StringReader(execute(process)));
			}
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}

		protected virtual string execute(Process p)
		{
			return ProcessUtil.ExecuteRedirected(p).ReadToEnd();
		}

		private string formatDate(DateTime date)
		{
			return date.ToString(COMMAND_DATE_FORMAT);
		}
	}
}
