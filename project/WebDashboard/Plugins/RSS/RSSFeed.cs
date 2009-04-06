using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS
{
    public class RSSFeed : ICruiseAction
    {
        public const string ACTION_NAME = "RSSFeed";
        private readonly IFarmService farmService;

        public RSSFeed(IFarmService farmService)
		{
            this.farmService = farmService;
        }

        public IResponse Execute(ICruiseRequest request)
        {
            return new XmlFragmentResponse(farmService.GetRSSFeed( request.ProjectSpecifier) );
        }

    }
}
