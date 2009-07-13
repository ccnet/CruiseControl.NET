using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// Defines an encrypted request.
    /// </summary>
    [XmlRoot("encryptedRequest")]
    [Serializable]
    public class EncryptedRequest
        : ServerRequest
    {
        #region Private fields
        private string encryptedData;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="EncryptedRequest"/>.
        /// </summary>
        public EncryptedRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="EncryptedRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public EncryptedRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="EncryptedRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="encryptedData">The name of the project.</param>
        public EncryptedRequest(string sessionToken, string encryptedData)
            : base(sessionToken)
        {
            this.encryptedData = encryptedData;
        }
        #endregion

        #region Public properties
        #region EncryptedData
        /// <summary>
        /// The encrypted data.
        /// </summary>
        [XmlElement("data")]
        public string EncryptedData
        {
            get { return encryptedData; }
            set { encryptedData = value; }
        }
        #endregion

        #region Action
        /// <summary>
        /// The action to perform.
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }
        #endregion
        #endregion
    }
}
