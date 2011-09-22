using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
    /// <summary>
    /// <para>
    /// The Web Dashboard comes pre-configured to use a default set of plugins. 
    /// The section is split up into 4 parts, representing the Farm, Server, Project and Build views available in the Dashboard. 
    /// </para>
    /// Each section can be configured with any number of plugins. 
    /// Most Build Plugins can be configured just to be used for certain Projects.
    /// <para>
    /// From version 1.5 onwards it is advised to use the administration plugin to enable / disable plugins.
    /// </para>
    /// <showChildren></showChildren>
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;plugins&gt;
    /// &lt;farmPlugins&gt;
    /// &lt;farmReportFarmPlugin categories="false" /&gt;
    /// &lt;cctrayDownloadPlugin /&gt;
    /// &lt;administrationPlugin password="" /&gt;
    /// &lt;/farmPlugins&gt;
    /// &lt;serverPlugins&gt;
    /// &lt;serverReportServerPlugin /&gt;
    /// &lt;/serverPlugins&gt;
    /// &lt;projectPlugins&gt;
    /// &lt;projectReportProjectPlugin /&gt;
    /// &lt;viewProjectStatusPlugin /&gt;
    /// &lt;latestBuildReportProjectPlugin /&gt;
    /// &lt;viewAllBuildsProjectPlugin /&gt;
    /// &lt;/projectPlugins&gt;
    /// &lt;buildPlugins&gt;
    /// &lt;buildReportBuildPlugin&gt;
    /// &lt;xslFileNames&gt;
    /// &lt;xslFile&gt;xsl\header.xsl&lt;/xslFile&gt;
    /// &lt;xslFile&gt;xsl\modifications.xsl&lt;/xslFile&gt;
    /// &lt;/xslFileNames&gt;
    /// &lt;/buildReportBuildPlugin&gt;
    /// &lt;buildLogBuildPlugin /&gt;
    /// &lt;/buildPlugins&gt;
    /// &lt;securityPlugins&gt;
    /// &lt;simpleSecurity /&gt;
    /// &lt;/securityPlugins&gt;
    /// &lt;/plugins&gt;
    /// </code>
    /// </example>
    /// <title>Plugins</title>
    /// <version>1.0</version>
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
        /// This provides some theming support for the Dashboard.
        /// The folder is relative to the dashboard folder. And must contain all files from the templates folder. 
        /// The files in this custom templates folder can be
        /// easily modified, they will not be overwritten when you install a new version of CCNet
        /// </summary>
        /// <default>_NA_</default>
        /// <version>1.4.3</version>
        [ReflectorProperty("customTemplates", Required = false)]
        public string TemplateLocation
        {
            get { return templateLocation; }
            set { templateLocation = value; }
        }

        /// <summary>
        /// Gets or sets the <link>farm plugins</link>.
        /// </summary>
        /// <value>The farm plugins.</value>
        /// <version>1.0</version>
        [ReflectorProperty("farmPlugins", Required = true)]
        public IPlugin[] FarmPlugins
        {
            get { return farmPlugins; }
            set { farmPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the <link>server plugins</link>.
        /// </summary>
        /// <value>The server plugins.</value>
        /// <version>1.0</version>
        [ReflectorProperty("serverPlugins", Required = true)]
        public IPlugin[] ServerPlugins
        {
            get { return serverPlugins; }
            set { serverPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the <link>project plugins</link>.
        /// </summary>
        /// <value>The project plugins.</value>
        /// <version>1.0</version>
        [ReflectorProperty("projectPlugins", Required = true)]
        public IPlugin[] ProjectPlugins
        {
            get { return projectPlugins; }
            set { projectPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the <link>build plugins</link>.
        /// </summary>
        /// <value>The build plugins.</value>
        /// <version>1.0</version>
        [ReflectorProperty("buildPlugins", Required = true)]
        public IBuildPlugin[] BuildPlugins
        {
            get { return buildPlugins; }
            set { buildPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the <link>security plugins</link>.
        /// </summary>
        /// <value>The security plugins.</value>
        /// <version>1.5</version>
        [ReflectorProperty("securityPlugins", Required = false)]
        public ISecurityPlugin[] SecurityPlugins
        {
            get { return securityPlugins; }
            set { securityPlugins = value; }
        }

        /// <summary>
        /// Gets or sets the <link>session store</link>.
        /// </summary>
        /// <value>The session store.</value>
        /// <version>1.6</version>
        [ReflectorProperty("sessionStore", InstanceTypeKey = "type", Required = false)]
        public ISessionStore SessionStore
        {
            get { return sessionStore; }
            set { sessionStore = value; }
        }
    }
}
