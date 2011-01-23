
namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public class ConsoleRunnerArguments
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DEFAULT_CONFIG_PATH = @"ccnet.config";
		private bool useRemoting = true;
		private string project;
		private string configFile;
		private bool validateConfigOnly;
		private bool showHelp;
		private bool launchDebugger;
		private bool logging = true;
		private bool errorPause = true;

        /// <summary>
        /// Gets or sets the use remoting.	
        /// </summary>
        /// <value>The use remoting.</value>
        /// <remarks></remarks>
		public bool UseRemoting
		{
			get { return useRemoting; }
			set { useRemoting = value; }
		}

        /// <summary>
        /// Gets or sets the project.	
        /// </summary>
        /// <value>The project.</value>
        /// <remarks></remarks>
		public string Project
		{
			get { return project; }
			set { project = value; }
		}

        /// <summary>
        /// Gets or sets the config file.	
        /// </summary>
        /// <value>The config file.</value>
        /// <remarks></remarks>
		public string ConfigFile
		{
			get
			{
				return (configFile == null) ? DEFAULT_CONFIG_PATH : configFile;
			}
			set { configFile = value; }
		}

        /// <summary>
        /// Gets or sets the validate config only.	
        /// </summary>
        /// <value>The validate config only.</value>
        /// <remarks></remarks>
	    public bool ValidateConfigOnly
	    {
            get { return validateConfigOnly; }
            set { validateConfigOnly = value; }
	    }

        /// <summary>
        /// Gets or sets the show help.	
        /// </summary>
        /// <value>The show help.</value>
        /// <remarks></remarks>
		public bool ShowHelp
		{
			get { return showHelp; }
			set { showHelp = value; }
		}

        /// <summary>
        /// Gets or sets the launch debugger.	
        /// </summary>
        /// <value>The launch debugger.</value>
        /// <remarks></remarks>
	    public bool LaunchDebugger
	    {
            get { return launchDebugger; }
            set { launchDebugger = value; }
	    }

        /// <summary>
        /// Gets or sets the logging.	
        /// </summary>
        /// <value>The logging.</value>
        /// <remarks></remarks>
        public bool Logging
        {
            get { return logging; }
            set { logging = value; }
        }

        /// <summary>
        /// Gets or sets the pause on error.	
        /// </summary>
        /// <value>The pause on error.</value>
        /// <remarks></remarks>
        public bool PauseOnError
        {
            get { return errorPause; }
            set { errorPause = value; }
        }
	}
}