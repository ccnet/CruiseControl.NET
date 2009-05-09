using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics
{
    [ReflectorType("projectStatisticsPlugin")]
    public class ProjectStatisticsPlugin : ICruiseAction, IPlugin
    {
        public static readonly string ACTION_NAME = "ViewStatisticsReport";
        private readonly IFarmService farmService;
        private readonly IPhysicalApplicationPathProvider pathProvider;
        private readonly ITransformer transformer;
        private string xslFileName;

        public ProjectStatisticsPlugin(IFarmService farmService, IPhysicalApplicationPathProvider pathProvider)
        {
            this.farmService = farmService;
            this.pathProvider = pathProvider;
            transformer = new XslTransformer();
        }

        [ReflectorProperty("xslFileName")]
        public string XslFileName
        {
            get { return xslFileName; }
            set { xslFileName = value; }
        }

        #region ICruiseAction Members

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            if (xslFileName == null)
            {
                throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
            }
            Hashtable xsltArgs = new Hashtable();
            xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;

            string xslFile = pathProvider.GetFullPathFor(XslFileName);
            string statisticsDocument = farmService.GetStatisticsDocument(cruiseRequest.ProjectSpecifier, cruiseRequest.RetrieveSessionToken());
            Log.Debug(statisticsDocument);
            string htmlFragment;
            try
            {
                htmlFragment = transformer.Transform(statisticsDocument, xslFile, xsltArgs);
            }
            catch (CruiseControlException)
            {
                htmlFragment = "Missing/Invalid statistics reports. Please check if you have enabled the Statistics Publisher, and statistics have been collected atleast once after that.";
            }
            return new HtmlFragmentResponse(htmlFragment);
        }

        #endregion

        #region IPlugin Members

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] {new ImmutableNamedAction(ACTION_NAME, this)}; }
        }

        public string LinkDescription
        {
            get { return "View Statistics"; }
        }

        #endregion
    }
}