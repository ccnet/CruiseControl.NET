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
	public class StarTeam : ProcessSourceControl, IStarTeamRegExProvider
	{
		//stcmd hist -nologo -x -is -filter IO -p "userid:password@host:port/project/path" "files"		
		internal readonly static string HISTORY_COMMAND_FORMAT = "hist -nologo -x -is -filter IO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" ";
		internal readonly static string GET_SOURCE_COMMAND_FORMAT = "co -nologo -x -is -q -f NCO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" ";
		internal CultureInfo Culture = CultureInfo.CurrentCulture;

		private string _executable;		
		private string _username;
		private string _password;		
		private string _host;
		private int    _port;
		private string _project;
		private string _path;
		private bool _autoGetSource;
		private string _pathOverrideViewWorkingDir;
		private string _pathOverrideFolderWorkingDir;

		// The regular expression to capture info about each folder
		private string folderRegEx = @"(?m:^Folder: (?<folder_name>.+)  \(working dir: (?<working_directory>.+)\)(?s:.*?)(?=^Folder: ))";

		// The regular expression to capture info about each file in a folder
		// KEEP IT AS IT IS, DO NOT ALIGN LINES
		private string fileRegEx = @"(?m:History for: (?<file_name>.+)
Description:(?<file_description>.*)
Locked by:(?<locked_by>.*)
Status:(?<file_status>.+)
-{28}(?# the file history separator ---...)
(?s:(?<file_history>.*?))
={77}(?# the file info separator ====....))";

		// The regular expression to capture the history of a file
		// KEEP IT AS IT IS, DO NOT ALIGN LINES
		private string fileHistoryRegEx = @"(?m:Revision: (?<file_revision>\S+) View: (?<view_name>.+) Branch Revision: (?<branch_revision>\S+)
Author: (?<author_name>.*?) Date: (?<date_string>\d{01,2}/\d{1,2}/\d\d \d{1,2}:\d\d:\d\d (A|P)M).*\n(?s:(?<change_comment>.*?))-{28})";

		public StarTeam(): base(new StarTeamHistoryParser(null))
		{
			_executable = "stcmd.exe";
			_host = "127.0.0.1";
			_port = 49201;
			_path = String.Empty;
			_autoGetSource = false;
			_pathOverrideViewWorkingDir = String.Empty;
			_pathOverrideFolderWorkingDir = String.Empty;
			// We have to do this here since we can't pass a reference to 'this' as part of the call to 'base' above
			// Its nasty, but I don't like inheritence anyway (Mike R)
			_historyParser = new StarTeamHistoryParser(this);
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

		[ReflectorProperty("overrideViewWorkingDir", Required=false)]
		public string OverrideViewWorkingDir
		{
			get { return _pathOverrideViewWorkingDir; }
			set { _pathOverrideViewWorkingDir = value; }
		}
		
		[ReflectorProperty("overrideFolderWorkingDir", Required=false)]
		public string OverrideFolderWorkingDir
		{
			get { return _pathOverrideFolderWorkingDir; }
			set { _pathOverrideFolderWorkingDir = value; }
		}

		[ReflectorProperty("folderRegEx", Required=false)]
		public string FolderRegEx
		{
			get { return folderRegEx; }
			set { folderRegEx = value; }
		}

		[ReflectorProperty("fileRegEx", Required=false)]
		public string FileRegEx
		{
			get { return fileRegEx; }
			set { fileRegEx = value; }
		}

		[ReflectorProperty("fileHistoryRegEx", Required=false)]
		public string FileHistoryRegEx
		{
			get { return fileHistoryRegEx; }
			set { fileHistoryRegEx = value; }
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

		public override void LabelSourceControl(string label, IIntegrationResult result)
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

		internal void AddOptionalArgs(ref string formatted)
		{
			if( 0 != _pathOverrideViewWorkingDir.Length )
			{
				formatted = String.Concat(formatted," -rp ",String.Format("\"{0}\" ",_pathOverrideViewWorkingDir));
			}
			else if( 0 != _pathOverrideFolderWorkingDir.Length )
			{
				formatted = String.Concat(formatted," -fp ",String.Format("\"{0}\" ",_pathOverrideFolderWorkingDir));
			}
		}

		internal string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{			
			string formatted =  string.Format(
				HISTORY_COMMAND_FORMAT,
				Username,
				Password,
				Host,
				Port,
				Project,
				Path);

			AddOptionalArgs(ref formatted);
			formatted = String.Concat(formatted,"\"*\"");
	
			return formatted;
		}

		internal string GetSourceProcessArgs()
		{			
			string formatted = string.Format(
				GET_SOURCE_COMMAND_FORMAT,
				Username,
				Password,
				Host,
				Port,
				Project,
				Path);

			AddOptionalArgs(ref formatted);
			formatted = String.Concat(formatted,"\"*\"");

			return formatted;
		}
	}
}

