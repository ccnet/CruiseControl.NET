using System;
using System.Web;
using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DashboardComponentFactory
	{
		private readonly Control control;
		private readonly HttpContext context;
		private readonly HttpRequest request;

		public DashboardComponentFactory(HttpRequest request, HttpContext context, Control control)
		{
			this.request = request;
			this.context = context;
			this.control = control;
		}

		public DefaultBuildRetrieverForRequest DefaultBuildRetrieverForRequest
		{
			get { return new DefaultBuildRetrieverForRequest(CachingBuildRetriever, CruiseManagerBuildNameRetriever); }
		}

		public CruiseManagerBuildRetriever CruiseManagerBuildRetriever
		{
			get { return new CruiseManagerBuildRetriever(ServerAggregatingCruiseManagerWrapper); }
		}

		public CachingBuildRetriever CachingBuildRetriever
		{
			get { return new CachingBuildRetriever(LocalFileCacheManager, CruiseManagerBuildRetriever); }
		}

		public CruiseManagerBuildNameRetriever CruiseManagerBuildNameRetriever
		{
			get { return new CruiseManagerBuildNameRetriever(ServerAggregatingCruiseManagerWrapper); }
		}

		public LocalFileCacheManager LocalFileCacheManager
		{
			get { return new LocalFileCacheManager(HttpPathMapper, ConfigurationSettingsConfigGetter); }
		}

		public ConfigurationSettingsConfigGetter ConfigurationSettingsConfigGetter
		{
			get { return new ConfigurationSettingsConfigGetter(); }
		}

		public HttpPathMapper HttpPathMapper
		{
			get { return new HttpPathMapper(context, control); }
		}

		public DefaultUrlBuilder DefaultUrlBuilder
		{
			get { return new DefaultUrlBuilder(HttpPathMapper); }
		}

		public DefaultHtmlBuilder DefaultHtmlBuilder
		{
			get { return new DefaultHtmlBuilder(); }
		}

		public DefaultBuildNameFormatter DefaultBuildNameFormatter
		{
			get { return new DefaultBuildNameFormatter(); }
		}

		public ServerAggregatingCruiseManagerWrapper ServerAggregatingCruiseManagerWrapper
		{
			get { return new ServerAggregatingCruiseManagerWrapper(ConfigurationSettingsConfigGetter, RemoteCruiseManagerFactory); }
		}

		public QueryStringRequestWrapper QueryStringRequestWrapper
		{
			get { return new QueryStringRequestWrapper(request.QueryString); }
		}

		public NameValueCruiseRequestFactory NameValueCruiseRequestFactory
		{
			get { return new NameValueCruiseRequestFactory(); }
		}

		public RemoteCruiseManagerFactory RemoteCruiseManagerFactory
		{
			get { return new RemoteCruiseManagerFactory(); }
		}

		public NetReflectorProjectSerializer NetReflectorProjectSerializer
		{
			get { return new NetReflectorProjectSerializer(); }
		}
	}
}
