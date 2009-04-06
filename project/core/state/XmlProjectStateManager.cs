using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.State
{
    /// <summary>
    /// Records the state of a project.
    /// </summary>
    public class XmlProjectStateManager
        : IProjectStateManager
    {
        #region Fields
        private readonly string persistanceFileName = Path.Combine(Environment.CurrentDirectory, "ProjectsState.xml");
        private Dictionary<string, bool> projectStates = null;
        #endregion

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

                if (File.Exists(persistanceFileName))
                {
                    XmlDocument stateDocument = new XmlDocument();
                    stateDocument.Load(persistanceFileName);
                    foreach (XmlElement projectState in stateDocument.SelectNodes("/state/project"))
                    {
                        ChangeProjectState(projectState.InnerText, false);
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
                XmlDocument stateDocument = new XmlDocument();
                XmlElement rootElement = stateDocument.CreateElement("state");
                stateDocument.AppendChild(rootElement);
                foreach (string projectName in projectStates.Keys)
                {
                    if (!projectStates[projectName])
                    {
                        XmlElement projectElement = stateDocument.CreateElement("project");
                        projectElement.InnerText = projectName;
                        rootElement.AppendChild(projectElement);
                    }
                }
                using (XmlTextWriter xmlWriter = new XmlTextWriter(persistanceFileName, Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    stateDocument.Save(xmlWriter);
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
            bool saveStates = true;
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
            if (saveStates) SaveProjectStates();
        }
        #endregion
        #endregion
    }
}
