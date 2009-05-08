using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a file transfer instance.
    /// </summary>
    [XmlRoot("fileTransferResponse")]
    [Serializable]
    public class FileTransferResponse
        : Response
    {
        #region Private fields
        private IFileTransfer fileTransfer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="FileTransferResponse"/>.
        /// </summary>
        public FileTransferResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileTransferResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public FileTransferResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="FileTransferResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public FileTransferResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region FileTransfer
        /// <summary>
        /// The file transfer instance.
        /// </summary>
        [XmlIgnore]
        public IFileTransfer FileTransfer
        {
            get { return fileTransfer; }
            set { fileTransfer = value; }
        }
        #endregion
        #endregion
    }
}
