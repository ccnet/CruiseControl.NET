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
        private static Regex linkFinder = new Regex("(src|href)=\"[^\"]*\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
            var htmlData = LoadHtmlFile(cruiseRequest);
            var prefixPos = HtmlFileName.LastIndexOf("\\");
            var prefix = prefixPos >= 0 ? HtmlFileName.Substring(0, prefixPos + 1) : string.Empty;
            MatchEvaluator evaluator = (match) =>
            {
                var splitPos = match.Value.IndexOf("=\"");
                var newValue = match.Value.Substring(0, splitPos + 2) +
                    "RetrieveBuildFile.aspx?file=" +
                    prefix +
                    match.Value.Substring(splitPos + 2);
                return newValue;
            };
            htmlData = linkFinder.Replace(htmlData, evaluator);
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

        #region Private methods
        #region LoadHtmlFile()
        /// <summary>
        /// Loads the HTML file.
        /// </summary>
        /// <returns></returns>
        private string LoadHtmlFile(ICruiseRequest cruiseRequest)
        {
            if (string.IsNullOrEmpty(HtmlFileName))
            {
                throw new ApplicationException("HTML File Name has not been set for HTML Report Action");
            }

            // Retrieve the file transfer object
            var fileTransfer = farmService.RetrieveFileTransfer(cruiseRequest.BuildSpecifier, HtmlFileName, FileTransferSource.Artefact);
            if (fileTransfer != null)
            {
                // Transfer the file across and load it into a string
                var stream = new MemoryStream();
                fileTransfer.Download(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                string htmlData = reader.ReadToEnd();
                return htmlData;
            }
            else
            {
                return "<div>Unable to find file</div>";
            }
        }
        #endregion
        #endregion
    }
}
