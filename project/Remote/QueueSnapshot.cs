using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// A snapshot of a particular integration queue and it's contents.
	/// </summary>
	[Serializable]
    [XmlRoot("queueSnapshot")]
	public class QueueSnapshot
	{
		private string queueName;
        private List<QueuedRequestSnapshot> queueRequests = new List<QueuedRequestSnapshot>();
        // Required for 1.4.4 or earlier compatibility
        private QueuedRequestSnapshotList _requests;

        /// <summary>
        /// Initialise a new blank <see cref="QueueSnapshot"/>.
        /// </summary>
        public QueueSnapshot()
        {
        }

        /// <summary>
        /// Initialise a new populated <see cref="QueueSnapshot"/>.
        /// </summary>
        /// <param name="queueName"></param>
		public QueueSnapshot(string queueName)
		{
			this.queueName = queueName;
		}

        /// <summary>
        /// The name of the queue.
        /// </summary>
        [XmlAttribute("name")]
		public string QueueName
		{
			get { return queueName; }
            set { queueName = value; }
		}

        /// <summary>
        /// The current requests in the queue.
        /// </summary>
        [XmlElement("queueRequest")]
        public List<QueuedRequestSnapshot> Requests
		{
			get { return queueRequests; }
		}

        /// <summary>
        /// Whether there are any requests in the queue or not.
        /// </summary>
        [XmlIgnore]
        public bool IsEmpty
        {
            get { return queueRequests.Count == 0; }
        }

        #region Private methods
        #region DataReceived()
        /// <summary>
        /// Handle any old (pre-1.5.0) data.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void DataReceived(StreamingContext context)
        {
            if (_requests != null)
            {
                queueRequests = new List<QueuedRequestSnapshot>();
                foreach (var queue in _requests)
                {
                    queueRequests.Add(queue as QueuedRequestSnapshot);
                }
            }
        }
        #endregion
        #endregion
    }
}