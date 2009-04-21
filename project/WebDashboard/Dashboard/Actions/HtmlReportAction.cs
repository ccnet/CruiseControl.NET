using System;
using System.IO;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Remote;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
    public class HtmlReportAction
        : ICruiseAction, IConditionalGetFingerprintProvider
    {
        #region Private fields
	    private readonly IFingerprintFactory fingerprintFactory;
        private readonly IFarmService farmService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="HtmlReportAction"/>.
        /// </summary>
        /// <param name="fingerprintFactory"></param>
        public HtmlReportAction(IFingerprintFactory fingerprintFactory, IFarmService farmService)
        {
            this.fingerprintFactory = fingerprintFactory;
            this.farmService = farmService;
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
            var htmlData = string.Format("<iframe width=\"100%\" height=\"600\" frameborder=\"1\" src=\"RetrieveBuildFile.aspx?file={0}\"></iframe>", HtmlFileName);
			return new HtmlFragmentResponse(htmlData);
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
