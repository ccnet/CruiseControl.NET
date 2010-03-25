using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("plugins")]
	public class NetReflectorPluginConfiguration : IPluginConfiguration
	{
		private IPlugin[] farmPlugins = new IPlugin[0];
		private IPlugin[] serverPlugins = new IPlugin[0];
		private IPlugin[] projectPlugins = new IPlugin[0];
		private IBuildPlugin[] buildPlugins = new IBuildPlugin[0];
        private string templateLocation;
        private ISecurityPlugin[] securityPlugins = new ISecurityPlugin[0];
        private ISessionStore sessionStore = new CookieSessionStore();

        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>The template location.</value>
        [ReflectorProperty("customTemplates", Required=false)]
        public string TemplateLocation
        {
            get { return templateLocation; }
            set { templateLocation = value; }
        }

        /// <summary>
        /// Gets or sets the farm plugins.
        /// </summary>
        /// <value>The farm plugins.</value>
        [ReflectorProperty("farmPlugins", Required = true)]
		public IPlugin[] FarmPlugins
		{
			get { return farmPlugins; }
			set { farmPlugins = value; }
		}

        /// <summary>
        /// Gets or sets the server plugins.
        /// </summary>
        /// <value>The server plugins.</value>
        [ReflectorProperty("serverPlugins", Required = true)]
		public IPlugin[] ServerPlugins
		{
			get { return serverPlugins; }
			set { serverPlugins = value; }
		}

        /// <summary>
        /// Gets or sets the project plugins.
        /// </summary>
        /// <value>The project plugins.</value>
        [ReflectorProperty("projectPlugins", Required = true)]
		public IPlugin[] ProjectPlugins
		{
			get { return projectPlugins; }
			set { projectPlugins = value; }
		}

        /// <summary>
        /// Gets or sets the build plugins.
        /// </summary>
        /// <value>The build plugins.</value>
        [ReflectorProperty("buildPlugins", Required = true)]
		public IBuildPlugin[] BuildPlugins
		{
			get { return buildPlugins; }
			set { buildPlugins = value; }
		}

        /// <summary>
        /// Gets or sets the security plugins.
        /// </summary>
        /// <value>The security plugins.</value>
        [ReflectorProperty("securityPlugins", Required = false)]
        public ISecurityPlugin[] SecurityPlugins
        {
            get { return securityPlugins; }
            set { securityPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the session store.
        /// </summary>
        /// <value>The session store.</value>
        [ReflectorProperty("sessionStore", InstanceTypeKey="type", Required=false)]
        public ISessionStore SessionStore
        {
            get { return sessionStore; }
            set { sessionStore = value; }
        }
	}
}
