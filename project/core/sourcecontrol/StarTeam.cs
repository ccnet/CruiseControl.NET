using System;
using System.Diagnostics;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol
{
	/// <summary>
	/// Source Controller for StarTeam
	/// </summary>	
	[ReflectorType("starteam")]
	public class StarTeam : ProcessSourceControl
	{
		//stcmd hist -nologo -x -is -filter IO -p "userid:password@host:port/project/path" "files"		
		internal readonly static string HISTORY_COMMAND_FORMAT = "hist -nologo -x -is -filter IO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" \"*\"";
		internal readonly static string DATE_FORMAT = "MM/dd/yyyy hh:mm:ss tt";


		private string _executable;		
		private string _username;
		private string _password;		
		private string _host;
		private int    _port;
		private string _project;
		private string _path;

		private IHistoryParser _parser = new StarTeamHistoryParser();

		public StarTeam()
		{
			_executable = "stcmd.exe";
			_host = "127.0.0.1";
			_port = 49201;
			_path = String.Empty;
		}

		protected override IHistoryParser HistoryParser
		{
			get { return _parser; }
		}

		public override Process CreateHistoryProcess(DateTime from, DateTime to)
		{
			string args = BuildHistoryProcessArgs(from, to);
			return ProcessUtil.CreateProcess(Executable, args);
		}

		[ReflectorProperty("executable")]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}

		[ReflectorProperty("project")]
		public string Project
		{
			get { return _project; }
			set { _project = value; }
		}

		[ReflectorProperty("username")]
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		[ReflectorProperty("password")]
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		[ReflectorProperty("host", Required=false)]
		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

		[ReflectorProperty("port", Required=false)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		[ReflectorProperty("path", Required=false)]
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		internal string FormatCommandDate(DateTime date)
		{
			return date.ToString(DATE_FORMAT);
		}

		public override Process CreateLabelProcess(string label, DateTime timeStamp) 
		{
			return null;
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{			
			return String.Format(
			HISTORY_COMMAND_FORMAT,
			Username,
			Password,
			Host,
			Port,
			Project,
			Path);
		}
	}
}
