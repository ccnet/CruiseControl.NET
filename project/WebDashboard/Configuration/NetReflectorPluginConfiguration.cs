using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("plugins")]
	public class NetReflectorPluginConfiguration : IPluginConfiguration
	{
        private StylesheetConfiguration[] styleSheets = new StylesheetConfiguration[0];
		private IPlugin[] farmPlugins = new IPlugin[0];
		private IPlugin[] serverPlugins = new IPlugin[0];
		private IPlugin[] projectPlugins = new IPlugin[0];
		private IBuildPlugin[] buildPlugins = new IBuildPlugin[0];
        private string templateLocation;
        private ISecurityPlugin[] securityPlugins = new ISecurityPlugin[0];
        private ISessionStore sessionStore = new QuerySessionStore();

        [ReflectorArray("stylesheets", Required = false)]
        public StylesheetConfiguration[] Stylesheets
        {
            get { return styleSheets; }
            set { styleSheets = value; }
        }

        [ReflectorProperty("customTemplates", Required=false)]
        public string TemplateLocation
        {
            get { return templateLocation; }
            set { templateLocation = value; }
        }

		[ReflectorArray("farmPlugins", Required=true)]
		public IPlugin[] FarmPlugins
		{
			get
			{
				return farmPlugins;
			}
			set
			{
				farmPlugins = value;
			}
		}

		[ReflectorArray("serverPlugins", Required=true)]
		public IPlugin[] ServerPlugins
		{
			get
			{
				return serverPlugins;
			}
			set
			{
				serverPlugins = value;
			}
		}

		[ReflectorArray("projectPlugins", Required=true)]
		public IPlugin[] ProjectPlugins
		{
			get
			{
				return projectPlugins;
			}
			set
			{
				projectPlugins = value;
			}
		}

		[ReflectorArray("buildPlugins", Required=true)]
		public IBuildPlugin[] BuildPlugins
		{
			get
			{
				return buildPlugins;
			}
			set
			{
				buildPlugins = value;
			}
		}

        [ReflectorArray("securityPlugins", Required = false)]
        public ISecurityPlugin[] SecurityPlugins
        {
            get { return securityPlugins; }
            set { securityPlugins = value; }
        }

        [ReflectorProperty("sessionStore", InstanceTypeKey="type", Required=false)]
        public ISessionStore SessionStore
        {
            get { return sessionStore; }
            set { sessionStore = value; }
        }
	}
}
