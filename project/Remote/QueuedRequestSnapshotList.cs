using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of the contents of a particular integration queue as stored in the snapshot.
	/// </summary>
	[Serializable]
	public class QueuedRequestSnapshotList : IEnumerable
	{
		private ArrayList queuedRequests;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueuedRequestSnapshotList"/> class.
		/// </summary>
		public QueuedRequestSnapshotList()
		{
			queuedRequests = new ArrayList();
		}

		public int Count
		{
			get { return queuedRequests.Count; }
		}

		public void Add(QueuedRequestSnapshot queuedRequestSnapshot)
		{ 
			queuedRequests.Add(queuedRequestSnapshot);
		}

		public QueuedRequestSnapshot this[int index]
		{ 
			get { return queuedRequests[index] as QueuedRequestSnapshot; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return queuedRequests.GetEnumerator();
		}
	}
}
