using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class InMemoryResponseCache : IResponseCache
	{
		private readonly DashboardCacheDependency cacheDependency;
		private static readonly Cache cache = HttpRuntime.Cache;

		public InMemoryResponseCache(DashboardCacheDependency dashboardCacheDependency)
		{
			cacheDependency = dashboardCacheDependency;
		}

		public IResponse Get(IRequest request)
		{
			return (IResponse) cache.Get(request.RawUrl);
		}

		public void Insert(IRequest request, IResponse response)
		{
			cache.Insert(request.RawUrl, response, new CacheDependency(cacheDependency.Filenames), AbsoluteExpirationTime(), LastAccessExpirationTime());
		}

		private static TimeSpan LastAccessExpirationTime()
		{
			return new TimeSpan(1, 0, 0);
		}

		private static DateTime AbsoluteExpirationTime()
		{
			return DateTime.MaxValue;
		}
	}

	public class DashboardCacheDependency
	{
		private readonly IPluginConfiguration config;
		private readonly IPhysicalApplicationPathProvider physicalApplicationPathProvider;

		public DashboardCacheDependency(IPluginConfiguration config, IPhysicalApplicationPathProvider physicalApplicationPathProvider)
		{
			this.config = config;
			this.physicalApplicationPathProvider = physicalApplicationPathProvider;
		}

		public string[] Filenames
		{
			get
			{
				BuildReportBuildPlugin plugin = (BuildReportBuildPlugin) config.BuildPlugins[0];
				string[] xslFilenames = new string[plugin.XslFileNames.Length];
				for (int i = 0; i < xslFilenames.Length; i++)
				{
					xslFilenames[i] = physicalApplicationPathProvider.GetFullPathFor(plugin.XslFileNames[i].Filename);
				}
				return xslFilenames;
			}
		}
	}
}