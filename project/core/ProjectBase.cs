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
		private string category = "";
		private string configuredWorkingDirectory = "";
		private string configuredArtifactDirectory = "";
        private ITrigger triggers = new MultipleTrigger(new ITrigger[] { });
		private ExternalLink[] externalLinks = new ExternalLink[0];

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

		[ReflectorProperty("category", Required=false)]
		public virtual string Category
		{
			get { return category; }
			set { category = value; }
		}

		[ReflectorProperty("triggers", InstanceType=typeof(MultipleTrigger), Required=false)]
		public ITrigger Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		[ReflectorProperty("workingDirectory", Required=false)]
		public string ConfiguredWorkingDirectory
		{
			get { return configuredWorkingDirectory; }
			set { configuredWorkingDirectory = value; }
		}

		[ReflectorProperty("artifactDirectory", Required=false)]
		public string ConfiguredArtifactDirectory
		{
			get { return configuredArtifactDirectory; }
			set { configuredArtifactDirectory = value; }
		}

		[ReflectorArray("externalLinks", Required=false)]
		public ExternalLink[] ExternalLinks
		{
			get { return externalLinks; }
			set { externalLinks = value; }
		}

		public string WorkingDirectory
		{
			get
			{
                if (string.IsNullOrEmpty(configuredWorkingDirectory))
				{
					return new DirectoryInfo(Path.Combine(Name, DefaultWorkingSubDirectory)).FullName;
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
					return new DirectoryInfo(Path.Combine(Name, DefaultArtifactSubDirectory)).FullName;
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
