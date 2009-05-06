using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a list of generic data.
    /// </summary>
    [XmlRoot("dataListResponse")]
    [Serializable]
    public class DataListResponse
        : Response
    {
        #region Private fields
        private List<string> data = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="DataListResponse"/>.
        /// </summary>
        public DataListResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="DataListResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public DataListResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="DataListResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public DataListResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Data
        /// <summary>
        /// The data package.
        /// </summary>
        [XmlElement("data")]
        public List<string> Data
        {
            get { return data; }
            set { data = value; }
        }
        #endregion
        #endregion
    }
}
