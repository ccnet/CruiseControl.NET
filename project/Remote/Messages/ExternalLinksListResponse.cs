using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a list of external links.
    /// </summary>
    [XmlRoot("externalLinksResponse")]
    [Serializable]
    public class ExternalLinksListResponse
        : Response
    {
        #region Private fields
        private List<ExternalLink> data = new List<ExternalLink>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="ExternalLinksListResponse"/>.
        /// </summary>
        public ExternalLinksListResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ExternalLinksListResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public ExternalLinksListResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ExternalLinksListResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public ExternalLinksListResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region ExternalLinks
        /// <summary>
        /// The external links.
        /// </summary>
        [XmlElement("link")]
        public List<ExternalLink> ExternalLinks
        {
            get { return data; }
            set { data = value; }
        }
        #endregion
        #endregion
    }
}
