using System;
using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Source Controller for StarTeam SCM.
    /// </para>
    /// </summary>	
    /// <title>StarTeam Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>starteam</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="starteam"&gt;
    /// &lt;executable&gt;c:\starteam\stcmd.exe&lt;/executable&gt;
    /// &lt;project&gt;ccnet&lt;/project&gt;
    /// &lt;username&gt;buildguy&lt;/username&gt;
    /// &lt;password&gt;buildguypw&lt;/password&gt;
    /// &lt;host&gt;thebuildmachine&lt;/host&gt;
    /// &lt;port&gt;49201&lt;/port&gt;
    /// &lt;path&gt;release2.0&lt;/path&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;folderRegEx&gt;customRegEx&lt;/folderRegEx&gt;
    /// &lt;fileRegEx&gt;customRegEx&lt;/fileRegEx&gt;
    /// &lt;fileHistoryRegEx&gt;customRegEx&lt;/fileHistoryRegEx&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>RegEx Configuration</heading>
    /// <para>
    /// CruiseControl.NET uses StarTeam's command line interface to find changes submitted to Source Control. 3 regular
    /// expressions are used in doing this, as specified above. You have the option of changing these regular
    /// expressions to choose how your instance of CruiseControl.NET parses StarTeam output. It is recommended if you do
    /// this that you download the source version of CruiseControl.NET to see the default RegEx's and how they are used.
    /// </para>
    /// <para>
    /// One suggested alternative RegEx so far is for the fileHistoryRegEx, as follows:
    /// </para>
    /// <code type="None">
    /// ^Revision: (?&lt;file_revision&gt;\S+) View: (?&lt;view_name&gt;.+) Branch Revision: (?&lt;branch_revision&gt;\S+).\nAuthor: (?&lt;author_name&gt;.*) Date: (?&lt;date_string&gt;.*) \w+\r\n(?&lt;change_comment&gt;.*)
    /// </code>
    /// <para>
    /// (Note that this is all one line)
    /// </para>
    /// </remarks>
    [ReflectorType("starteam")]
	public class StarTeam : ProcessSourceControl, IStarTeamRegExProvider
	{
		//stcmd hist -nologo -x -is -filter IO -p "userid:password@host:port/project/path" "files"		
		internal readonly static string HISTORY_COMMAND_FORMAT = "hist -nologo -x -is -filter IO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" ";
		internal readonly static string GET_SOURCE_COMMAND_FORMAT = "co -nologo -ts -x -is -q -f NCO -p \"{0}:{1}@{2}:{3}/{4}/{5}\" ";
		public CultureInfo Culture = CultureInfo.CurrentCulture;

		private string _executable;		
		private string _username;
		private string _password;		
		private string _host;
		private int    _port;
		private string _project;
		private string _path;
		private bool _autoGetSource = true;
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
			historyParser = new StarTeamHistoryParser(this);
		}

        /// <summary>
        /// The local path for the StarTeam command-line client (eg. c:\starteam\stcmd.exe).
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("executable")]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}

        /// <summary>
        /// The StarTeam project (and view) to monitor (eg. project/view).
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
		public string Project
		{
			get { return _project; }
			set { _project = value; }
		}

        /// <summary>
        /// StarTeam ID that CCNet should use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("username")]
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

        /// <summary>
        /// Password for the StarTeam user ID.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("password")]
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

        /// <summary>
        /// The IP address or machine name of the StarTeam server. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>127.0.0.1</default>
        [ReflectorProperty("host", Required = false)]
		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

        /// <summary>
        /// The port on the StarTeam server to connect to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>49201</default>
        [ReflectorProperty("port", Required = false)]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

        /// <summary>
        /// The path to monitor.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("path", Required = false)]
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

        /// <summary>
        /// Instruct CCNet whether or not you want it to automatically retrieve the latest source from the repository.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource
		{
			get { return _autoGetSource; }
			set { _autoGetSource = value; }
		}

        /// <summary>
        /// Instruct CCNet whether or not you want it to automatically retrieve the latest source from the repository.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("overrideViewWorkingDir", Required = false)]
		public string OverrideViewWorkingDir
		{
			get { return _pathOverrideViewWorkingDir; }
			set { _pathOverrideViewWorkingDir = value; }
		}

        /// <summary>
        /// If set, use the -rp option to use a different View Working Directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("overrideFolderWorkingDir", Required = false)]
		public string OverrideFolderWorkingDir
		{
			get { return _pathOverrideFolderWorkingDir; }
			set { _pathOverrideFolderWorkingDir = value; }
		}

        /// <summary>
        /// Allows you to use your own RegEx to parse StarTeam's folder output.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("folderRegEx", Required = false)]
		public string FolderRegEx
		{
			get { return folderRegEx; }
			set { folderRegEx = value; }
		}

        /// <summary>
        /// Allows you to use your own RegEx to parse StarTeam's file output.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("fileRegEx", Required = false)]
		public string FileRegEx
		{
			get { return fileRegEx; }
			set { fileRegEx = value; }
		}

        /// <summary>
        /// Allows you to use your own RegEx to parse StarTeam's file history.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("fileHistoryRegEx", Required = false)]
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

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            Modification[] modifications = GetModifications(CreateHistoryProcessInfo(from.StartTime, to.StartTime), from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
        }

		public override void LabelSourceControl(IIntegrationResult result)
		{
		}

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from StarTeam");

			if (AutoGetSource)
			{
				string args = GetSourceProcessArgs();
				ProcessInfo info = new ProcessInfo(Executable, args);
				Execute(info);
			}
		}

		public string FormatCommandDate(DateTime date)
		{
			return date.ToString(Culture.DateTimeFormat);
		}

		internal void AddOptionalArgs(ref string formatted)
		{
			if( 0 != _pathOverrideViewWorkingDir.Length )
			{
				formatted = String.Concat(formatted," -rp ",string.Format(System.Globalization.CultureInfo.CurrentCulture,"\"{0}\" ",_pathOverrideViewWorkingDir));
			}
			else if( 0 != _pathOverrideFolderWorkingDir.Length )
			{
				formatted = String.Concat(formatted," -fp ",string.Format(System.Globalization.CultureInfo.CurrentCulture,"\"{0}\" ",_pathOverrideFolderWorkingDir));
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

		public string GetSourceProcessArgs()
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