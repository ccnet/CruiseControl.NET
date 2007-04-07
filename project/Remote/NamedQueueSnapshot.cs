using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// A snapshot of a named integration queue and it's contents.
	/// </summary>
	[Serializable]
	public class NamedQueueSnapshot
	{
		private string queueName;
		private QueuedItemSnapshotList items;

		public NamedQueueSnapshot(string queueName)
		{
			this.queueName = queueName;
			items = new QueuedItemSnapshotList();
		}

		public string QueueName
		{
			get { return queueName; }
		}

		public QueuedItemSnapshotList Items
		{
			get { return items; }
		}
	}
}