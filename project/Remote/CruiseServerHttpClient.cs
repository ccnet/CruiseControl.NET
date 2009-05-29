using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A client connection to an old (pre-1.5) version of CruiseControl.NET via HTTP.
    /// </summary>
    public class CruiseServerHttpClient
        : CruiseServerClientBase
    {
        #region Private fields
        private readonly string serverUri;
        private string targetServer;
        private WebClient client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new <see cref="CruiseServerRemotingClient"/>.
        /// </summary>
        /// <param name="serverUri">The address of the server.</param>
        public CruiseServerHttpClient(string serverUri)
            : this(serverUri, new WebClient())
        {
        }

        /// <summary>
        /// Initialises a new <see cref="CruiseServerRemotingClient"/>.
        /// </summary>
        /// <param name="serverUri">The address of the server.</param>
        /// <param name="client">The <see cref="WebClient"/> to use.</param>
        public CruiseServerHttpClient(string serverUri, WebClient client)
        {
            this.serverUri = serverUri;
            this.client = client;
        }
        #endregion

        #region Public properties
        #region TargetServer
        /// <summary>
        /// The server that will be targeted by all messages.
        /// </summary>
        public override string TargetServer
        {
            get
            {
                if (string.IsNullOrEmpty(targetServer))
                {
                    var targetUri = new Uri(serverUri);
                    return targetUri.Host;
                }
                else
                {
                    return targetServer;
                }
            }
            set { targetServer = value; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this client busy performing an operation.
        /// </summary>
        public override bool IsBusy
        {
            get { return false; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public override ProjectStatus[] GetProjectStatus()
        {
            var response = client.DownloadString(GenerateUrl("xmlstatusreport.aspx"));
            return null;
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        public override void ForceBuild(string projectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forces a build for the named project with some parameters.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="parameters"></param>
        public override void ForceBuild(string projectName, List<NameValuePair> parameters)
        {
            ForceBuild(projectName);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Attempts to abort a current project build.
        /// </summary>
        /// <param name="projectName">The name of the project to abort.</param>
        public override void AbortBuild(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Request()
        /// <summary>
        /// Sends a build request to the server.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="integrationRequest"></param>
        public override void Request(string projectName, IntegrationRequest integrationRequest)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="project"></param>
        public override void StartProject(string project)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stop project.
        /// </summary>
        /// <param name="project"></param>
        public override void StopProject(string project)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="message"></param>
        public override void SendMessage(string projectName, Message message)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateUrl()
        private string GenerateUrl(string pageUrl)
        {
            var uri = new Uri(serverUri);
            if (Uri.TryCreate(new Uri(serverUri), pageUrl, out uri))
            {
                return uri.AbsoluteUri;
            }
            else
            {
                return serverUri;
            }
        }
        #endregion
        #endregion
    }
}
