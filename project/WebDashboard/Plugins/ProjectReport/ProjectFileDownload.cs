using System.IO;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <summary>
    /// Downloads a file from the server to the client.
    /// </summary>
    public class ProjectFileDownload
        : ICruiseAction
    {
        public const string ActionName = "RetrieveFile";
        private readonly IFarmService farmService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFileDownload"/> class.
        /// </summary>
        /// <param name="farmService">The farm service.</param>
        public ProjectFileDownload(IFarmService farmService)
        {
            this.farmService = farmService;
        }

        /// <summary>
        /// Executes the specified cruise request.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string fileName = cruiseRequest.Request.GetText("file");
            string label = cruiseRequest.Request.GetText("label");
            string path = fileName;
            if (!string.IsNullOrEmpty(label)) path = Path.Combine(label, fileName);
            RemotingFileTransfer fileTransfer = farmService.RetrieveFileTransfer(cruiseRequest.ProjectSpecifier, path, cruiseRequest.RetrieveSessionToken());
            FileTransferResponse response = new FileTransferResponse(fileTransfer, fileName);
            return response;
        }
    }
}
