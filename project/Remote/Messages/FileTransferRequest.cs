using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A message for requesting a file transfer instance from the server.
    /// </summary>
    [XmlRoot("fileTransferMessage")]
    [Serializable]
    public class FileTransferRequest
        : ProjectRequest
    {
        #region Private fields
        private string fileName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new blank <see cref="FileTransferRequest"/>.
        /// </summary>
        public FileTransferRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="FileTransferRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public FileTransferRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="FileTransferRequest"/> with all the properties.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The project to retrieve the file from.</param>
        /// <param name="fileName">The name of the file to retrieve.</param>
        public FileTransferRequest(string sessionToken, string projectName, string fileName)
            : base(sessionToken, projectName)
        {
            this.fileName = fileName;
        }
        #endregion

        #region Public properties
        #region FileName
        /// <summary>
        /// The name of the file to retrieve.
        /// </summary>
        [XmlElement("fileName")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        #endregion
        #endregion
    }
}
