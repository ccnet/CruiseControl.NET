using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
    public class BuildFileDownload
        : ICruiseAction
    {
        #region Private fields
        public const string ActionName = "RetrieveBuildFile";
        private readonly IFarmService farmService;
        private static Regex linkFinder = new Regex("(src|href)=\"[^\"]*\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Constructors
        public BuildFileDownload(IFarmService farmService)
        {
            this.farmService = farmService;
        }
        #endregion

        #region Public methods
        #region Execute()
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string fileName = cruiseRequest.Request.GetText("file").Replace("/", "\\");
            if (fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase))
            {
                var htmlData = LoadHtmlFile(cruiseRequest, fileName);
                var prefixPos = fileName.LastIndexOf("\\");
                var prefix = prefixPos >= 0 ? fileName.Substring(0, prefixPos + 1) : string.Empty;
                MatchEvaluator evaluator = (match) =>
                {
                    var splitPos = match.Value.IndexOf("=\"");
                    var prefex = match.Value.Substring(0, splitPos + 2);
                    if (match.Value.StartsWith(prefex + "data:", StringComparison.OrdinalIgnoreCase) ||
                        match.Value.StartsWith(prefex + "#", StringComparison.OrdinalIgnoreCase) ||
                        match.Value.StartsWith(prefex + "http://", StringComparison.OrdinalIgnoreCase))
                    {
                        return match.Value;
                    }

                    var newValue = prefex +
                        "RetrieveBuildFile.aspx?file=" +
                        prefix +
                        match.Value.Substring(splitPos + 2);
                    return newValue;
                };
                htmlData = linkFinder.Replace(htmlData, evaluator);
                return new HtmlFragmentResponse(htmlData);
            }
            else
            {
                // Retrieve the file transfer object
                var fileTransfer = farmService.RetrieveFileTransfer(cruiseRequest.BuildSpecifier, fileName, cruiseRequest.RetrieveSessionToken());
                if (fileTransfer != null)
                {
                    return new FileTransferResponse(fileTransfer, fileName);
                }
                else
                {
                    return new HtmlFragmentResponse("<div>Unable to find file</div>");
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region LoadHtmlFile()
        /// <summary>
        /// Loads the HTML file.
        /// </summary>
        /// <returns></returns>
        private string LoadHtmlFile(ICruiseRequest cruiseRequest, string fileName)
        {
            try
            {
                // Retrieve the file transfer object
                var fileTransfer = farmService.RetrieveFileTransfer(cruiseRequest.BuildSpecifier, fileName, cruiseRequest.RetrieveSessionToken());
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
            catch (Exception error)
            {
                return "<div>An error occurred while retrieving the file: " + error.Message + "</div>";
            }
        }
        #endregion
        #endregion
    }
}
