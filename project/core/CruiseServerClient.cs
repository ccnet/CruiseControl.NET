using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Client for connecting to a remote server instance.
    /// </summary>
    public class CruiseServerClient
        : MarshalByRefObject, ICruiseServerClient
    {
        #region Private fields
        private readonly ICruiseServer cruiseServer;
        private Dictionary<string, Type> messageTypes = null;
        private Dictionary<Type, XmlSerializer> messageSerialisers = new Dictionary<Type, XmlSerializer>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CruiseServerClient"/>.
        /// </summary>
        /// <param name="cruiseServer"></param>
        public CruiseServerClient(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public virtual ProjectStatusResponse GetProjectStatus(ServerRequest request)
        {
            return cruiseServer.GetProjectStatus(request);
        }
        #endregion

        #region Start()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response Start(ProjectRequest request)
        {
            return cruiseServer.Start(request);
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Attempts to stop a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response Stop(ProjectRequest request)
        {
            return cruiseServer.Stop(request);
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response ForceBuild(ProjectRequest request)
        {
            return cruiseServer.ForceBuild(request);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Aborts the build of the selected project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response AbortBuild(ProjectRequest request)
        {
            return cruiseServer.AbortBuild(request);
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public Response CancelPendingRequest(ProjectRequest request)
        {
            return cruiseServer.CancelPendingRequest(request);
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Send a text message to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response SendMessage(MessageRequest request)
        {
            return cruiseServer.SendMessage(request);
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Waits for the project to exit.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Response WaitForExit(ProjectRequest request)
        {
            return cruiseServer.WaitForExit(request);
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public virtual SnapshotResponse GetCruiseServerSnapshot(ServerRequest request)
        {
            return cruiseServer.GetCruiseServerSnapshot(request);
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public virtual DataResponse GetLatestBuildName(ProjectRequest request)
        {
            return cruiseServer.GetLatestBuildName(request);
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual DataListResponse GetBuildNames(ProjectRequest request)
        {
            return cruiseServer.GetBuildNames(request);
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual DataListResponse GetMostRecentBuildNames(BuildListRequest request)
        {
            return cruiseServer.GetMostRecentBuildNames(request);
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        public virtual DataResponse GetLog(BuildRequest request)
        {
            return cruiseServer.GetLog(request);
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
        public virtual DataResponse GetServerLog(ServerRequest request)
        {
            return cruiseServer.GetServerLog(request);
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public virtual Response AddProject(ChangeConfigurationRequest request)
        {
            return cruiseServer.AddProject(request);
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public virtual Response DeleteProject(ChangeConfigurationRequest request)
        {
            return cruiseServer.DeleteProject(request);
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public virtual Response UpdateProject(ChangeConfigurationRequest request)
        {
            return cruiseServer.UpdateProject(request);
        }
        #endregion

        #region GetProject()
        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
        public virtual DataResponse GetProject(ProjectRequest request)
        {
            return cruiseServer.GetProject(request);
        }
        #endregion

        #region GetExternalLinks()
        public virtual ExternalLinksListResponse GetExternalLinks(ProjectRequest request)
        {
            return cruiseServer.GetExternalLinks(request);
        }
        #endregion

        #region GetArtifactDirectory()
        public virtual DataResponse GetArtifactDirectory(ProjectRequest request)
        {
            return cruiseServer.GetArtifactDirectory(request);
        }
        #endregion

        #region GetStatisticsDocument()
        public virtual DataResponse GetStatisticsDocument(ProjectRequest request)
        {
            return cruiseServer.GetStatisticsDocument(request);
        }
        #endregion

        #region GetModificationHistoryDocument()
        public virtual DataResponse GetModificationHistoryDocument(ProjectRequest request)
        {
            return cruiseServer.GetModificationHistoryDocument(request);
        }
        #endregion

        #region GetRSSFeed()
        public virtual DataResponse GetRSSFeed(ProjectRequest request)
        {
            return cruiseServer.GetRSSFeed(request);
        }
        #endregion

        #region GetServerVersion()
        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public virtual DataResponse GetServerVersion(ServerRequest request)
        {
            return cruiseServer.GetServerVersion(request);
        }
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public LoginResponse Login(LoginRequest request)
        {
            return cruiseServer.Login(request);
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="request"></param>
        public Response Logout(ServerRequest request)
        {
            return cruiseServer.Logout(request);
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        /// <param name="request"></param>
        public DataResponse GetSecurityConfiguration(ServerRequest request)
        {
            return cruiseServer.GetSecurityConfiguration(request);
        }
        #endregion

        #region ListUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public ListUsersResponse ListUsers(ServerRequest request)
        {
            return cruiseServer.ListUsers(request);
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A set of diagnostics information.</returns>
        public DiagnoseSecurityResponse DiagnoseSecurityPermissions(DiagnoseSecurityRequest request)
        {
            return cruiseServer.DiagnoseSecurityPermissions(request);
        }
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public ReadAuditResponse ReadAuditRecords(ReadAuditRequest request)
        {
            return cruiseServer.ReadAuditRecords(request);
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to retrieve the parameters for.</param>
        /// <returns>The list of parameters (if any).</returns>
        public BuildParametersResponse ListBuildParameters(ProjectRequest request)
        {
            return cruiseServer.ListBuildParameters(request);
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="request"></param>
        public Response ChangePassword(ChangePasswordRequest request)
        {
            return cruiseServer.ChangePassword(request);
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="request"></param>
        public Response ResetPassword(ChangePasswordRequest request)
        {
            return cruiseServer.ResetPassword(request);
        }
        #endregion

        #region ProcessMessage()
        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message in an XML format.</param>
        /// <returns>The response message in an XML format.</returns>
        public virtual string ProcessMessage(string action, string message)
        {
            Response response = new Response();

            try
            {
                // Find the type of message
                XmlDocument messageXml = new XmlDocument();
                messageXml.LoadXml(message);
                Type messageType = FindMessageType(messageXml.DocumentElement.Name);
                if (messageType == null)
                {
                    throw new CruiseControlException(
                        string.Format(
                            "Unable to translate message: '{0}' is unknown",
                            messageXml.DocumentElement.Name));
                }

                // Find the action
                Type cruiseType = typeof(ICruiseServerClient);
                MethodInfo actionMethod = cruiseType.GetMethod(action,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public);
                if (actionMethod == null)
                {
                    throw new CruiseControlException(
                        string.Format(
                            "Unable to locate action '{0}'",
                            action));
                }

                // Convert the message and invoke the action
                object request = ConvertXmlToObject(messageType, message);
                response = actionMethod.Invoke(this,
                    new object[] {
                        request
                    }) as Response;
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage("Unable to process error: " + error.Message));
            }

            return response.ToString();
        }

        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        public virtual Response ProcessMessage(string action, ServerRequest message)
        {
            Response response = new Response();

            try
            {
                // Find the action
                Type cruiseType = typeof(ICruiseServerClient);
                MethodInfo actionMethod = cruiseType.GetMethod(action,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public);
                if (actionMethod == null)
                {
                    throw new CruiseControlException(
                        string.Format(
                            "Unable to locate action '{0}'",
                            action));
                }

                // Invoke the action
                response = actionMethod.Invoke(this,
                    new object[] {
                        message
                    }) as Response;
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage("Unable to process error: " + error.Message));
            }

            return response;
        }
        #endregion

        #region InitializeLifetimeService()
        /// <summary>
        /// Initialise the lifetime service.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public virtual DataResponse GetFreeDiskSpace(ServerRequest request)
        {
            return cruiseServer.GetFreeDiskSpace(request);
        }
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        public virtual StatusSnapshotResponse TakeStatusSnapshot(ProjectRequest request)
        {
            return cruiseServer.TakeStatusSnapshot(request);
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves a list of packages for a project.
        /// </summary>
        public virtual ListPackagesResponse RetrievePackageList(ProjectRequest request)
        {
            return cruiseServer.RetrievePackageList(request);
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public RemotingFileTransfer RetrieveFileTransfer(string project, string fileName)
        {
            return cruiseServer.RetrieveFileTransfer(project, fileName);
        }
        #endregion
        #endregion

        #region Private methods
        #region FindMessageType()
        /// <summary>
        /// Finds the type of object that a message is.
        /// </summary>
        /// <param name="messageName">The name of the message.</param>
        /// <returns>The message type, if found, null otherwise.</returns>
        private Type FindMessageType(string messageName)
        {
            Type messageType = null;

            // If the message types have not been loaded, load them into a dictionary
            if (messageTypes == null)
            {
                messageTypes = new Dictionary<string, Type>();

                // Messages will only come from the remoting library
                Assembly remotingLibrary = typeof(ICruiseServerClient).Assembly;
                foreach (Type remotingType in remotingLibrary.GetExportedTypes())
                {
                    XmlRootAttribute[] attributes = remotingType.GetCustomAttributes(
                        typeof(XmlRootAttribute), false) as XmlRootAttribute[];
                    foreach (XmlRootAttribute attribute in attributes)
                    {
                        messageTypes.Add(attribute.ElementName, remotingType);
                    }
                }
            }

            // Attempt to find the message within the message types
            if (messageTypes.ContainsKey(messageName))
            {
                messageType = messageTypes[messageName];
            }

            return messageType;
        }
        #endregion

        #region ConvertXmlToObject()
        /// <summary>
        /// Converts a message string into an object.
        /// </summary>
        /// <param name="messageType">The type of message.</param>
        /// <param name="message">The XML of the message.</param>
        /// <returns>The object of the message.</returns>
        private object ConvertXmlToObject(Type messageType, string message)
        {
            object messageObj = null;

            // Make sure the serialiser has been loaded
            if (!messageSerialisers.ContainsKey(messageType))
            {
                messageSerialisers[messageType] = new XmlSerializer(messageType);
            }

            // Perform the actual conversion
            using (StringReader reader = new StringReader(message))
            {
                messageObj = messageSerialisers[messageType].Deserialize(reader);
            }

            return messageObj;
        }
        #endregion
        #endregion
    }
}