using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Represents a snapshot of the integration queue's current state at a point in time.
	/// For serializing to CCTray and the web dashboard.
	/// </summary>
	[Serializable]
	public class QueueSetSnapshot
	{
		private QueueSnapshotList queueSnapshots;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueueSetSnapshot"/> class.
		/// </summary>
		public QueueSetSnapshot()
		{
			queueSnapshots = new QueueSnapshotList();
		}

		public QueueSnapshotList Queues
		{
			get { return queueSnapshots; }
		}
	}
}