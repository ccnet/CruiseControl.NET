
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.State
{
    /// <summary>
    /// Records the state of a project.
    /// </summary>
    public class XmlProjectStateManager
        : IProjectStateManager
    {
        #region Fields
		private readonly IFileSystem fileSystem;
		private readonly IExecutionEnvironment executionEnvironment;

        private readonly string persistanceFileName;
        private Dictionary<string, bool> projectStates = null;
        private bool isLoading;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="XmlProjectStateManager"/> with the default path.
        /// </summary>
        public XmlProjectStateManager()
			: this(new SystemIoFileSystem(), new ExecutionEnvironment())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlProjectStateManager" /> class.	
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="executionEnvironment">The execution environment.</param>
        /// <remarks></remarks>
		public XmlProjectStateManager(IFileSystem fileSystem, IExecutionEnvironment executionEnvironment)
		{
			this.fileSystem = fileSystem;
			this.executionEnvironment = executionEnvironment;

			persistanceFileName = Path.Combine(this.executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server), "ProjectsState.xml");
			fileSystem.EnsureFolderExists(persistanceFileName);
		}
        #endregion

        /// <summary>
        /// Gets the name of the persistance file.	
        /// </summary>
        /// <value>The name of the persistance file.</value>
        /// <remarks></remarks>
		public string PersistanceFileName
		{
			get { return persistanceFileName; }
		}

        #region Public methods
        #region RecordProjectAsStopped()
        /// <summary>
        /// Records a project as stopped.
        /// </summary>
        /// <param name="projectName">The name of the project to record</param>
        public void RecordProjectAsStopped(string projectName)
        {
            ChangeProjectState(projectName, false);
        }
        #endregion

        #region RecordProjectAsStartable()
        /// <summary>
        /// Records a project as being able to start automatically.
        /// </summary>
        /// <param name="projectName">The name of the project to record</param>
        public void RecordProjectAsStartable(string projectName)
        {
            ChangeProjectState(projectName, true);
        }
        #endregion

        #region CheckIfProjectCanStart()
        /// <summary>
        /// Checks if a project can be started automatically.
        /// </summary>
        /// <param name="projectName">The name of the project to check.</param>
        /// <returns></returns>
        public bool CheckIfProjectCanStart(string projectName)
        {
            LoadProjectStates(false);
            if (projectStates.ContainsKey(projectName))
            {
                return projectStates[projectName];
            }
            else
            {
                return true;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region LoadProjectStates()
        /// <summary>
        /// Loads all the states from a persistance file.
        /// </summary>
        /// <param name="forceLoad"></param>
        private void LoadProjectStates(bool forceLoad)
        {
            if (forceLoad || (projectStates == null))
            {
                projectStates = new Dictionary<string, bool>();

                if (fileSystem.FileExists(persistanceFileName))
                {
                    isLoading = true;
                    try
                    {
                        var stateDocument = new XmlDocument();
                        using (var stream = fileSystem.OpenInputStream(persistanceFileName))
                        {
                            stateDocument.Load(stream);
                        }
                        foreach (XmlElement projectState in stateDocument.SelectNodes("/state/project"))
                        {
                            ChangeProjectState(projectState.InnerText, false);
                        }
                    }
                    finally
                    {
                        isLoading = false;
                    }
                }
            }
        }
        #endregion

        #region SaveProjectStates()
        /// <summary>
        /// Saves all the states to a persistance file.
        /// </summary>
        private void SaveProjectStates()
        {
            if (projectStates != null)
            {
                var stateDocument = new XmlDocument();
                var rootElement = stateDocument.CreateElement("state");
                stateDocument.AppendChild(rootElement);
                foreach (var projectName in projectStates.Keys)
                {
                    if (!projectStates[projectName])
                    {
                        var projectElement = stateDocument.CreateElement("project");
                        projectElement.InnerText = projectName;
                        rootElement.AppendChild(projectElement);
                    }
                }
                using (var stream = fileSystem.OpenOutputStream(persistanceFileName))
                {
                    var settings = new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8,
                        OmitXmlDeclaration = true,
                        Indent = false
                    };
                    using (var xmlWriter = XmlTextWriter.Create(stream, settings))
                    {
                        stateDocument.Save(xmlWriter);
                    }
                }
            }
        }
        #endregion

        #region ChangeProjectState()
        /// <summary>
        /// See if we need to change the state and if so, change it, then persist the states
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="newState"></param>
        private void ChangeProjectState(string projectName, bool newState)
        {
            LoadProjectStates(false);
            var saveStates = true;
            if (projectStates.ContainsKey(projectName))
            {
                if (projectStates[projectName] != newState)
                {
                    projectStates[projectName] = newState;
                }
                else
                {
                    saveStates = false;
                }
            }
            else
            {
                projectStates.Add(projectName, newState);
            }
            if (saveStates && !isLoading) SaveProjectStates();
        }
        #endregion
        #endregion
    }
}
