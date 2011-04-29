// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCmdletProvider.cs" company="The CruiseControl.NET Team">
//   Copyright (C) 2011 by The CruiseControl.NET Team
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.PowerShell
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Provider;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Exposes the client settings as a provider in PowerShell.
    /// </summary>
    [CmdletProvider("CCNet", ProviderCapabilities.None)]
    public class ClientCmdletProvider
        : NavigationCmdletProvider
    {
        #region Private fields
        /// <summary>
        /// The root path.
        /// </summary>
        private const string RootPath = "\\";

        /// <summary>
        /// The path for queues.
        /// </summary>
        private const string QueuesPath = "\\queues";

        /// <summary>
        /// The path for projects.
        /// </summary>
        private const string ProjectsPath = "\\projects";

        /// <summary>
        /// The current snapshot.
        /// </summary>
        private CruiseServerSnapshot currentSnapshot;

        /// <summary>
        /// The time the current snapshot was taken.
        /// </summary>
        private DateTime currentSnapshotTime;
        #endregion

        #region Protected methods
        #region Start()
        /// <summary>
        /// Starts this provider.
        /// </summary>
        /// <param name="providerInfo">The provider info.</param>
        /// <returns>
        /// The updated <see cref="ProviderInfo"/>.
        /// </returns>
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            WriteVerbose("ClientCmdletProvider::Start()");
            var info = base.Start(providerInfo);
            info.Description = "CruiseControl.NET management provider";
            return info;
        }
        #endregion

        #region InitializeDefaultDrives()
        /// <summary>
        /// Initializes the default drives.
        /// </summary>
        /// <returns>
        /// The default drives.
        /// </returns>
        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            WriteVerbose("ClientCmdletProvider::InitializeDefaultDrives()");
            var drive = new PSDriveInfo("local", this.ProviderInfo, RootPath, "Local CC.NET", PSCredential.Empty);
            var localDrive = new ClientDriveInfo("tcp://localhost:21234", drive);
            var drives = new Collection<PSDriveInfo>
                             {
                                 localDrive
                             };
            return drives;
        }
        #endregion

        #region NewDrive()
        /// <summary>
        /// Initialises a new drive.
        /// </summary>
        /// <param name="drive">The drive.</param>
        /// <returns>
        /// The <see cref="PSDriveInfo"/> information.
        /// </returns>
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            WriteVerbose("ClientCmdletProvider::NewDrive()");
            if (drive is ClientDriveInfo)
            {
                return drive;
            }

            var dynamicParameters = this.DynamicParameters as RuntimeDefinedParameterDictionary;
            var address = dynamicParameters["address"].Value.ToString();
            return new ClientDriveInfo(address, drive);
        }
        #endregion

        #region NewDriveDynamicParameters()
        /// <summary>
        /// Defines the dynamic parameters for a new drive.
        /// </summary>
        /// <returns>
        /// The parameters for the new drive.
        /// </returns>
        protected override object NewDriveDynamicParameters()
        {
            WriteVerbose("ClientCmdletProvider::NewDriveDynamicParameters()");
            var dynamicParameters = new RuntimeDefinedParameterDictionary();
            var attributes = new Collection<Attribute>();
            var parameterAttribute = new ParameterAttribute
                                         {
                                             Mandatory = true
                                         };
            attributes.Add(parameterAttribute);
            dynamicParameters.Add("address", new RuntimeDefinedParameter("address", typeof(string), attributes));
            return dynamicParameters;
        }
        #endregion

        #region IsValidPath()
        /// <summary>
        /// Determines whether a path is valid.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the path is valid; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsValidPath(string path)
        {
            WriteVerbose("ClientCmdletProvider::IsValidPath(path=" + (path ?? string.Empty) + ")");
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            var isValid = IsContainer(path) ||
                path.StartsWith(QueuesPath, StringComparison.InvariantCultureIgnoreCase) ||
                path.StartsWith(QueuesPath, StringComparison.InvariantCultureIgnoreCase);
            return isValid;
        }
        #endregion

        #region ItemExists()
        /// <summary>
        /// Checks if an item exists.
        /// </summary>
        /// <param name="path">The path to the item.</param>
        /// <returns>
        /// <c>true</c> of the item exists; <c>false</c> otherwise.
        /// </returns>
        protected override bool ItemExists(string path)
        {
            WriteVerbose("ClientCmdletProvider::ItemExists(path=" + (path ?? string.Empty) + ")");
            var drive = this.PSDriveInfo as ClientDriveInfo;
            if (drive == null)
            {
                return false;
            }

            var exists = IsContainer(path) ||
                (this.RetrieveQueue(path) != null) ||
                (this.RetrieveProject(path) != null);
            return exists;
        }
        #endregion

        #region GetItem()
        /// <summary>
        /// Gets an item.
        /// </summary>
        /// <param name="path">The path.</param>
        protected override void GetItem(string path)
        {
            WriteVerbose("ClientCmdletProvider::GetItem(path=" + (path ?? string.Empty) + ")");
            var drive = this.PSDriveInfo as ClientDriveInfo;
            if (drive == null)
            {
                return;
            }

            if (IsRootPath(path))
            {
                this.WriteItemObject(drive.RootFolder, path, true);
            }
            else if (IsContainer(path))
            {
                this.WriteItemObject(new ClientFolder(path), path, true);
            }
            else
            {
                var queue = this.RetrieveQueue(path);
                if (queue != null)
                {
                    this.WriteItemObject(queue, this.MakePath(path, queue.QueueName), false);
                }

                var project = this.RetrieveProject(path);
                if (project != null)
                {
                    this.WriteItemObject(Project.Wrap(drive.Client, project), this.MakePath(path, project.Name), false);
                }
            }
        }
        #endregion

        #region HasChildItems()
        /// <summary>
        /// Determines whether the item at path has any child items.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if [has child items] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool HasChildItems(string path)
        {
            WriteVerbose("ClientCmdletProvider::HasChildItems(path=" + (path ?? string.Empty) + ")");

            return IsContainer(path);
        }
        #endregion

        #region GetChildNames
        /// <summary>
        /// Gets the child names.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="returnContainers">Whether to return containers or not.</param>
        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            WriteVerbose("ClientCmdletProvider::GetChildNames(path=" + (path ?? string.Empty) + ", returnContainers=" + returnContainers + ")");

            if (IsRootPath(path))
            {
                WriteItemObject(ProjectsPath, path, true);
                WriteItemObject(QueuesPath, path, true);
            }
            else if (IsQueuesPath(path))
            {
                var snapshot = this.RetrieveSnapshot();
                if (snapshot != null)
                {
                    foreach (var queue in snapshot.QueueSetSnapshot.Queues)
                    {
                        this.WriteItemObject(queue.QueueName, path, false);
                    }
                }
            }
            else if (IsProjectsPath(path))
            {
                var snapshot = this.RetrieveSnapshot();
                if (snapshot != null)
                {
                    foreach (var project in snapshot.ProjectStatuses)
                    {
                        this.WriteItemObject(project.Name, path, false);
                    }
                }
            }
        }
        #endregion

        #region GetChildItems
        /// <summary>
        /// Gets the child items.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="recurse">Whether to recurse containers or not.</param>
        protected override void GetChildItems(string path, bool recurse)
        {
            WriteVerbose("ClientCmdletProvider::GetChildItems(path=" + (path ?? string.Empty) + ", recurse=" + recurse + ")");
            if (path == null)
            {
                return;
            }

            var drive = this.PSDriveInfo as ClientDriveInfo;
            if (drive == null)
            {
                return;
            }

            if (IsRootPath(path))
            {
                this.WriteItemObject(new ClientFolder(ProjectsPath), ProjectsPath, true);
                this.WriteItemObject(new ClientFolder(QueuesPath), QueuesPath, true);
            }
            else if (IsQueuesPath(path))
            {
                var snapshot = this.RetrieveSnapshot();
                if (snapshot != null)
                {
                    foreach (var queue in snapshot.QueueSetSnapshot.Queues)
                    {
                        this.WriteItemObject(queue, this.MakePath(path, queue.QueueName), false);
                    }
                }
            }
            else if (IsProjectsPath(path))
            {
                var snapshot = this.RetrieveSnapshot();
                if (snapshot != null)
                {
                    foreach (var project in snapshot.ProjectStatuses)
                    {
                        this.WriteItemObject(Project.Wrap(drive.Client, project), this.MakePath(path, project.Name), false);
                    }
                }
            }
        }
        #endregion

        #region IsItemContainer
        /// <summary>
        /// Determines whether the path is an item container or not.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the path is an item container; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsItemContainer(string path)
        {
            WriteVerbose("ClientCmdletProvider::IsItemContainer(path=" + (path ?? string.Empty) + ")");
            if (path == null)
            {
                return false;
            }

            return IsContainer(path);
        }
        #endregion
        #endregion

        #region Private methods
        #region IsContainer()
        /// <summary>
        /// Determines whether the specified path is a container.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the specified path is a container; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsContainer(string path)
        {
            return IsRootPath(path) ||
                   IsQueuesPath(path) ||
                   IsProjectsPath(path);
        }
        #endregion

        #region IsRootPath()
        /// <summary>
        /// Determines whether a path is the root path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the path is the root; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsRootPath(string path)
        {
            return path.Equals(RootPath, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region IsQueuesPath()
        /// <summary>
        /// Determines whether a path is the queues path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the path is the queues path; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsQueuesPath(string path)
        {
            return path.Equals(QueuesPath, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region IsProjectsPath()
        /// <summary>
        /// Determines whether a path is the projects path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the path is the projects; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsProjectsPath(string path)
        {
            return path.Equals(ProjectsPath, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the server.
        /// </summary>
        /// <returns>
        /// The current server snapshot.
        /// </returns>
        private CruiseServerSnapshot RetrieveSnapshot()
        {
            if ((this.currentSnapshot == null) || ((DateTime.Now - this.currentSnapshotTime).TotalSeconds > 1))
            {
                var drive = this.PSDriveInfo as ClientDriveInfo;
                if (drive == null)
                {
                    return null;
                }

                this.currentSnapshot = drive.Client.GetCruiseServerSnapshot();
                this.currentSnapshotTime = DateTime.Now;
            }

            return this.currentSnapshot;
        }
        #endregion

        #region RetrieveQueue()
        /// <summary>
        /// Retrieves a queue.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="QueueSnapshot"/> if the path is valid; <c>null</c> otherwise.
        /// </returns>
        private QueueSnapshot RetrieveQueue(string path)
        {
            if (!path.StartsWith(QueuesPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var queueName = path.Substring(path.LastIndexOf('\\') + 1);
            var snapshot = this.RetrieveSnapshot();
            var queue = snapshot
                .QueueSetSnapshot
                .Queues
                .FirstOrDefault(q => q.QueueName.Equals(queueName, StringComparison.InvariantCultureIgnoreCase));
            return queue;
        }
        #endregion

        #region RetrieveProject()
        /// <summary>
        /// Retrieves a project.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="ProjectStatus"/> if the path is valid; <c>null</c> otherwise.
        /// </returns>
        private ProjectStatus RetrieveProject(string path)
        {
            if (!path.StartsWith(ProjectsPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var projectName = path.Substring(path.LastIndexOf('\\') + 1);
            ClientDriveInfo drive;
            var snapshot = this.RetrieveSnapshot();
            var project = snapshot
                .ProjectStatuses
                .FirstOrDefault(q => q.Name.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
            return project;
        }
        #endregion
        #endregion
    }
}
