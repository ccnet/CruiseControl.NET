using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Represents a snapshot of the integration queue's current state at a point in time.
	/// For serializing to CCTray and the web dashboard.
	/// </summary>
	[Serializable]
    [XmlRoot("queueSetSnapshot")]
	public class QueueSetSnapshot
	{
        private List<QueueSnapshot> snapshots = new List<QueueSnapshot>();
        // Required for 1.4.4 or earlier compatibility
        private QueueSnapshotList queueSnapshots = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueueSetSnapshot"/> class.
		/// </summary>
		public QueueSetSnapshot()
		{
		}

        /// <summary>
        /// The queues on the server and their current status.
        /// </summary>
        [XmlElement("queue")]
        public List<QueueSnapshot> Queues
		{
			get { return snapshots; }
		}

        /// <summary>
        /// Finds a queue by its name.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public QueueSnapshot FindByName(string queueName)
        {
            foreach (QueueSnapshot queueSnapshot in snapshots)
            {
                if (queueSnapshot.QueueName == queueName)
                {
                    return queueSnapshot;
                }
            }
            return null;
        }

        #region Private methods
        #region DataReceived()
        /// <summary>
        /// Handle any old (pre-1.5.0) data.
        /// </summary>
        /// <param name="context"></param>
//  COMMENTED BY CODEIT.RIGHT
//        [OnDeserialized]
//        private void DataReceived(StreamingContext context)
//        {
//            if (queueSnapshots != null)
//            {
//                snapshots = new List<QueueSnapshot>();
//                foreach (var queue in queueSnapshots)
//                {
//                    snapshots.Add(queue as QueueSnapshot);
//                }
//            }
//        }
        #endregion
        #endregion
    }
}