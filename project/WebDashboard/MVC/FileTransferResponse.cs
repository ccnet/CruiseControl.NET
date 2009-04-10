using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class FileTransferResponse : IResponse
    {
        private IFileTransfer fileTransfer;
        private ConditionalGetFingerprint serverFingerprint;
        private string fileName;
        private string type;

        public FileTransferResponse(IFileTransfer fileTransfer, string fileName)
        {
            this.fileTransfer = fileTransfer;
        }

        public FileTransferResponse(IFileTransfer fileTransfer, string fileName, string type)
        {
            this.fileTransfer = fileTransfer;
            this.fileName = fileName;
            this.type = type;
        }

        public void Process(HttpResponse response)
        {
            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
            if (!string.IsNullOrEmpty(fileName))
            {
                response.AppendHeader("fileTransfer-Disposition",
                    string.Format("filename=\"{0}\"", fileName));
            }
            if (!string.IsNullOrEmpty(type)) response.ContentType = type;
            fileTransfer.Download(response.OutputStream);
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }
    }
}