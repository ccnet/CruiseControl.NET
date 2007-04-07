using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Represents a snapshot of the integration queue's current state at a point in time.
	/// For serializing to CCTray and the web dashboard.
	/// </summary>
	[Serializable]
	public class IntegrationQueueSnapshot
	{
		private SerializableDateTime timeStamp;
		private NamedQueueSnapshotList namedQueueSnapshots;

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationQueueSnapshot"/> class.
		/// </summary>
		public IntegrationQueueSnapshot()
			: this(DateTime.Now)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationQueueSnapshot"/> class.
		/// </summary>
		public IntegrationQueueSnapshot(DateTime timeStamp)
		{
			this.timeStamp = new SerializableDateTime(timeStamp);
			namedQueueSnapshots = new NamedQueueSnapshotList();
		}

		public DateTime TimeStamp
		{
			get { return timeStamp.DateTime; }
		}

		public NamedQueueSnapshotList Queues
		{
			get { return namedQueueSnapshots; }
		}
	}
}