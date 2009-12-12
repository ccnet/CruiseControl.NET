using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	public abstract class ProjectBase: INotifyPropertyChanged
	{
		public static readonly string DefaultWorkingSubDirectory = "WorkingDirectory";
		public static readonly string DefaultArtifactSubDirectory = "Artifacts";

		private string name;
        private string description;
		private string category = string.Empty;
		private string configuredWorkingDirectory = string.Empty;
		private string configuredArtifactDirectory = string.Empty;
        private ITrigger triggers = new MultipleTrigger(new ITrigger[] { });
		private ExternalLink[] externalLinks = new ExternalLink[0];
        private DisplayLevel askForForceBuildReason = DisplayLevel.None;
		private readonly IExecutionEnvironment executionEnvironment;

		protected ProjectBase() : this(new ExecutionEnvironment())
		{}

		protected ProjectBase(IExecutionEnvironment executionEnvironment)
		{
			this.executionEnvironment = executionEnvironment;
		}

        /// <summary>
        /// The name of your project - this must be unique for any given CruiseControl.NET server.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("name")]
		public virtual string Name
		{
			get { return name; }
            set
            {
                name = value;
                FirePropertyChanged("Name");
            }
        }

        /// <summary>
        /// An optional description of the project.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("description", Required = false)]
        public virtual string Description
        {
            get { return description; }
            set
            {
                description = value;
                FirePropertyChanged("Description");
            }
		}

        /// <summary>
        /// A general category for this project. This is used by the dashboard to provide groupings to the project. Categories do not span
        /// servers.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
		[ReflectorProperty("category", Required=false)]
		public virtual string Category
		{
			get { return category; }
			set { category = value; }
		}

        /// <summary>
        /// Trigger blocks allow you to specify when CruiseControl.NET will start a new integration cycle.
        /// </summary>
        /// <remarks>
        /// Specifying an empty element (&lt;triggers /&gt;) means integrations are only ever forced manually (for example using CCTray or the
        /// Web Dashboard.) Not including a &lt;triggers&gt; element at all means the project will behave like a &lt;triggers /&gt; element
        /// (before 1.4.3 the default when not including a &lt;triggers&gt; was a single <link>Interval Trigger</link> with default
        /// configuration).
        /// </remarks>
        /// <version>1.0</version>
        /// <default>None</default>
		[ReflectorProperty("triggers", InstanceType=typeof(MultipleTrigger), Required=false)]
		public ITrigger Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

        /// <summary>
        /// The Working Directory for the project (this is used by other blocks). Relative paths are relative to a directory called the project
        /// Name in the directory where the CruiseControl.NET server was launched from. The Working Directory is meant to contain the checked
        /// out version of the project under integration. Make sure this folder us unique per project to prevent problems with the build. You
        /// don't need to quote the Working Directory, even if it contains spaces.
        /// </summary>
        /// <version>1.0</version>
        /// <default>WorkingDirectory</default>
		[ReflectorProperty("workingDirectory", Required=false)]
		public string ConfiguredWorkingDirectory
		{
			get { return configuredWorkingDirectory; }
			set { configuredWorkingDirectory = value; }
		}

        /// <summary>
        /// The Artifact Directory for the project (this is used by other blocks). Relative paths are relative to a directory called the
        /// project Name in the directory where the CruiseControl.NET server was launched from. The Artifact Directory is meant to be a
        /// persistence location for anything you want saved from the results of the build, e.g. build logs, distributables, etc. Make sure
        /// this folder us unique per project to prevent problems with reporting about a build. You don't need to quote the Aftifact Directory,
        /// even if it contains spaces.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Artifacts</default>
		[ReflectorProperty("artifactDirectory", Required=false)]
		public string ConfiguredArtifactDirectory
		{
			get { return configuredArtifactDirectory; }
			set { configuredArtifactDirectory = value; }
		}

        /// <summary>
        /// Each of these are used to display project related links on the project report page of the Web Dashboard, and are meant as a
        /// convenient shortcut to project-related web sites outside of CruiseControl.NET.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
		[ReflectorArray("externalLinks", Required=false)]
		public ExternalLink[] ExternalLinks
		{
			get { return externalLinks; }
			set { externalLinks = value; }
		}

        #region AskForForceBuildReason
        /// <summary>
        /// Should a reason be requested when a force build is triggered.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("askForForceBuildReason", Required = false)]
        public DisplayLevel AskForForceBuildReason
        {
            get { return askForForceBuildReason; }
            set { askForForceBuildReason = value; }
        }

        #endregion

		public string WorkingDirectory
		{
			get
			{
                if (string.IsNullOrEmpty(configuredWorkingDirectory))
				{
					return executionEnvironment.EnsurePathIsRooted(Path.Combine(Name, DefaultWorkingSubDirectory));
				}
				return new DirectoryInfo(configuredWorkingDirectory).FullName;
			}
		}

		public string ArtifactDirectory
		{
			get
			{
                if (string.IsNullOrEmpty(configuredArtifactDirectory))
				{
					return executionEnvironment.EnsurePathIsRooted(Path.Combine(Name, DefaultArtifactSubDirectory));
				}
				return new DirectoryInfo(configuredArtifactDirectory).FullName;
			}
		}

        #region Public events
        /// <summary>
        /// A property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Protected methods
        #region FirePropertyChanged()
        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
        #endregion
	}
}
