using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of the contents of a particular integration queue as stored in the snapshot.
	/// </summary>
	[Serializable]
	public class QueuedItemSnapshotList : IEnumerable
	{
		private ArrayList queueSnapshotItems;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueuedItemSnapshotList"/> class.
		/// </summary>
		public QueuedItemSnapshotList()
		{
			queueSnapshotItems = new ArrayList();
		}

		public int Count
		{
			get { return queueSnapshotItems.Count; }
		}

		public void Add(QueuedItemSnapshot queuedItemSnapshot)
		{ 
			queueSnapshotItems.Add(queuedItemSnapshot);
		}

		public QueuedItemSnapshot this[int index]
		{ 
			get { return queueSnapshotItems[index] as QueuedItemSnapshot; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return queueSnapshotItems.GetEnumerator();
		}
	}
}
