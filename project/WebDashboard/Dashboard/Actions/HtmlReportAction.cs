using System;
using System.IO;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Remote;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
    public class HtmlReportAction
        : ICruiseAction, IConditionalGetFingerprintProvider
    {
        #region Private fields
	    private readonly IFingerprintFactory fingerprintFactory;
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="HtmlReportAction"/>.
        /// </summary>
        /// <param name="fingerprintFactory"></param>
        /// <param name="farmService"></param>
        /// <param name="viewGenerator"></param>
        public HtmlReportAction(IFingerprintFactory fingerprintFactory, IFarmService farmService,
            IVelocityViewGenerator viewGenerator)
        {
            this.fingerprintFactory = fingerprintFactory;
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
        }
        #endregion

        #region Public properties
        #region HtmlFileName
        /// <summary>
        /// The name of the file to display.
        /// </summary>
        public string HtmlFileName { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Execute()
        /// <summary>
        /// Execute the action.
        /// </summary>
        /// <param name="cruiseRequest"></param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
		{
            var velocityContext = new Hashtable();
            velocityContext["url"] = string.Format("RetrieveBuildFile.aspx?file={0}", HtmlFileName);
            return viewGenerator.GenerateView("HtmlReport.vm", velocityContext);
        }
        #endregion

        #region GetFingerprint()
        /// <summary>
        /// Generate a fingerprint for this action.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
            return fingerprintFactory.BuildFromFileNames(HtmlFileName);
	    }
        #endregion
        #endregion
    }
}
