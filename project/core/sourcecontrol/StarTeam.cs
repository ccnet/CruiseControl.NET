using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Source Controller for StarTeam
	/// </summary>	
	[ReflectorType("starteam")]
	public class StarTeam : ProcessSourceControl
	{
		//stcmd hist -nologo -x -is -filter IO -p "userid:password@host:port/project/path" "files"		
		internal readonly static string HISTORY_COMMAND_FORMAT = "hist -nologo -x -is -filter IO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" \"*\"";
		internal readonly static string GET_SOURCE_COMMAND_FORMAT = "co -nologo -x -is -q -f NCO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" \"*\"";
		internal CultureInfo Culture = CultureInfo.CurrentCulture;

		private string _executable;		
		private string _username;
		private string _password;		
		private string _host;
		private int    _port;
		private string _project;
		private string _path;
		private bool _autoGetSource;

		public StarTeam(): base(new StarTeamHistoryParser())
		{
			_executable = "stcmd.exe";
			_host = "127.0.0.1";
			_port = 49201;
			_path = String.Empty;
			_autoGetSource = false;
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

		[ReflectorProperty("autoGetSource", Required=false)]
		public bool AutoGetSource
		{
			get { return _autoGetSource; }
			set { _autoGetSource = value; }
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
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

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				string args = GetSourceProcessArgs();
				ProcessInfo info = new ProcessInfo(Executable, args);
				Execute(info);
			}
		}

		internal string FormatCommandDate(DateTime date)
		{
			return date.ToString(Culture.DateTimeFormat);
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{			
			return string.Format(
				HISTORY_COMMAND_FORMAT,
				Username,
				Password,
				Host,
				Port,
				Project,
				Path);
		}

		internal string GetSourceProcessArgs()
		{			
			return string.Format(
				GET_SOURCE_COMMAND_FORMAT,
				Username,
				Password,
				Host,
				Port,
				Project,
				Path);
		}
	}
}
