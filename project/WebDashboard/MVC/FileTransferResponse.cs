using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Remote;
using System.IO;
using Microsoft.Win32;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class FileTransferResponse : IResponse
    {
        private IFileTransfer fileTransfer;
        private ConditionalGetFingerprint serverFingerprint;
        private string fileName;
        private string type;

        public FileTransferResponse(IFileTransfer fileTransfer, string fileName)
            : this(fileTransfer, fileName, null)
        {
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
            if (!string.IsNullOrEmpty(type))
            {
                response.ContentType = type;
            }
            else if (!string.IsNullOrEmpty(fileName))
            {
                var mimeType = GetMimeType(fileName);
                response.ContentType = type;
            }
            fileTransfer.Download(response.OutputStream);
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }

        #region GetMimeType()
        /// <summary>
        /// Retrieves the mime type for a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetMimeType(string filename)
        {
            var mimeType = "application/octetstream";
            var fileExtension = Path.GetExtension(filename).ToLower();
            var regkey = Registry.ClassesRoot.OpenSubKey(fileExtension);
            if ((regkey != null) && (regkey.GetValue("Content Type") != null))
            {
                mimeType = regkey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
        #endregion
    }
}