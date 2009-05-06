using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing package details.
    /// </summary>
    [XmlRoot("listPackagesResponse")]
    [Serializable]
    public class ListPackagesResponse
        : Response
    {
        #region Private fields
        private List<PackageDetails> packages = new List<PackageDetails>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="ListPackagesResponse"/>.
        /// </summary>
        public ListPackagesResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ListPackagesResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public ListPackagesResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ListPackagesResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public ListPackagesResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Packages
        /// <summary>
        /// The packages.
        /// </summary>
        [XmlElement("packages")]
        public List<PackageDetails> Packages
        {
            get { return packages; }
            set { packages = value; }
        }
        #endregion
        #endregion
    }
}
