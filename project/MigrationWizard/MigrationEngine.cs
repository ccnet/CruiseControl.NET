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
        private Version versionNumber;
        #endregion

        #region Version numbers
        private readonly Version version140 = new Version(1, 4, 0);
        private readonly Version version141 = new Version(1, 4, 1);
        private readonly Version version142 = new Version(1, 4, 2);
        private readonly Version version143 = new Version(1, 4, 3);
        private readonly Version version144 = new Version(1, 4, 4);
        private readonly Version version150 = new Version(1, 5, 0);
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

            versionNumber = new Version(MigrationOptions.CurrentVersion);

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

                if (versionNumber < version150)
                {
                    // Make sure the folder exists
                    EnsureFolderExists(MigrationOptions.NewServerLocation, "server");
                }

                // Load the configuration
                var document = new XmlDocument();
                document.Load(MigrationOptions.ConfigurationLocation);

                if (versionNumber < version150)
                {
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
                }

                // Update the configuration files
                var logLocation = Path.Combine(MigrationOptions.NewServerLocation, "ccnet.log");
                UpdateServerConfigurationFile(logLocation, "ccnet.exe.config");
                UpdateServerConfigurationFile(logLocation, "ccservice.exe.config");

                // Update any statistics files
                if (versionNumber < version143)
                {
                    var statisticsFiles = FindStatisticsFiles(document);
                    if (statisticsFiles.Count == 0)
                    {
                        FireMessage("No statistics files found", MigrationEventType.Information);
                    }
                    else
                    {
                        FireMessage(string.Format("{0} statistics file(s) found", statisticsFiles.Count), MigrationEventType.Information);
                        foreach (var statisticsFile in statisticsFiles)
                        {
                            FireMessage(string.Format("Migrating {0}", statisticsFile), MigrationEventType.Information);
                            var lines = File.ReadAllLines(statisticsFile);
                            using (var output = File.CreateText(statisticsFile))
                            {
                                foreach (var line in lines)
                                {
                                    if (!string.Equals(line, "<statistics>") &&
                                        !string.Equals(line, "</statistics>"))
                                    {
                                        output.WriteLine(line);
                                    }
                                }
                                output.Flush();
                            }
                        }
                    }
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

        #region UpdateServerConfigurationFile()
        private void UpdateServerConfigurationFile(string logLocation, string configFileName)
        {
            FireMessage("Updating server configuration file: " + configFileName, MigrationEventType.Information);
            var changeCount = 0;

            var configFile = Path.Combine(MigrationOptions.CurrentServerLocation, configFileName);
            if (MigrationOptions.BackupServerConfiguration)
            {
                BackupFile(configFile);
            }
            var configDocument = new XmlDocument();
            configDocument.Load(configFile);

            if (versionNumber < version150)
            {
                changeCount += UpdateAttribute(configDocument, "/configuration/appSettings/add[@key='ServerLogFilePath']", "value", "ccnet.log", logLocation) ? 1 : 0;
                changeCount += UpdateAttribute(configDocument, "/configuration/log4net/appender[@name='RollingFileAppender']/file", "value", "ccnet.log", logLocation) ? 1 : 0;
            }
            if (changeCount > 0)
            {
                configDocument.Save(configFile);
                FireMessage("Server configuration file updated", MigrationEventType.Information);
            }
            else
            {
                FireMessage("Server configuration file not changed - up to date", MigrationEventType.Information);
            }
        }
        #endregion

        #region UpdateNode()
        private bool UpdateAttribute(XmlDocument configDocument, string xpath, string attribute, string oldValue, string newValue)
        {
            var updated = false;
            var node = configDocument.SelectSingleNode(xpath) as XmlElement;
            if (node != null)
            {
                var nodeValue = node.GetAttribute(attribute);
                if (string.Equals(oldValue, nodeValue))
                {
                    node.SetAttribute(attribute, newValue);
                    updated = true;
                }
            }

            return updated;
        }
        #endregion

        #region BackupFile()
        /// <summary>
        /// Backups a file.
        /// </summary>
        /// <param name="fileName"></param>
        private void BackupFile(string fileName)
        {
            FireMessage("Backing up " + fileName, MigrationEventType.Information);

            var loop = 0;
            var newFile = Path.Combine(Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName) +
                "(Backup)" + 
                Path.GetExtension(fileName));
            while (File.Exists(newFile))
            {
                newFile = Path.Combine(Path.GetDirectoryName(fileName),
                    Path.GetFileNameWithoutExtension(fileName) +
                    string.Format(" (Backup {0})", ++loop) +
                    Path.GetExtension(fileName));
            }

            File.Copy(fileName, newFile);

            FireMessage(
                string.Format("{0} backed up to {1}", Path.GetFileName(fileName), newFile),
                MigrationEventType.Information);
        }
        #endregion

        #region FindProjectNode()
        /// <summary>
        /// Find the parent project node.
        /// </summary>
        /// <param name="itemNode"></param>
        /// <returns></returns>
        private XmlNode FindProjectNode(XmlNode itemNode)
        {
            var node = itemNode.ParentNode;
            while ((node != null) && (node.Name != "project"))
            {
                node = node.ParentNode;
            }
            return node;
        }
        #endregion

        #region FindStatisticsFiles()
        private List<string> FindStatisticsFiles(XmlDocument document)
        {
            FireMessage("Searching for statistics publishers", MigrationEventType.Information);
            var statisticsFiles = new List<string>();
            foreach (XmlElement statisticsEl in document.SelectNodes("//statistics"))
            {
                var projectEl = FindProjectNode(statisticsEl) as XmlElement;
                var artefactFolder = projectEl.GetAttribute("artifactDirectory");
                if (string.IsNullOrEmpty(artefactFolder))
                {
                    var artefactNode = projectEl.SelectSingleNode("artifactDirectory");
                    if (artefactNode != null) artefactFolder = artefactNode.InnerText;
                }

                string fileName;
                if (!string.IsNullOrEmpty(artefactFolder))
                {
                    fileName = Path.Combine(artefactFolder, "report.xml");
                }
                else
                {
                    var projectName = projectEl.GetAttribute("name");
                    if (string.IsNullOrEmpty(projectName)) projectName = projectEl.SelectSingleNode("name").InnerText;
                    fileName = Path.Combine(
                            Path.Combine(MigrationOptions.NewServerLocation, projectName),
                            Path.Combine("Artifacts", "report.xml"));
                }
                if (File.Exists(fileName))
                {
                    statisticsFiles.Add(fileName);
                }
            }
            return statisticsFiles;
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

                // Backup the configuration
                if (MigrationOptions.BackupConfiguration)
                {
                    BackupFile(Path.Combine(MigrationOptions.CurrentServerLocation, "ccnet.config"));
                }

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

                if (versionNumber < version150)
                {
                    // Make sure the folder exists
                    EnsureFolderExists(MigrationOptions.NewWebDashboardLocation, "web dashboard");
                }

                if (versionNumber < version150)
                {
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
