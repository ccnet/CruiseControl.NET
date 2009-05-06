using System;
using System.Xml.Serialization;
using System.Collections.Generic;

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
		private List<QueueSnapshot> queueSnapshots;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueueSetSnapshot"/> class.
		/// </summary>
		public QueueSetSnapshot()
		{
            queueSnapshots = new List<QueueSnapshot>();
		}

        /// <summary>
        /// The queues on the server and their current status.
        /// </summary>
        [XmlElement("queue")]
        public List<QueueSnapshot> Queues
		{
			get { return queueSnapshots; }
		}

        /// <summary>
        /// Finds a queue by its name.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public QueueSnapshot FindByName(string queueName)
        {
            foreach (QueueSnapshot queueSnapshot in queueSnapshots)
            {
                if (queueSnapshot.QueueName == queueName)
                {
                    return queueSnapshot;
                }
            }
            return null;
        }
	}
}