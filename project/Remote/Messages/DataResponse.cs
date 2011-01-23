using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing generic data.
    /// </summary>
    [XmlRoot("dataResponse")]
    [Serializable]
    public class DataResponse
        : Response
    {
        #region Private fields
        private string data;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="DataResponse"/>.
        /// </summary>
        public DataResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="DataResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public DataResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="DataResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public DataResponse(Response response)
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
        public string Data
        {
            get { return data; }
            set { data = value; }
        }
        #endregion
        #endregion
    }
}
