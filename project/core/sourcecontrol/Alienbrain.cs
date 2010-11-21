using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Source control integration for the Alienbrain source control product.
    /// </para>
    /// </summary>
    /// <title>Alienbrain Source Control Block</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sourcecontrol type="alienbrain"&gt;
    /// &lt;server&gt;MyServer&lt;/server&gt;
    /// &lt;database&gt;MyDatabase&lt;/database&gt;
    /// &lt;username&gt;Username&lt;/username&gt;
    /// &lt;password&gt;Password&lt;/password&gt;
    /// &lt;project&gt;ab://myprojectpath&lt;/project&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="alienbrain"&gt;
    /// &lt;server&gt;MyServer&lt;/server&gt;
    /// &lt;database&gt;MyDatabase&lt;/database&gt;
    /// &lt;username&gt;Username&lt;/username&gt;
    /// &lt;password&gt;Password&lt;/password&gt;
    /// &lt;project&gt;ab://myprojectpath&lt;/project&gt;
    /// &lt;executable&gt;c:\alienbrain\ab.exe&lt;/executable&gt;
    /// &lt;workingDirectory&gt;d:\code&lt;/workingDirectory&gt;
    /// &lt;branch&gt;Root Branch/Branch1&lt;/branch&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;labelOnSuccess&gt;true&lt;/labelOnSuccess&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>alienbrain</value>
    /// </key>
    /// <remarks>
    /// <heading>Contributions</heading>
    /// <para>
    /// Alienbrain support added by Francis Tremblay.
    /// </para>
    /// </remarks>
    [ReflectorType("alienbrain")]
	public class Alienbrain : ProcessSourceControl
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static readonly string NO_CHANGE = "No files or folders found!";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string AB_REGISTRY_PATH = @"SOFTWARE\NxN\alienbrain";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string AB_REGISTRY_KEY = "InstallDir";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string AB_COMMMAND_PATH = @"Client\Application\Tools";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string AB_EXE = "ab.exe";

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string BRANCH_COMMAND_TEMPLATE = @"setactivebranch ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}""";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string MODIFICATIONS_COMMAND_TEMPLATE = @"find ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -regex ""SCIT > {5} AND SCIT < {6}""  -format ""#CheckInComment#|#Name#|#DbPath#|#SCIT#|#Mime Type#|#LocalPath#|#Changed By#|#NxN_VersionNumber#""";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string LABEL_COMMAND_TEMPLATE = @"setlabel ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -name ""{5}"" -comment ""This label is brought to you by CruiseControl.NET""";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string GET_COMMAND_TEMPLATE = @"getlatest ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -localpath ""{5}"" -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y";

		private IRegistry registry;
		private string executable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Alienbrain" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public Alienbrain() : this(new AlienbrainHistoryParser(), new ProcessExecutor(), new Registry())
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Alienbrain" /> class.	
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="executor">The executor.</param>
        /// <param name="registry">The registry.</param>
        /// <remarks></remarks>
		public Alienbrain(IHistoryParser parser, ProcessExecutor executor, IRegistry registry) : base(parser, executor)
		{
			this.registry = registry;
            this.Server = string.Empty;
            this.Database = string.Empty;
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Branch = string.Empty;
            this.Project = string.Empty;
            this.AutoGetSource = true;
            this.WorkingDirectory = string.Empty;
            this.LabelOnSuccess = false;
		}

        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <remarks>
        /// If not set, then the executable location will come from the registry.
        /// </remarks>
        /// <version>1.0</version>
        /// <default>None</default>
		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
                if (string.IsNullOrEmpty(executable))
					executable = GetExecutableFromRegistry();
				return executable;
			}
			set { executable = value; }
		}

        /// <summary>
        /// Alienbrain server hostname or ip adress. The list of valid server name and ip adresses are listed in the File, Connect to
        /// project database, Step 1, list box.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("server")]
        public string Server { get; set; }

        /// <summary>
        /// Alienbrain project database name. The list of valid project databases are listed in the File, Connect to project database, Step 2,
        /// list box. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("database")]
        public string Database { get; set; }

        /// <summary>
        /// The name of the user you want to use to connect to the server project database.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// The password of the user you want to use to connect to the server project database.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory))]
        public PrivateString Password { get; set; }

        /// <summary>
        /// The path of the branch specification. to enumarate the name of the branches, use the ab enumbranch command line.
        /// </summary>
        /// <remarks>
        /// If this is not set, then the root branch will be used.
        /// </remarks>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("branch", Required = false)]
        public string Branch { get; set; }

        /// <summary>
        /// This is the path of to monitor the file changes. Use alienbrain://Code or ab://Code project path format.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("project", Required = true)]
        public string Project { get; set; }

        /// <summary>
        /// Specifies whether the current version of the source should be retrieved from Alienbrain. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// The path where the get latest will update the files. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Specifies whether or not the repository should be labelled after a successful build. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("labelOnSuccess", Required = false)]
        public bool LabelOnSuccess { get; set; }

		// Actions
		// I had to bake something here because Alienbrain return ERRORLEVEL 1 when nothing is found
		// that caused an exception in CruiseControl.NET that think that the process failed (not pretty in the logs)
		// So I'm checking the result to see if there is changes, if not I return nothing
        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo processInfo = CreateModificationProcess(MODIFICATIONS_COMMAND_TEMPLATE, from.StartTime, to.StartTime);
			processInfo.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(processInfo);
			if (!result.StandardOutput.TrimEnd().EndsWith(NO_CHANGE))
			{
                Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
                base.FillIssueUrl(modifications);
                return modifications;
			}
			else
			{
				return new Modification[0];
			}
		}

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
				SelectBranch();

				ProcessInfo process = CreateLabelProcess(LABEL_COMMAND_TEMPLATE, result);
				Execute(process);
			}

		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from AlienBrain");

			if (AutoGetSource)
			{
				SelectBranch();

				if (result.Modifications.Length > 0)
				{
					foreach (Modification m in result.Modifications)
					{
						ProcessInfo process = CreateGetProcess(m.FolderName + "/" + m.FileName);
						Execute(process);
					}
				}
				else
				{
					ProcessInfo process = CreateGetProcess();
					Execute(process);
				}
			}
		}

		private string GetExecutableFromRegistry()
		{
			string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(AB_REGISTRY_PATH, AB_REGISTRY_KEY);
			return Path.Combine(comServerPath, AB_COMMMAND_PATH + "\\" + AB_EXE);
		}

		private void SelectBranch()
		{
            if (!string.IsNullOrEmpty(Branch))
			{
				ProcessInfo process = CreateBranchProcess(BRANCH_COMMAND_TEMPLATE);
				Execute(process);
			}
		}

        /// <summary>
        /// Determines whether the specified process info has changes.	
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool HasChanges(ProcessInfo processInfo)
		{
			ProcessResult result = Execute(processInfo);
			return !result.StandardOutput.TrimEnd().EndsWith(NO_CHANGE);
		}

		// Process Creations
        /// <summary>
        /// Creates the modification process.	
        /// </summary>
        /// <param name="processCommand">The process command.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public ProcessInfo CreateModificationProcess(string processCommand, DateTime from, DateTime to)
		{
			var arguments = String.Format(CultureInfo.CurrentCulture, processCommand, Project, Server, Database, Username, Password.PrivateValue, from.ToFileTime(), to.ToFileTime());
			return new ProcessInfo(Executable, arguments);
		}

        /// <summary>
        /// Creates the label process.	
        /// </summary>
        /// <param name="processCommand">The process command.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public ProcessInfo CreateLabelProcess(string processCommand, IIntegrationResult result)
		{
			string arguments = String.Format(CultureInfo.CurrentCulture, processCommand, Project, Server, Database, Username, Password.PrivateValue, result.Label);
			return new ProcessInfo(Executable, arguments);
		}

        /// <summary>
        /// Creates the get process.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public ProcessInfo CreateGetProcess()
		{
			return CreateGetProcess(Project);
		}

        /// <summary>
        /// Creates the get process.	
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public ProcessInfo CreateGetProcess(string filename)
		{
			// @"getlatest ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -localpath ""{5}"" -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y"
			var args = new PrivateArguments();
            args.Add("getlatest ", filename, true);
            args.Add("-s ", Server, true);
            args.Add("-d ", Database, true);
            args.Add("-u ", Username, true);
            args.Add("-p ", Password, true);
            args.AddIf(!string.IsNullOrEmpty(WorkingDirectory), "-localpath ", WorkingDirectory, true);
            args.Add("-overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y");
            return new ProcessInfo(Executable, args);
		}

        /// <summary>
        /// Creates the branch process.	
        /// </summary>
        /// <param name="processCommand">The process command.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public ProcessInfo CreateBranchProcess(string processCommand)
		{
			string arguments = String.Format(CultureInfo.CurrentCulture, processCommand, Branch, Server, Database, Username, Password.PrivateValue);
			return new ProcessInfo(Executable, arguments);
		}
	}
}