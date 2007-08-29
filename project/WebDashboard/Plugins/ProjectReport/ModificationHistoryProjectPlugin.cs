using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    [ReflectorType("modificationHistoryProjectPlugin")]
    class ModificationHistoryProjectPlugin : ICruiseAction, IPlugin
    {
        public const string ActionName = "ViewProjectModificationHistory";
        private const string XslFileName = "xsl\\ModificationHistory.xsl";
        private readonly IPhysicalApplicationPathProvider pathProvider;

        private readonly IFarmService farmService;
        private ITransformer transformer;

        public ModificationHistoryProjectPlugin(IFarmService farmService, IPhysicalApplicationPathProvider pathProvider)
		{
            this.farmService = farmService;
            transformer = new XslTransformer();
            this.pathProvider = pathProvider;
		}

        public IResponse Execute(ICruiseRequest cruiseRequest)
		{			
            Hashtable xsltArgs = new Hashtable();
            xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;

            string HistoryDocument = farmService.GetModificationHistoryDocument(cruiseRequest.ProjectSpecifier);
            if (HistoryDocument.Length == 0)
            {
                return new HtmlFragmentResponse("No history Data found, use the modificationHistory Publisher for this project");
            }
            else
            {
                string xslFile = pathProvider.GetFullPathFor(XslFileName);
                return new HtmlFragmentResponse(transformer.Transform(HistoryDocument, xslFile, xsltArgs));
            }

		}

        public string LinkDescription
		{
			get { return "View Modification History"; }
		}

		public INamedAction[] NamedActions
		{
			get { return new INamedAction[] {new ImmutableNamedAction(ActionName, this)}; }
		}
	
    }
}
