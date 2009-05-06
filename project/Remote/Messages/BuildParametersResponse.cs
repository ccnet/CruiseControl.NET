using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing build parameters.
    /// </summary>
    [XmlRoot("buildParametersResponse")]
    [Serializable]
    public class BuildParametersResponse
        : Response
    {
        #region Private fields
        private List<ParameterBase> parameters = new List<ParameterBase>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="BuildParametersResponse"/>.
        /// </summary>
        public BuildParametersResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="BuildParametersResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public BuildParametersResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="BuildParametersResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public BuildParametersResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Parameters
        /// <summary>
        /// The parameters.
        /// </summary>
        [XmlElement("parameter")]
        public List<ParameterBase> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
        #endregion
        #endregion
    }
}
