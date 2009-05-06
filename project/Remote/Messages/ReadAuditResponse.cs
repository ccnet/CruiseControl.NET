using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a list of audit records.
    /// </summary>
    [XmlRoot("readAuditResponse")]
    [Serializable]
    public class ReadAuditResponse
        : Response
    {
        #region Private fields
        private List<AuditRecord> records = new List<AuditRecord>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="ReadAuditResponse"/>.
        /// </summary>
        public ReadAuditResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ReadAuditResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public ReadAuditResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ReadAuditResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public ReadAuditResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Records
        /// <summary>
        /// The audit records.
        /// </summary>
        [XmlElement("record")]
        public List<AuditRecord> Records
        {
            get { return records; }
            set { records = value; }
        }
        #endregion
        #endregion
    }
}
