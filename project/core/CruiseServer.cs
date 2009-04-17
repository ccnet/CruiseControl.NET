using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServer
        : CruiseServerEventsBase, ICruiseServer
	{
		private readonly IProjectSerializer projectSerializer;
		private readonly IConfigurationService configurationService;
        private IConfiguration configuration;
		private readonly ICruiseManager manager;
		// TODO: Why the monitor? What reentrancy do we have? davcamer, dec 24 2008
		private readonly ManualResetEvent monitor = new ManualResetEvent(true);
        private ISecurityManager securityManager;
        private readonly List<ICruiseServerExtension> extensions = new List<ICruiseServerExtension>();

		private bool disposed;
		private IQueueManager integrationQueueManager;

		public CruiseServer(IConfigurationService configurationService,
		                    IProjectIntegratorListFactory projectIntegratorListFactory, 
                            IProjectSerializer projectSerializer,
                            IProjectStateManager stateManager,
                            List<ExtensionConfiguration> extensionList)
		{
			this.configurationService = configurationService;
			this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
			this.projectSerializer = projectSerializer;

			// ToDo - get rid of manager, maybe
			manager = new CruiseManager(this);
			InitializeServerThread();

			configuration = configurationService.Load();
			integrationQueueManager = IntegrationQueueManagerFactory.CreateManager(projectIntegratorListFactory, configuration, stateManager);
            integrationQueueManager.AssociateIntegrationEvents(OnIntegrationStarted, OnIntegrationCompleted);

            securityManager = configuration.SecurityManager;

            // Load the extensions
            if (extensionList != null)
            {
                InitialiseExtensions(extensionList);
            }
        }

        #region Integration pass-through events
        /// <summary>
        /// Pass this event onto any listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnIntegrationStarted(object sender, IntegrationStartedEventArgs args)
        {
            FireIntegrationStarted(args.Request, args.ProjectName);
        }

        /// <summary>
        /// Pass this event onto any listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnIntegrationCompleted(object sender, IntegrationCompletedEventArgs args)
        {
            FireIntegrationCompleted(args.Request, args.ProjectName, args.Status);
        }
        #endregion

        public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			monitor.Reset();
			integrationQueueManager.StartAllProjects();
            Log.Info("Initialising security");
            securityManager.Initialise();

            // Start the extensions
            Log.Info("Starting extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Start();
            }
        }

		/// <summary>
		/// Start integrator for specified project. 
		/// </summary>
		public void Start(string sessionToken, string project)
		{
            if (!FireProjectStarting(project))
            {
	            CheckSecurity(sessionToken, project, SecurityPermission.StartProject, SecurityEvent.StartProject);
                integrationQueueManager.Start(project);
                FireProjectStarted(project);
            }
		}

		/// <summary>
		/// Stop all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Stop()
		{
            // Stop the extensions
            Log.Info("Stopping extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Stop();
            }
            
            Log.Info("Stopping CruiseControl.NET Server");
			integrationQueueManager.StopAllProjects();
			monitor.Set();
		}

		/// <summary>
		/// Stop integrator for specified project. 
		/// </summary>
		public void Stop(string sessionToken, string project)
		{
            if (!FireProjectStopping(project))
            {
            	CheckSecurity(sessionToken, project, SecurityPermission.StartProject, SecurityEvent.StopProject);
                integrationQueueManager.Stop(project);
                FireProjectStopped(project);
            }
		}

		/// <summary>
		/// Abort all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Abort()
		{
            // Abort the extensions
            Log.Info("Aborting extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Abort();
            }
            
            Log.Info("Aborting CruiseControl.NET Server");
			integrationQueueManager.Abort();
			monitor.Set();
		}

		/// <summary>
		/// Restart server by stopping all integrators, creating a new set of integrators from Configuration and then starting them.
		/// </summary>
		public void Restart()
		{
			Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

			configuration = configurationService.Load();
			integrationQueueManager.Restart(configuration);
            securityManager = configuration.SecurityManager;
            securityManager.Initialise();
		}

		/// <summary>
		/// Block thread until all integrators to have been stopped or aborted.
		/// </summary>
		public void WaitForExit()
		{
			monitor.WaitOne();
		}

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
        public void CancelPendingRequest(string sessionToken, string projectName)
		{
            CheckSecurity(sessionToken, projectName, SecurityPermission.ForceBuild, SecurityEvent.CancelRequest);
			integrationQueueManager.CancelPendingRequest(projectName);
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return integrationQueueManager.GetCruiseServerSnapshot();
		}

		public ICruiseManager CruiseManager
		{
			get { return manager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return integrationQueueManager.GetProjectStatuses();
		}

		public void ForceBuild(string sessionToken, string projectName, string enforcerName)
		{
            if (!FireForceBuildReceived(projectName, enforcerName))
            {
            	string displayName = CheckSecurity(sessionToken, projectName, SecurityPermission.ForceBuild, SecurityEvent.ForceBuild);
            	if (!string.IsNullOrEmpty(displayName)) enforcerName = displayName;
                integrationQueueManager.ForceBuild(projectName, enforcerName);
                FireForceBuildProcessed(projectName, enforcerName);
            }
		}

		public void AbortBuild(string sessionToken, string projectName, string enforcerName)
		{
            if (!FireAbortBuildReceived(projectName, enforcerName))
            {
            	string displayName = CheckSecurity(sessionToken, projectName, SecurityPermission.ForceBuild, SecurityEvent.AbortBuild);
            	if (!string.IsNullOrEmpty(displayName)) enforcerName = displayName;
                GetIntegrator(projectName).AbortBuild(enforcerName);
                FireAbortBuildProcessed(projectName, enforcerName);
            }
        }
		
		public void WaitForExit(string projectName)
		{
			integrationQueueManager.WaitForExit(projectName);
		}

        public void Request(string sessionToken, string project, IntegrationRequest request)
        {
            if (!FireForceBuildReceived(project, request.Source))
            {
            	string displayName = CheckSecurity(sessionToken, project, SecurityPermission.ForceBuild, SecurityEvent.ForceBuild);
            	if (!string.IsNullOrEmpty(displayName))
            	{
                	request = new IntegrationRequest(request.BuildCondition, displayName);
            	}
                integrationQueueManager.Request(project, request);
                FireForceBuildProcessed(project, request.Source);
            }
        }

		public string GetLatestBuildName(string projectName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetLatestBuildName();
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetMostRecentBuildNames(buildCount);
		}

		public string[] GetBuildNames(string projectName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetBuildNames();
		}

		public string GetLog(string projectName, string buildName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetBuildLog(buildName);
		}

		public string GetServerLog()
		{
			return new ServerLogFileReader().Read();
		}

		public string GetServerLog(string projectName)
		{
			return new ServerLogFileReader().Read(projectName);
		}

		// ToDo - test

		public void AddProject(string serializedProject)
		{
			Log.Info("Adding project - " + serializedProject);
			try
			{
				IConfiguration configuration = configurationService.Load();
				IProject project = projectSerializer.Deserialize(serializedProject);
				configuration.AddProject(project);
				project.Initialize();
				configurationService.Save(configuration);
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		// ToDo - test
		// ToDo - when we decide how to handle configuration changes, do more here (like stopping/waiting for project, returning asynchronously, etc.)

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory,
		                          bool purgeSourceControlEnvironment)
		{
			Log.Info("Deleting project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge(purgeWorkingDirectory, purgeArtifactDirectory,
				                                          purgeSourceControlEnvironment);
				configuration.DeleteProject(projectName);
				configurationService.Save(configuration);
			}
			catch (Exception e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		// ToDo - this done TDD

		public string GetProject(string name)
		{
			Log.Info("Getting project - " + name);
			return new NetReflectorProjectSerializer().Serialize(configurationService.Load().Projects[name]);
		}

		public string GetVersion()
		{
			Log.Info("Returning version number");
			try
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to get project version . Exception was - " + e.Message);
			}
		}

		// ToDo - this done TDD
		// ToDo - really delete working dir? What if SCM hasn't changed?

		public void UpdateProject(string projectName, string serializedProject)
		{
			Log.Info("Updating project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge(true, false, true);
				configuration.DeleteProject(projectName);
				IProject project = projectSerializer.Deserialize(serializedProject);
				configuration.AddProject(project);
				project.Initialize();
				configurationService.Save(configuration);
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return LookupProject(projectName).ExternalLinks;
		}

		private IProject LookupProject(string projectName)
		{
			return GetIntegrator(projectName).Project;
		}

        public void SendMessage(string sessionToken, string projectName, Message message)
		{
            if (!FireSendMessageReceived(projectName, message))
            {
            	CheckSecurity(sessionToken, projectName, SecurityPermission.SendMessage, SecurityEvent.SendMessage);
                Log.Info("New message received: " + message);
                LookupProject(projectName).AddMessage(message);
                FireSendMessageProcessed(projectName, message);
            }
		}

		public string GetArtifactDirectory(string projectName)
		{
			return LookupProject(projectName).ArtifactDirectory;
		}

		public string GetStatisticsDocument(string projectName)
		{
			return GetIntegrator(projectName).Project.Statistics;
		}


        public string GetModificationHistoryDocument(string projectName)
        {
            return GetIntegrator(projectName).Project.ModificationHistory;
        }


        public string GetRSSFeed(string projectName)
        {
            return GetIntegrator(projectName).Project.RSSFeed;
        }


		private IProjectIntegrator GetIntegrator(string projectName)
		{
			return integrationQueueManager.GetIntegrator(projectName);
        }

        #region InitialiseExtensions()
        /// <summary>
        /// Initialise all the extensions for the server.
        /// </summary>
        /// <param name="extensionList">The extensions to load.</param>
        private void InitialiseExtensions(List<ExtensionConfiguration> extensionList)
        {
            foreach (ExtensionConfiguration extensionConfig in extensionList)
            {
                // See if we can find the type
                Type extensionType = Type.GetType(extensionConfig.Type);
                if (extensionType == null) throw new NullReferenceException(string.Format("Unable to find extension '{0}'", extensionConfig.Type));

                // Load and initialise the extension
                ICruiseServerExtension extension = Activator.CreateInstance(extensionType) as ICruiseServerExtension;
                if (extension == null) throw new NullReferenceException(string.Format("Unable to create an instance of '{0}'", extensionType.FullName));
                extension.Initialise(this, extensionConfig);

                // Add to the list of extensions
                extensions.Add(extension);
            }
        }
        #endregion

		void IDisposable.Dispose()
		{
			lock (this)
			{
				if (disposed) return;
				disposed = true;
			}
			Abort();
		}

		private static void InitializeServerThread()
		{
			try
			{
				Thread.CurrentThread.Name = "CCNet Server";
			}
			catch (InvalidOperationException)
			{
				// Thread name has already been set.  This only happens during unit tests.
			}
		}

        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
		/// <returns>The amount of free space in bytes.</returns>
		public long GetFreeDiskSpace()
        {
			//TODO: this is currently a hack
			// this method sould return a collection of drives used by ccnet
			// since each project can be hostet on a different drive.
			// We should determine the drives used by a project
			// (working, artefacs, SCM checkout, build publisher, etc)
			// of each project and return a list of free disc space for every used drive.
        	string drive = ConfigurationManager.AppSettings["DataDrive"];
        	if (string.IsNullOrEmpty(drive))
        	{
        		if (System.IO.Path.DirectorySeparatorChar == '/')
        			drive = "/";
        		else
        			drive = "C:";
        	}

        	IFileSystem fileSystem = new SystemIoFileSystem();
			return fileSystem.GetFreeDiskSpace(drive);
        }

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        public virtual ProjectStatusSnapshot TakeStatusSnapshot(string projectName)
        {
            ProjectStatusSnapshot snapshot = null;

            IProjectIntegrator integrator = GetIntegrator(projectName);
            if (integrator != null)
            {
                // First see if the project has its own associated generator
                if (integrator.Project is IStatusSnapshotGenerator)
                {
                    snapshot = (integrator.Project as IStatusSnapshotGenerator).GenerateSnapshot() 
                        as ProjectStatusSnapshot;
                }
                else
                {
                    // Otherwise generate an overview snapshot (details will not be available)
                    ProjectStatus status = integrator.Project.CreateProjectStatus(integrator);
                    snapshot = new ProjectStatusSnapshot();
                    snapshot.Name = integrator.Project.Name;
                    if (status.Activity.IsBuilding())
                    {
                        snapshot.Status = ItemBuildStatus.Running;
                    }
                    else if (status.Activity.IsPending())
                    {
                        snapshot.Status = ItemBuildStatus.Pending;
                    }
                    else if (status.Activity.IsSleeping())
                    {
                        switch (status.BuildStatus)
                        {
                            case IntegrationStatus.Success:
                                snapshot.Status = ItemBuildStatus.CompletedSuccess;
                                break;
                            case IntegrationStatus.Exception:
                            case IntegrationStatus.Failure:
                                snapshot.Status = ItemBuildStatus.CompletedSuccess;
                                break;
                        }
                    }
                }
            }
            else
            {
                throw new NoSuchProjectException(projectName);
            }

            return snapshot;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(string projectName)
        {
            List<PackageDetails> packages = GetIntegrator(projectName).Project.RetrievePackageList();
            return packages.ToArray();
        }

        /// <summary>
        /// Retrieves the list of packages for a build for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(string projectName, string buildLabel)
        {
            List<PackageDetails> packages = GetIntegrator(projectName).Project.RetrievePackageList(buildLabel);
            return packages.ToArray();
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="source">Where to retrieve the file from.</param>
        public virtual RemotingFileTransfer RetrieveFileTransfer(string project, string fileName, FileTransferSource source)
        {
            if (Path.IsPathRooted(fileName))
            {
                Log.Warning(string.Format("Absolute path requested ('{0}') - request denied", fileName));
                throw new CruiseControlException("Unable to retrieve absolute files - must be relative to the project");
            }
            Log.Debug(
                string.Format("Retrieving file '{0}' from '{1}' ({2})", fileName, project, source));
            IProject sourceProject = GetIntegrator(project).Project;
            string filePath = null;
            switch (source)
            {
                case FileTransferSource.Artefact:
                    filePath = Path.Combine(sourceProject.ArtifactDirectory ?? string.Empty, fileName);
                    break;
                case FileTransferSource.Working:
                    filePath = Path.Combine(sourceProject.WorkingDirectory ?? string.Empty, fileName);
                    break;
            }
            RemotingFileTransfer fileTransfer = null;
            if (File.Exists(filePath))
            {
                fileTransfer = new RemotingFileTransfer(File.OpenRead(filePath));
            }
            return fileTransfer;
        }
        #endregion

        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public string Login(ISecurityCredentials credentials)
        {
            return securityManager.Login(credentials);
        }

        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="sesionToken"></param>
        public void Logout(string sesionToken)
        {
            securityManager.Logout(sesionToken);
        }

        /// <summary>
        /// Checks to see if a session has the required right to perform a permission.
        /// </summary>
        /// <param name="sessionToken">The session to check.</param>
        /// <param name="projectName">The project the permission is for.</param>
        /// <param name="permission">The permission being checked.</param>
        /// <param name="eventType">The event type for logging.</param>
        /// <returns>The display name of the user if the permission is allowed.</returns>
         private string CheckSecurity(string sessionToken, string projectName, SecurityPermission permission, SecurityEvent eventType)
        {
            // Retrieve the project authorisation
            IProjectAuthorisation authorisation = null;
            bool requiresSession = true;
            string displayName = securityManager.GetDisplayName(sessionToken);
            string userName = securityManager.GetUserName(sessionToken);
            if (!string.IsNullOrEmpty(projectName))
            {
                IProjectIntegrator projectIntegrator = GetIntegrator(projectName);
                if ((projectIntegrator != null) &&
                    (projectIntegrator.Project != null) &&
                    (projectIntegrator.Project.Security != null))
                {
                    // The project has been found and it has security
                    authorisation = projectIntegrator.Project.Security;
                    requiresSession = authorisation.RequiresSession;
                }
                else if ((projectIntegrator != null) &&
                    (projectIntegrator.Project != null) &&
                    (projectIntegrator.Project.Security == null))
                {
                    // The project is found, but security is missing - application error
                    string errorMessage = string.Format("Security not found for project {0}", projectName);
                    Log.Error(errorMessage);
                    securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Deny, errorMessage);
                    throw new SecurityException(errorMessage);
                }
                else
                {
                    // Couldn't find the requested project
                    string errorMessage = string.Format("project not found {0}", projectName);
                    Log.Error(errorMessage);
                    securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Deny, errorMessage);
                    throw new NoSuchProjectException(projectName);
                }
            }

            if (!requiresSession || (userName != null))
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    // Checking server-level security
                    if (!securityManager.CheckServerPermission(userName, permission))
                    {
                        string info = string.Format("{2} [{0}] has been denied {1} permission at the server",
                            userName, permission, displayName);
                        Log.Warning(info);
                        securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Deny, info);
                        throw new PermissionDeniedException(permission.ToString());
                    }
                    else
                    {
                        string info = string.Format("{2} [{0}] has been granted {1} permission at the server",
                            userName, permission, displayName);
 
                        Log.Debug(info);

                        securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Allow, info);
                        return displayName;
                    }
                }
                else
                {
                    // Checking project-level security
                    if (!authorisation.CheckPermission(securityManager, userName, permission))
                    {
                        string info = string.Format("{3} [{0}] has been denied {1} permission on '{2}'",
                            userName, permission, projectName, displayName); 
                        Log.Warning(info);
                        securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Deny, info);

                        throw new PermissionDeniedException(permission.ToString());
                    }
                    else
                    {
                        Log.Debug(string.Format("{3} [{0}] has been granted {1} permission on '{2}'",
                            userName,
                            permission,
                            projectName,
                            displayName));
                        securityManager.LogEvent(projectName, userName, eventType, SecurityRight.Allow, null);
                        return displayName;
                    }
                }
            }
            else
            {
                // Tell the user that the session is unknown
                Log.Warning(string.Format("Session with token '{0}' is not valid", sessionToken));
                securityManager.LogEvent(projectName, null, eventType, SecurityRight.Deny, "EVENT_SessionNotFound");
                throw new SessionInvalidException();
            }
        }
       

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers(string sessionToken)
        {
            Log.Info("Listing users");
            CheckSecurity(sessionToken, string.Empty, SecurityPermission.ViewSecurity, SecurityEvent.ListAllUsers);
            return securityManager.ListAllUsers();
        }

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string sessionToken, string userName, params string[] projectNames)
        {
            CheckSecurity(sessionToken, string.Empty, SecurityPermission.ViewSecurity, SecurityEvent.DiagnoseSecurityPermissions);
            List<SecurityCheckDiagnostics> diagnoses = new List<SecurityCheckDiagnostics>();

            Array permissions = Enum.GetValues(typeof(SecurityPermission));
            foreach (string projectName in projectNames)
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    Log.Info(string.Format("DiagnoseServerPermission for user {0}", userName));
                }
                else
                {
                    Log.Info(string.Format("DiagnoseProjectPermission for user {0} project {1}", userName, projectName));
                }
                foreach (SecurityPermission permission in permissions)
                {
                    SecurityCheckDiagnostics diagnostics = new SecurityCheckDiagnostics();
                    diagnostics.Permission = permission.ToString();
                    diagnostics.Project = projectName;
                    diagnostics.User = userName;
                    diagnostics.IsAllowed = DiagnosePermission(userName, projectName, permission);
                    diagnoses.Add(diagnostics);
                }
            }

            return diagnoses;
        }

        /// <summary>
        /// Checks to see if a session has the required right to perform a permission.
        /// </summary>
        /// <param name="userName">The user to check.</param>
        /// <param name="projectName">The project the permission is for.</param>
        /// <param name="permission">The permission being checked.</param>
        /// <returns>True if the permission is allowed, false otherwise.</returns>
        private bool DiagnosePermission(string userName, string projectName, SecurityPermission permission)
        {
            bool isAllowed = false;
            if (userName != null)
            {
                IProjectIntegrator projectIntegrator = GetIntegrator(projectName);
                if ((projectIntegrator != null) &&
                    (projectIntegrator.Project != null) &&
                    (projectIntegrator.Project.Security != null))
                {
                    IProjectAuthorisation authorisation = projectIntegrator.Project.Security;
                    isAllowed = authorisation.CheckPermission(securityManager, userName, permission);
                }
            }
            return isAllowed;
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords)
        {
            CheckSecurity(sessionToken, string.Empty, SecurityPermission.ViewSecurity, SecurityEvent.ViewAuditLog);
            return securityManager.ReadAuditRecords(startPosition, numberOfRecords);
        }

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords, IAuditFilter filter)
        {
            CheckSecurity(sessionToken, string.Empty, SecurityPermission.ViewSecurity, SecurityEvent.ViewAuditLog);
            return securityManager.ReadAuditRecords(startPosition, numberOfRecords, filter);
        }

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            string displayName = securityManager.GetDisplayName(sessionToken);
            Log.Debug(string.Format("Changing password for '{0}'", displayName));
            securityManager.ChangePassword(sessionToken, oldPassword, newPassword);
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            string displayName = securityManager.GetDisplayName(sessionToken);
            Log.Debug(string.Format("'{0}' is resetting password for '{1}'", displayName, userName));
            securityManager.ResetPassword(sessionToken, userName, newPassword);
        }
        #endregion

        public virtual string GetSecurityConfiguration(string sessionToken)
        {
            Log.Info("GetSecurityConfiguration");
            CheckSecurity(sessionToken, string.Empty, SecurityPermission.ViewSecurity, SecurityEvent.GetSecurityConfiguration);
            ServerSecurityConfigurationInformation config = new ServerSecurityConfigurationInformation();
            config.Manager = securityManager;
            foreach (IProject project in configuration.Projects)
            {
                config.AddProject(project);
            }
            return config.ToString();
        }
	}
}
