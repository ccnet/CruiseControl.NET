using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a status snapshot.
    /// </summary>
    [XmlRoot("statusSnapshotResponse")]
    [Serializable]
    public class StatusSnapshotResponse
        : Response
    {
        #region Private fields
        private ProjectStatusSnapshot snapshot;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="StatusSnapshotResponse"/>.
        /// </summary>
        public StatusSnapshotResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="StatusSnapshotResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public StatusSnapshotResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="StatusSnapshotResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public StatusSnapshotResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Snapshot
        /// <summary>
        /// The snapshot.
        /// </summary>
        [XmlElement("snapshot")]
        public ProjectStatusSnapshot Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }
        #endregion
        #endregion
    }
}
