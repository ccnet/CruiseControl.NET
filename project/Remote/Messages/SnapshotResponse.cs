using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing a server snapshot.
    /// </summary>
    [XmlRoot("snapshotResponse")]
    [Serializable]
    public class SnapshotResponse
        : Response
    {
        #region Private fields
        private CruiseServerSnapshot snapshot;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="SnapshotResponse"/>.
        /// </summary>
        public SnapshotResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="SnapshotResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public SnapshotResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="SnapshotResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public SnapshotResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Snapshot
        /// <summary>
        /// The snapshot package.
        /// </summary>
        [XmlElement("snapshot")]
        public CruiseServerSnapshot Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }
        #endregion
        #endregion
    }
}
