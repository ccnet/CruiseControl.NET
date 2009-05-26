using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <summary>
    /// Display Ohloh stats for a project.
    /// </summary>
    [ReflectorType("ohlohProjectPlugin")]
    public class OhlohProjectPlugin
        : ICruiseAction, IPlugin
    {
        #region Private fields
        private const string ActionName = "ViewOhlohProjectStats";
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        #endregion

        #region Constructors
        public OhlohProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
        }
        #endregion

        #region Public properties
        #region LinkDescription
        public string LinkDescription
        {
            get { return "View Ohloh Stats"; }
        }
        #endregion

        #region NamedActions
        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction(ActionName, this) }; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Execute()
        public IResponse Execute(ICruiseRequest request)
		{
            var ohloh = farmService.GetLinkedSiteId(request.ProjectSpecifier,
                request.RetrieveSessionToken(),
                "ohloh");

            if (string.IsNullOrEmpty(ohloh))
            {
                return new HtmlFragmentResponse("<div>This project has not been linked to a project in Ohloh</div>");
            }
            else
            {
                var velocityContext = new Hashtable();
                velocityContext["ohloh"] = ohloh;
                velocityContext["projectName"] = request.ProjectName;

                return viewGenerator.GenerateView(@"OhlohStats.vm", velocityContext);
            }
		}
        #endregion
        #endregion
    }
}
