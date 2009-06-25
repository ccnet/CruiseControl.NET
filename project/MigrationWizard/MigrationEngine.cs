using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    /// <summary>
    /// The engine for performing the actual migration.
    /// </summary>
    public class MigrationEngine
    {
        #region Private fields
        private Stack<Action<string>> rollbackActions = new Stack<Action<string>>();
        #endregion

        #region Public properties
        #region MigrationOptions
        /// <summary>
        /// The current migration options.
        /// </summary>
        public MigrationOptions MigrationOptions { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Performs a migration.
        /// </summary>
        public void Run()
        {
            FireMessage("Starting migration", MigrationEventType.Information);
            var isSuccessful = true;

            if (MigrationOptions.MigrateServer && isSuccessful)
            {
                isSuccessful = MigrateServerFiles();
            }
            if (MigrationOptions.MigrateConfiguration && isSuccessful)
            {
                isSuccessful = MigrateConfiguration();
            }
            if (MigrationOptions.MigrateWebDashboard && isSuccessful)
            {
                isSuccessful = MigrateWebDashboardFiles();
            }

            if (isSuccessful)
            {
                FireMessage("Migration has completed successfully", MigrationEventType.Information);
            }
            else
            {
                FireMessage("Migration has failed - rolling back changes", MigrationEventType.Warning);
                while (rollbackActions.Count > 0)
                {
                    var action = rollbackActions.Pop();
                    action(null);
                }
                FireMessage("Rollback completed", MigrationEventType.Information);
            }
        }
        #endregion
        #endregion

        #region Public events
        #region Message
        /// <summary>
        /// Logs a message.
        /// </summary>
        public event EventHandler<MigrationEventArgs> Message;
        #endregion
        #endregion

        #region Private methods
        #region FireMessage()
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message"></param>
        private void FireMessage(string message, MigrationEventType type)
        {
            if (Message != null)
            {
                Message(this, new MigrationEventArgs(message, type));
            }
        }
        #endregion

        #region MigrateServerFiles()
        private bool MigrateServerFiles()
        {
            var isSuccessful = false;
            try
            {
                FireMessage("Migrating server files", MigrationEventType.Information);

                // Make sure the folder exists
                EnsureFolderExists(MigrationOptions.NewServerLocation, "server");

                // Load the configuration
                var document = new XmlDocument();
                document.Load(MigrationOptions.ConfigurationLocation);

                // Find all the projects that don't have an artefacts or working folder and move them
                var projectFoldersToMove = FindProjectFoldersToMove(document);
                if (projectFoldersToMove.Count == 0)
                {
                    FireMessage("No project folders found", MigrationEventType.Information);
                }
                else
                {
                    FireMessage(string.Format("{0} project folder(s) found", projectFoldersToMove.Count), MigrationEventType.Information);
                    foreach (var projectName in projectFoldersToMove)
                    {
                        var oldProjectPath = Path.Combine(MigrationOptions.CurrentServerLocation, projectName);
                        var newProjectPath = Path.Combine(MigrationOptions.NewServerLocation, projectName);
                        MoveFolder(oldProjectPath, 
                            newProjectPath,
                            string.Format("Project folder for '{0}' moved", projectName),
                            string.Format("Project folder for '{0}' restored", projectName));
                    }
                }

                // Move the project state files
                var projectStatesToMove = FindProjectStatesToMove(document);
                if (projectStatesToMove.Count == 0)
                {
                    FireMessage("No project state files found", MigrationEventType.Information);
                }
                else
                {
                    FireMessage(string.Format("{0} project state file(s) found", projectStatesToMove.Count), MigrationEventType.Information);
                    foreach (var projectName in projectStatesToMove)
                    {
                        var oldProjectPath = Path.Combine(MigrationOptions.CurrentServerLocation, projectName + ".state");
                        var newProjectPath = Path.Combine(MigrationOptions.NewServerLocation, projectName + ".state");
                        MoveFile(oldProjectPath,
                            newProjectPath,
                            string.Format("Project state file for '{0}' moved", projectName),
                            string.Format("Project state file for '{0}' restored", projectName));
                    }
                }

                // Move the project state file
                var stateFile = Path.Combine(MigrationOptions.CurrentServerLocation, "ProjectsState.xml");
                if (File.Exists(stateFile))
                {
                    MoveFile(stateFile,
                        Path.Combine(MigrationOptions.NewServerLocation, "ProjectsState.xml"),
                        "Project state file moved",
                        "Project state file restored");
                }

                FireMessage("Server files successfully migrated", MigrationEventType.Information);
                isSuccessful = true;
            }
            catch (Exception error)
            {
                FireMessage("An unexpected error has occurred while migrating server files: " + error.Message, MigrationEventType.Error);
            }
            FireMessage("Server migration completed " + (isSuccessful ? "without errors" : "with errors"), MigrationEventType.Status);
            return isSuccessful;
        }
        #endregion

        #region FindProjectFoldersToMove()
        private List<string> FindProjectFoldersToMove(XmlDocument document)
        {
            FireMessage("Searching for project folders to migrate", MigrationEventType.Information);
            var projectFoldersToMove = new List<string>();
            foreach (XmlElement projectEl in document.SelectNodes("/cruisecontrol/project"))
            {
                var hasWorking = !string.IsNullOrEmpty(projectEl.GetAttribute("workingDirectory")) ||
                    (projectEl.SelectSingleNode("workingDirectory") != null);
                var hasArtefact = !string.IsNullOrEmpty(projectEl.GetAttribute("artifactDirectory")) ||
                    (projectEl.SelectSingleNode("artifactDirectory") != null);

                if (!hasArtefact || !hasWorking)
                {
                    var projectName = projectEl.GetAttribute("name");
                    if (string.IsNullOrEmpty(projectName))
                    {
                        var nameEl = projectEl.SelectSingleNode("name");
                        if (nameEl != null) projectName = nameEl.InnerText;
                    }

                    // Since we are not validating the XML, there is the possibility of invalid XML (i.e. no name)
                    if (!string.IsNullOrEmpty(projectName))
                    {
                        var projectPath = Path.Combine(MigrationOptions.CurrentServerLocation, projectName);
                        if (Directory.Exists(projectPath)) projectFoldersToMove.Add(projectName);
                    }
                }
            }
            return projectFoldersToMove;
        }
        #endregion

        #region FindProjectStatesToMove()
        private List<string> FindProjectStatesToMove(XmlDocument document)
        {
            FireMessage("Searching for project state files to migrate", MigrationEventType.Information);
            var projectStatesToMove = new List<string>();
            foreach (XmlElement projectEl in document.SelectNodes("/cruisecontrol/project"))
            {
                var projectName = projectEl.GetAttribute("name");
                if (string.IsNullOrEmpty(projectName))
                {
                    var nameEl = projectEl.SelectSingleNode("name");
                    if (nameEl != null) projectName = nameEl.InnerText;
                }

                // Since we are not validating the XML, there is the possibility of invalid XML (i.e. no name)
                if (!string.IsNullOrEmpty(projectName))
                {
                    var projectPath = Path.Combine(MigrationOptions.CurrentServerLocation, projectName + ".state");
                    if (File.Exists(projectPath)) projectStatesToMove.Add(projectName);
                }
            }
            return projectStatesToMove;
        }
        #endregion

        #region MoveFolder()
        private void MoveFolder(string oldPath, string newPath, string completedMessage, string rollbackMessage)
        {
            FireMessage(
                string.Format("Moving folder '{0}' to '{1}'", oldPath, newPath),
                MigrationEventType.Information);
            if (Directory.Exists(newPath))
            {
                FireMessage(
                    string.Format("Folder '{0}' already exists - unable to move", newPath),
                    MigrationEventType.Warning);
            }
            else
            {
                Directory.Move(oldPath, newPath);
                FireMessage(completedMessage, MigrationEventType.Information);
                rollbackActions.Push(s =>
                {
                    FireMessage(
                        string.Format("Moving folder '{0}' to '{1}'", newPath, oldPath),
                        MigrationEventType.Information);
                    Directory.Move(newPath, oldPath);
                    FireMessage(rollbackMessage, MigrationEventType.Information);
                });
            }
        }
        #endregion

        #region MoveFile()
        private void MoveFile(string oldPath, string newPath, string completedMessage, string rollbackMessage)
        {
            FireMessage(
                string.Format("Moving file '{0}' to '{1}'", oldPath, newPath),
                MigrationEventType.Information);
            if (File.Exists(newPath))
            {
                FireMessage(
                    string.Format("File '{0}' already exists - unable to move", newPath),
                    MigrationEventType.Warning);
            }
            else
            {
                File.Move(oldPath, newPath);
                FireMessage(completedMessage, MigrationEventType.Information);
                rollbackActions.Push(s =>
                {
                    FireMessage(
                        string.Format("Moving file '{0}' to '{1}'", newPath, oldPath),
                        MigrationEventType.Information);
                    File.Move(newPath, oldPath);
                    FireMessage(rollbackMessage, MigrationEventType.Information);
                });
            }
        }
        #endregion

        #region EnsureFolderExists()
        private void EnsureFolderExists(string folderPath, string name)
        {
            if (!Directory.Exists(folderPath))
            {
                FireMessage("Creating " + name + " folder: " + folderPath, MigrationEventType.Information);
                Directory.CreateDirectory(folderPath);
                rollbackActions.Push(s =>
                {
                    FireMessage("Deleting " + name + " folder: " + folderPath, MigrationEventType.Information);
                    Directory.Delete(folderPath, true);
                    FireMessage("Server folder deleted", MigrationEventType.Information);
                });
                FireMessage(CapitialiseName(name) + " folder created", MigrationEventType.Information);
            }
            else
            {
                FireMessage(CapitialiseName(name) + " folder already exists: " + folderPath, MigrationEventType.Information);
            }
        }
        #endregion

        #region CapitialiseName()
        private string CapitialiseName(string name)
        {
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
        #endregion

        #region MigrateConfiguration()
        private bool MigrateConfiguration()
        {
            var isSuccessful = false;
            try
            {
                FireMessage("Migrating configuration", MigrationEventType.Information);

                FireMessage("Configuration files successfully migrated", MigrationEventType.Information);
                isSuccessful = true;
            }
            catch (Exception error)
            {
                FireMessage("An unexpected error has occurred while migrating configuration: " + error.Message, MigrationEventType.Error);
            }
            FireMessage("Configuration migration completed " + (isSuccessful ? "without errors" : "with errors"), MigrationEventType.Status);
            return isSuccessful;
        }
        #endregion

        #region MigrateWebDashboardFiles()
        private bool MigrateWebDashboardFiles()
        {
            var isSuccessful = false;
            try
            {
                FireMessage("Migrating web dashboard files", MigrationEventType.Information);

                // Make sure the folder exists
                EnsureFolderExists(MigrationOptions.NewWebDashboardLocation, "web dashboard");

                // Move the dashboard configuration
                var stateFile = Path.Combine(MigrationOptions.CurrentWebDashboardLocation, "dashboard.config");
                if (File.Exists(stateFile))
                {
                    MoveFile(stateFile,
                        Path.Combine(MigrationOptions.NewWebDashboardLocation, "dashboard.config"),
                        "Dashboard configuration moved",
                        "Dashboard configuration restored");
                }

                // Move the packages folder
                var packagesFolder = Path.Combine(MigrationOptions.CurrentWebDashboardLocation, "packages");
                if (Directory.Exists(packagesFolder))
                {
                    MoveFolder(packagesFolder,
                        Path.Combine(MigrationOptions.NewWebDashboardLocation, "packages"),
                        "Packages folder moved",
                        "Packages folder restored");
                }

                FireMessage("Web dashboard files successfully migrated", MigrationEventType.Information);
                isSuccessful = true;
            }
            catch (Exception error)
            {
                FireMessage("An unexpected error has occurred while migrating web dashboard files: " + error.Message, MigrationEventType.Error);
            }
            FireMessage("Web dashboard migration completed " + (isSuccessful ? "without errors" : "with errors"), MigrationEventType.Status);
            return isSuccessful;
        }
        #endregion
        #endregion
    }
}
