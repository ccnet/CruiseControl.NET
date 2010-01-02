
namespace ThoughtWorks.CruiseControl.Core
{
	public class ConsoleRunnerArguments
	{
		public const string DEFAULT_CONFIG_PATH = @"ccnet.config";
		private bool useRemoting = true;
		private string project;
		private string configFile;
		private bool validateConfigOnly;
		private bool showHelp;
		private bool launchDebugger;
		private bool logging = true;
		private bool errorPause = true;

		public bool UseRemoting
		{
			get { return useRemoting; }
			set { useRemoting = value; }
		}

		public string Project
		{
			get { return project; }
			set { project = value; }
		}

		public string ConfigFile
		{
			get
			{
				return (configFile == null) ? DEFAULT_CONFIG_PATH : configFile;
			}
			set { configFile = value; }
		}

	    public bool ValidateConfigOnly
	    {
            get { return validateConfigOnly; }
            set { validateConfigOnly = value; }
	    }

		public bool ShowHelp
		{
			get { return showHelp; }
			set { showHelp = value; }
		}

	    public bool LaunchDebugger
	    {
            get { return launchDebugger; }
            set { launchDebugger = value; }
	    }

        public bool Logging
        {
            get { return logging; }
            set { logging = value; }
        }

        public bool PauseOnError
        {
            get { return errorPause; }
            set { errorPause = value; }
        }
	}
}