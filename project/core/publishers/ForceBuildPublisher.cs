using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// <para>
    /// The ForceBuildPublisher forces a build on a local or remote build server. It uses .NET Remoting to invoke a
    /// forced build on the CruiseControl.NET server at the specified URI.
    /// </para>
    /// <para>
    /// The forced build runs asynchronously, i.e. the ForceBuildPublisher does not wait for the forced build to
    /// finish. The ForceBuildPublisher is a great way to help <link> Splitting the build </link>.
    /// </para>
    /// <para>
    /// An alternative to the ForceBuildPublisher is the <link>Project Trigger</link>. The main difference is that the 
    /// ForceBuildPublisher is placed in the configuration for the primary project, while the ProjectTrigger is is
    /// placed in the configuration for the dependent project.
    /// </para>
    /// </summary>
    /// <title>Force Builder Publisher</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Simple Example">
    /// &lt;forcebuild&gt;
    /// &lt;project&gt;AcceptanceTestProject&lt;/project&gt;
    /// &lt;serverUri&gt;tcp://buildserver2:21234/CruiseManager.rem&lt;/serverUri&gt;
    /// &lt;integrationStatus&gt;Success&lt;/integrationStatus&gt;
    /// &lt;enforcerName&gt;Forcer&lt;/enforcerName&gt;
    /// &lt;/forcebuild&gt;
    /// </code>
    /// <code title="Example with Security">
    /// &lt;forcebuild&gt;
    /// &lt;project&gt;AcceptanceTestProject&lt;/project&gt;
    /// &lt;serverUri&gt;tcp://buildserver2:21234/CruiseManager.rem&lt;/serverUri&gt;
    /// &lt;integrationStatus&gt;Success&lt;/integrationStatus&gt;
    /// &lt;security&gt;
    /// &lt;namedValue name="username" value="autobuild" /&gt;
    /// &lt;namedValue name="password" value="autobuild" /&gt;
    /// &lt;/security&gt;
    /// &lt;/forcebuild&gt;
    /// </code>
    /// </example>
    [ReflectorType("forcebuild")]
	public class ForceBuildPublisher 
        : TaskBase
	{
        private readonly ICruiseServerClientFactory factory;
        private string BuildForcerName="BuildForcer";

        /// <summary>
        /// Initializes a new instance of the <see cref="ForceBuildPublisher" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public ForceBuildPublisher()
            : this(new CruiseServerClientFactory())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="ForceBuildPublisher" /> class.	
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <remarks></remarks>
        public ForceBuildPublisher(ICruiseServerClientFactory factory)
		{
			this.factory = factory;
            this.ServerUri = string.Format(System.Globalization.CultureInfo.CurrentCulture,"tcp://localhost:21234/{0}", RemoteCruiseServer.ManagerUri);
            this.IntegrationStatus = IntegrationStatus.Success;
		}
        /// <summary>
        /// The CCNet project to force build.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string Project { get; set; }

        /// <summary>
        /// Identification of a ForceBuildPublisher. This value is passed to the CCNetRequestSource attribute of the
        /// forced  project's build.
        /// </summary>
        /// <version>1.0</version>
        /// <default>BuildForcer</default>
        [ReflectorProperty("enforcerName", Required = false)]
        public string EnforcerName
        {
            get { return BuildForcerName; }
            set { BuildForcerName = value; }
        }

        /// <summary>
        /// The URI for the local or remote server managing the project to build. The default value is the default URI
        /// for the local build server.
        /// </summary>
        /// <version>1.0</version>
        /// <default>tcp://localhost:21234/CruiseManager.rem</default>
        /// <remarks>
        /// This publisher only uses .NET Remoting for connecting to the remote server. As such, it cannot use the 
        /// HTTP protocol for connecting.
        /// </remarks>
        [ReflectorProperty("serverUri", Required = false)]
        public string ServerUri { get; set; }

        /// <summary>
        /// The condition determining whether or not the remoting call should be made. The default value is "Success"
        /// indicating that the specified build will be forced if the current build was successful
        /// </summary>
        /// <version>1.0</version>
        /// <default>Success</default>
        [ReflectorProperty("integrationStatus", Required = false)]
        public IntegrationStatus IntegrationStatus { get; set; }

        /// <summary>
        /// The security credentials to pass through to the remote server.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// These are only needed if the remote project has security applied. If credentials are passed to the remote
        /// server, then the enforcerName will be ignored.
        /// Valid security tokens are: "username" and "password" (this list may be expanded in future).
        /// </remarks>
        [ReflectorProperty("security", Required = false)]
        public NameValuePair[] SecurityCredentials { get; set; }

        /// <summary>
        /// The parameters to pass to the remote project.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("parameters", Required = false)]
        public NameValuePair[] Parameters { get; set; }

        /// <summary>
        /// The logger to use.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Executes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool Execute(IIntegrationResult result)
		{
			if (IntegrationStatus != result.Status) return false;

            var logger = Logger ?? new DefaultLogger();
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Running for build publisher");

            var loggedIn = false;
            logger.Debug("Generating client for url '{0}'", ServerUri);
            var client = factory.GenerateClient(ServerUri);
            if ((SecurityCredentials != null) && (SecurityCredentials.Length > 0))
            {
                logger.Debug("Logging in");
                if (client.Login(new List<NameValuePair>(SecurityCredentials)))
                {
                    loggedIn = true;
                    logger.Debug("Logged on server, session token is " + client.SessionToken);
                }
                else
                {
                    logger.Warning("Unable to login to remote server");
                }
            }

            logger.Info("Sending ForceBuild request to '{0}' on '{1}'", Project, ServerUri);
            client.ForceBuild(Project, new List<NameValuePair>(Parameters ?? new NameValuePair[0]));
            if (loggedIn)
            {
                logger.Debug("Logging out");
                client.Logout();
            }

            return true;
		}
	}
}