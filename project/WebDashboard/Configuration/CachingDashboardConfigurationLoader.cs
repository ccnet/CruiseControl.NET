using System;
using System.Web;
using System.Web.Caching;
using Objection;
using Objection.NetReflectorPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	internal class CachingDashboardConfigurationLoader : IDashboardConfiguration
	{
		private const string DashboardConfigurationKey = "DashboardConfiguration";
		private IDashboardConfiguration dashboardConfiguration;

		public CachingDashboardConfigurationLoader(ObjectSource objectSource, HttpContext context)
		{
			dashboardConfiguration = context.Cache[DashboardConfigurationKey] as IDashboardConfiguration;
			if (dashboardConfiguration == null)
			{
				dashboardConfiguration = new DashboardConfigurationLoader(new ObjectionNetReflectorInstantiator(objectSource));
				context.Cache.Add(DashboardConfigurationKey, dashboardConfiguration, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.Normal, null);
			}
		}

		public IRemoteServicesConfiguration RemoteServices
		{
			get { return dashboardConfiguration.RemoteServices; }
		}

		public IPluginConfiguration PluginConfiguration
		{
			get { return dashboardConfiguration.PluginConfiguration; }
		}

        /// <summary>
        /// Clears the cached configuration.
        /// </summary>
        public static void ClearCache()
        {
            IDashboardConfiguration config = HttpContext.Current.Cache[DashboardConfigurationKey] as IDashboardConfiguration;
            if (config != null)
            {
                HttpContext.Current.Cache.Remove(DashboardConfigurationKey);
            }
        }
	}
}