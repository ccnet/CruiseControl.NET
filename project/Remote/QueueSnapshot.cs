using System;
using System.Xml.Serialization;
using System.Collections.Generic;

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
        private List<QueuedRequestSnapshot> _requests;

        public QueueSnapshot()
        {
        }

		public QueueSnapshot(string queueName)
		{
			this.queueName = queueName;
            _requests = new List<QueuedRequestSnapshot>();
		}

        [XmlAttribute("name")]
		public string QueueName
		{
			get { return queueName; }
            set { queueName = value; }
		}

        [XmlElement("queueRequest")]
        public List<QueuedRequestSnapshot> Requests
		{
			get { return _requests; }
		}

        public bool IsEmpty
        {
            get { return _requests.Count == 0; }
        }
	}
}