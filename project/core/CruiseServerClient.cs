
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Core.Security;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

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
        private readonly IChannelSecurity channelSecurity;
        private Dictionary<string, Type> messageTypes = null;
        private Dictionary<Type, XmlSerializer> messageSerialisers = new Dictionary<Type, XmlSerializer>();
        private Dictionary<string, SecureConnection> connections = new Dictionary<string, SecureConnection>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CruiseServerClient"/>.
        /// </summary>
        /// <param name="cruiseServer"></param>
        public CruiseServerClient(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;

            // Retrieve any associated channel security
            var server = cruiseServer as CruiseServer;
            if ((server != null) &&
                (server.SecurityManager != null))
            {
                channelSecurity = server.SecurityManager.Channel;
            }
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

        #region GetFinalBuildStatus()
        /// <summary>
        /// Gets the final status for a build.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="SnapshotResponse"/> for the build.</returns>
        public virtual StatusSnapshotResponse GetFinalBuildStatus(BuildRequest request)
        {
            return cruiseServer.GetFinalBuildStatus(request);
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
        /// <summary>
        /// Gets the external links.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual ExternalLinksListResponse GetExternalLinks(ProjectRequest request)
        {
            return cruiseServer.GetExternalLinks(request);
        }
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Gets the artifact directory.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DataResponse GetArtifactDirectory(ProjectRequest request)
        {
            return cruiseServer.GetArtifactDirectory(request);
        }
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Gets the statistics document.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DataResponse GetStatisticsDocument(ProjectRequest request)
        {
            return cruiseServer.GetStatisticsDocument(request);
        }
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Gets the modification history document.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DataResponse GetModificationHistoryDocument(ProjectRequest request)
        {
            return cruiseServer.GetModificationHistoryDocument(request);
        }
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Gets the RSS feed.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
        /// A list of <see cref="ListUsersResponse"/> containing the details on all the users
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
        /// <returns>A list of <see cref="ReadAuditResponse"/>s containing the audit details that match the filter.</returns>
        public ReadAuditResponse ReadAuditRecords(ReadAuditRequest request)
        {
            return cruiseServer.ReadAuditRecords(request);
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="request">The project to retrieve the parameters for.</param>
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
            var response = new Response();

            try
            {
                // Find the action and message type, extract the message and invoke the method
                response = ExtractAndInvokeMessage(message, action, new RemotingChannelSecurityInformation());
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage("Unable to process: " + error.Message));
            }

            var responseText = response.ToString();
            return responseText;
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
                Type cruiseType = this.GetType();
                MethodInfo actionMethod = cruiseType.GetMethod(action,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public);
                if (actionMethod == null)
                {
                    throw new CruiseControlException(
                        string.Format(
                            CultureInfo.CurrentCulture, "Unable to locate action '{0}'",
                            action));
                }

                // Invoke the action
                message.ChannelInformation = new RemotingChannelSecurityInformation();
                response = actionMethod.Invoke(this,
                    new object[] {
                        message
                    }) as Response;
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                if ((error is TargetInvocationException) && (error.InnerException != null))
                {
                    error = error.InnerException;
                }
                response.ErrorMessages.Add(
                    new ErrorMessage("Unable to process: " + error.Message));
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
        public FileTransferResponse RetrieveFileTransfer(FileTransferRequest request)
        {
            return cruiseServer.RetrieveFileTransfer(request);
        }
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetLinkedSiteId(ProjectItemRequest request)
        {
            return cruiseServer.GetLinkedSiteId(request);
        }
        #endregion

        #region ProcessSecureRequest()
        /// <summary>
        /// Processes an encrypted request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response ProcessSecureRequest(ServerRequest request)
        {
            // Validate the request
            var encryptedRequest = request as EncryptedRequest;
            if (encryptedRequest == null) throw new CruiseControlException("Incoming request is not an encrypted request");
            if (!connections.ContainsKey(request.SourceName)) throw new CruiseControlException("No secure connection for the source");
            var connection = connections[request.SourceName];

            // Decrypt the data
            var crypto = new RijndaelManaged();
            crypto.Key = connection.Key;
            crypto.IV = connection.IV;
            string data = DecryptMessage(crypto, encryptedRequest.EncryptedData);

            // Find the action and message type, extract the message and invoke the method
            var response = ExtractAndInvokeMessage(data, encryptedRequest.Action,
                new RemotingChannelSecurityInformation { IsEncrypted = true });

            // Encrypt the response
            var encryptedResponse = new EncryptedResponse(request);
            encryptedResponse.EncryptedData = response.ToString();
            encryptedResponse.EncryptedData = EncryptMessage(crypto, encryptedResponse.EncryptedData);
            encryptedResponse.Result = ResponseResult.Success;

            return encryptedResponse;
        }
        #endregion

        #region RetrievePublicKey()
        /// <summary>
        /// Retrieve the public key for the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataResponse RetrievePublicKey(ServerRequest request)
        {
            var response = new DataResponse(request);

            // Either generate or retrieve the key for CruiseControl.NET Server
            var cp = new CspParameters();
            cp.KeyContainerName = "CruiseControl.NET Server";
            var provider = new RSACryptoServiceProvider(cp);

            // Return the public key
            response.Data = provider.ToXmlString(false);
            response.Result = ResponseResult.Success;

            return response;
        }
        #endregion

        #region InitialiseSecureConnection()
        /// <summary>
        /// Initialise a secure communications connection.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response InitialiseSecureConnection(LoginRequest request)
        {
            // Decrypt the password
            var cp = new CspParameters();
            cp.KeyContainerName = "CruiseControl.NET Server";
            var provider = new RSACryptoServiceProvider(cp);
            var originalKey = request.FindCredential(LoginRequest.UserNameCredential).Value;
            var decryptedKey = UTF8Encoding.UTF8.GetString(
                provider.Decrypt(Convert.FromBase64String(originalKey), false));
            var originalIv = request.FindCredential(LoginRequest.PasswordCredential).Value;
            var decryptedIv = UTF8Encoding.UTF8.GetString(
                provider.Decrypt(Convert.FromBase64String(originalIv), false));

            // Generate the connection details
            var connection = new SecureConnection
            {
                Expiry = DateTime.Now.AddMinutes(15),
                IV = Convert.FromBase64String(decryptedIv),
                Key = Convert.FromBase64String(decryptedKey)
            };
            connections.Add(request.SourceName, 
                connection);

            // Generate a response
            var response = new Response(request);
            response.Result = ResponseResult.Success;
            return response;
        }
        #endregion

        #region TerminateSecureConnection()
        /// <summary>
        /// Terminate a secure communications connection.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Response TerminateSecureConnection(ServerRequest request)
        {
            // Remove the connection details
            if (connections.ContainsKey(request.SourceName))
            {
                connections.Remove(request.SourceName);
            }

            // Generate a response
            var response = new Response(request);
            response.Result = ResponseResult.Success;
            return response;
        }
        #endregion

        #region ListServers()
        /// <summary>
        /// Lists the available servers that can be monitored.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A list containing one server - local.
        /// </returns>
        /// <remarks>
        /// This message is not secured at all.
        /// </remarks>
        public DataListResponse ListServers(ServerRequest request)
        {
            return new DataListResponse
            {
                Data = new List<string>{
                    "local"
                }
            };
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

        #region EncryptMessage()
        private static string EncryptMessage(RijndaelManaged crypto, string message)
        {
            var encryptStream = new MemoryStream();
            var encrypt = new CryptoStream(encryptStream,
                crypto.CreateEncryptor(),
                CryptoStreamMode.Write);

            var dataToEncrypt = Encoding.UTF8.GetBytes(message);
            encrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
            encrypt.FlushFinalBlock();
            encrypt.Close();

            var data = Convert.ToBase64String(encryptStream.ToArray());
            return data;
        }
        #endregion

        #region DecryptMessage()
        private static string DecryptMessage(RijndaelManaged crypto, string message)
        {
            var inputStream = new MemoryStream(Convert.FromBase64String(message));
            string data;
            using (var decryptionStream = new CryptoStream(inputStream,
                crypto.CreateDecryptor(),
                CryptoStreamMode.Read))
            {
                using (var reader = new StreamReader(decryptionStream))
                {
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }
        #endregion

        #region ExtractAndInvokeMessage()
        private Response ExtractAndInvokeMessage(string message, 
            string action,
            object channelInformation)
        {
            // Load the message
            var messageXml = new XmlDocument();
            messageXml.LoadXml(message);

            // Find the action
            var cruiseType = typeof(ICruiseServerClient);
            var actionMethod = cruiseType.GetMethod(action,
                BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public);
            if (actionMethod == null)
            {
                throw new CruiseControlException(
                    string.Format(
                        CultureInfo.CurrentCulture, "Unable to locate action '{0}'",
                        action));
            }

            // Find the type of message
            var messageType = FindMessageType(messageXml.DocumentElement.Name);
            if (messageType == null)
            {
                throw new CruiseControlException(
                    string.Format(
                        CultureInfo.CurrentCulture, "Unable to translate message: '{0}' is unknown",
                        messageXml.DocumentElement.Name));
            }

            // Convert the message and invoke the action
            var request = ConvertXmlToObject(messageType, message);
            var requestMessage = request as CommunicationsMessage;
            if (requestMessage != null) requestMessage.ChannelInformation = channelInformation;
            var response = actionMethod.Invoke(this,
                new object[] {
                        request
                    }) as Response;
            return response;
        }
        #endregion
        #endregion

        #region Private classes
        #region SecureConnection
        /// <summary>
        /// Stores the details on a secure connection.
        /// </summary>
        private class SecureConnection
        {
            #region Public properties
            #region Key
            /// <summary>
            /// The key.
            /// </summary>
            public byte[] Key { get; set; }
            #endregion

            #region IV
            /// <summary>
            /// The IV.
            /// </summary>
            public byte[] IV { get; set; }
            #endregion

            #region Expiry
            /// <summary>
            /// The expiry time.
            /// </summary>
            public DateTime Expiry { get; set; }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}