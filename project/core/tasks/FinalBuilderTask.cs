namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Text;
    using System.IO;
    using ThoughtWorks.CruiseControl.Core.Util;
    using Exortech.NetReflector;

    /// <summary>
    /// <para>
    /// The FinalBuilder Task allows you to invoke FinalBuilder build projects as part of a CruiseControl.NET
    /// integration project. FinalBuilder is a commercial build and release management solution for Windows software
    /// developers and SCM professionals, developed and marketed by VSoft Technologies 
    /// (http://www.finalbuilder.com/finalbuilder.aspx).
    /// </para>
    /// </summary>
    /// <title>FinalBuilder Task</title>
    /// <version>1.3</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;FinalBuilder&gt;
    /// &lt;ProjectFile&gt;C:\Projects\BuildProject\Build Process.fbz5&lt;/ProjectFile&gt;
    /// &lt;/FinalBuilder&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;FinalBuilder&gt;
    /// &lt;ProjectFile&gt;C:\Projects\BuildProject\Build Process.fbz5&lt;/ProjectFile&gt;
    /// &lt;FBVersion&gt;5&lt;/FBVersion&gt;
    /// &lt;ShowBanner&gt;false&lt;/ShowBanner&gt;
    /// &lt;FBVariables&gt;
    /// &lt;FBVariable name="IsContinuousIntegrationBuild" value="True" /&gt;
    /// &lt;/FBVariables&gt;
    /// &lt;Timeout&gt;3600&lt;/Timeout&gt;
    /// &lt;DontWriteToLog&gt;true&lt;/DontWriteToLog&gt;
    /// &lt;/FinalBuilder&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Frequently Asked Questions</heading>
    /// <para>
    /// <b>Can I format output to the CruiseControl.NET web dashboard?</b>
    /// </para>
    /// <para>
    /// At the moment, output from the FinalBuilder Task is plain text. We are planning to change this so that the task
    /// outputs XML which can be easily used with the various CruiseControl.NET publishers. In the meantime, it is
    /// possible to use the FinalBuilder Export Log Action to export an XML file, which can then be incorporated via
    /// the File Merge Task.
    /// </para>
    /// <b>Which FinalBuilder version do I need?</b>
    /// <para>
    /// The task will work with FinalBuilder versions 3, 4, and 5. However, because the task uses the FBCMD command
    /// line utility, users of FinalBuilder 3 and 4 will need the Professional Edition. FinalBuilder 5 users can use
    /// either the Standard or Professional editions. A free 30 day trial download is available.
    /// </para>
    /// </remarks>
    [ReflectorType("FinalBuilder")]
	public class FinalBuilderTask
        : TaskBase
	{
		#region Fields

		private readonly ProcessExecutor _executor;
		private readonly IRegistry _registry;

		private FinalBuilderVersion _fbversion;
		private string _fbcmdpath;
                	
		#endregion

		#region Constructors

		public FinalBuilderTask() : this(new Registry(), new ProcessExecutor()) {}
		
		public FinalBuilderTask(IRegistry registry, ProcessExecutor executor)
		{
			_executor = executor;
			_registry = registry;
			_fbversion = FinalBuilderVersion.FBUnknown;
			_fbcmdpath = String.Empty;
		}

		#endregion

		#region Properties		
        /// <summary>
        /// The full path of the FinalBuilder project to run.
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
		[ReflectorProperty("ProjectFile", Required = true)]
		public string ProjectFile = string.Empty;

        /// <summary>
        /// Specify 'true' to enable the "banner" at the top of the FinalBuilder console output.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("ShowBanner", Required = false)]
		public bool ShowBanner;

        /// <summary>
        /// One or more FBVariable elements to pass to FinalBuilder. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorArray("FBVariables", Required = false)] 
		public FBVariable[] FBVariables;

        /// <summary>
        /// Use this element to explicitly specify a version of FinalBuilder to run (for instance, you could force
        /// a FinalBuilder 4 project to run in FinalBuilder 5.)
        /// </summary>
        /// <version>1.3</version>
        /// <default>Generated</default>
        /// <remarks>
        /// If this element is not specified, the FinalBuilder version is determined automatically from the project
        /// file name (recommended.)
        /// </remarks>
        [ReflectorProperty("FBVersion", Required = false)]
		public int FBVersion 
		{
			get
			{
				if (_fbversion == FinalBuilderVersion.FBUnknown) // Try and autodetect FB Version from project file name
				{
					try
					{
						return Byte.Parse(ProjectFile.Substring(ProjectFile.Length - 1, 1));
					}
					catch
					{
						throw new BuilderException(this, "Finalbuilder version could not be autodetected from project file name.");
					}
				}

				return (int)_fbversion;
			}
			set
			{
				_fbversion = (FinalBuilderVersion)value;
			}
		}

        /// <summary>
        /// The absolute path to FBCMD.EXE.
        /// </summary>
        /// <version>1.3</version>
        /// <default>Generated</default>
        /// <remarks>
        /// If this value is not set, then the value will be generated using either FBVersion or the project file.
        /// </remarks>
        [ReflectorProperty("FBCMDPath", Required = false)]
		public string FBCMDPath
		{
            get { return string.IsNullOrEmpty(_fbcmdpath) ? GetFBPath() : _fbcmdpath; }
			set {	_fbcmdpath = value;		}
		}

        /// <summary>
        /// Disable output to the FinalBuilder project log file.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("DontWriteToLog", Required = false)]
		public bool DontWriteToLog;

        /// <summary>
        /// Log to a temporary log file which is deleted when the project closes. Overrides DontWriteToLog.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        /// <remarks>
        /// Use this option instead of DontWriteToLog if you still want to be able to use the Export Log action, but
        /// don't want the log file to be updated.
        /// </remarks>
        [ReflectorProperty("UseTemporaryLogFile", Required = false)]
        public bool UseTemporaryLogFile;

        /// <summary>
        /// The number of seconds to wait before assuming that the FinalBuilder project has hung and should be killed. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>0</default>
        [ReflectorProperty("Timeout", Required = false)]
		public int Timeout;

        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : 
                            string.Format("Executing FinalBuilder : BuildFile: {0} ", ProjectFile));

            ProcessResult processResult = AttemptToExecute(NewProcessInfoFrom(result), result.ProjectName);
			result.AddTaskResult(new ProcessTaskResult(processResult));

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "Build timed out (after " + Timeout + " seconds)");
			}

            return !processResult.Failed;
		}

		#endregion

		#region Methods

		protected ProcessResult AttemptToExecute(ProcessInfo info, string projectName)
		{
			try
			{
				return _executor.Execute(info);
			}
			catch (Exception e)
			{
				throw new BuilderException(this, string.Format("FBCMD unable to execute: {0}\n{1}", info, e), e);
			}
		}

		private ProcessInfo NewProcessInfoFrom(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(FBCMDPath, GetFBArgs());
			info.TimeOut = Timeout*1000;
            int idx = ProjectFile.LastIndexOf('\\');
            if (idx > -1)
              info.WorkingDirectory = ProjectFile.Remove(idx, ProjectFile.Length - idx); // Trim down proj. file to get working dir.
			// Add IntegrationProperties as environment variables
			foreach (string varName in result.IntegrationProperties.Keys)
			{
				object obj1 = result.IntegrationProperties[varName];
				if ((obj1 != null) && !info.EnvironmentVariables.ContainsKey(varName))
				{
				  info.EnvironmentVariables.Add(varName, StringUtil.AutoDoubleQuoteString(StringUtil.RemoveTrailingPathDelimeter(StringUtil.IntegrationPropertyToString(obj1))));
				}
			}           
			return info;
		}

		// Returns arguments to FBCMD.EXE
		private string GetFBArgs()
		{
			StringBuilder args = new StringBuilder();

			if (!ShowBanner)
			{
				args.Append("/B ");
			}

            if (UseTemporaryLogFile)
            {
                args.Append("/TL ");
            }
			else if (DontWriteToLog)
			{
				args.Append("/S ");
			}

			
			if (FBVariables != null && FBVariables.Length > 0)
			{
				args.Append("/V");
				for(int j = 0; j < FBVariables.Length; j++)				
				{					
					args.Append(FBVariables[j].Name);
					args.Append("=");
					args.Append(StringUtil.AutoDoubleQuoteString(FBVariables[j].Value));
					if(j < FBVariables.Length - 1)
					{
						args.Append(";");
					}
					else
					{
						args.Append(" ");
					}
				}								
			}
			
			args.Append("/P");
			args.Append(StringUtil.AutoDoubleQuoteString(ProjectFile));		    
			return args.ToString();
		}	
		
		private string GetFBPath()
		{			
			int fbversion = FBVersion;			
			string keyName = String.Format(@"SOFTWARE\VSoft\FinalBuilder\{0}.0", fbversion); // Works for FB 3 through 5, should work for future versions
	
			string executableDir = _registry.GetLocalMachineSubKeyValue(keyName, "Location");
            if (string.IsNullOrEmpty((executableDir)))
			{
				throw new BuilderException(this, String.Format("Path to Finalbuilder {0} command line executable could not be found.", FBVersion));				
			}

			if (fbversion == 3) // FinalBuilder 3 has a different executable name to other versions
				return Path.GetDirectoryName(executableDir) + @"\FB3Cmd.exe";

			return Path.GetDirectoryName(executableDir) + @"\FBCmd.exe";
		}
	
		#endregion

		private enum FinalBuilderVersion
		{
			FBUnknown = -1,
			FB3 = 3,
			FB4 = 4,
			FB5 = 5
		}
	}
}
