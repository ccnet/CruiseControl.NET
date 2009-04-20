using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using System.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
    public class BuildFileDownload
        : ICruiseAction
    {
        public const string ActionName = "RetrieveBuildFile";
        private readonly IFarmService farmService;

        public BuildFileDownload(IFarmService farmService)
        {
            this.farmService = farmService;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string fileName = cruiseRequest.Request.GetText("file");
            string label = cruiseRequest.Request.GetText("label");
            string path = fileName;
            if (!string.IsNullOrEmpty(label)) path = Path.Combine(label, fileName);
            RemotingFileTransfer fileTransfer = farmService.RetrieveFileTransfer(cruiseRequest.BuildSpecifier, path, FileTransferSource.Artefact);
            if (fileTransfer != null)
            {
                FileTransferResponse response = new FileTransferResponse(fileTransfer, fileName);
                return response;
            }
            else
            {
                return new HtmlFragmentResponse("<div>Unable to find file</div>");
            }
        }
    }
}
