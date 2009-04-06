using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of named integration queues as stored in the snapshot.
	/// </summary>
	[Serializable]
	public class QueueSnapshotList : IEnumerable
	{
		private ArrayList queueSnapshots;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueueSnapshotList"/> class.
		/// </summary>
		public QueueSnapshotList()
		{
			queueSnapshots = new ArrayList();
		}

		public int Count
		{
			get { return queueSnapshots.Count; }
		}

		public void Add(QueueSnapshot queueSnapshot)
		{ 
			queueSnapshots.Add(queueSnapshot);
		}

		public QueueSnapshot this[int index]
		{ 
			get { return queueSnapshots[index] as QueueSnapshot; }
		}

		public QueueSnapshot this[string queueName]
		{ 
			get 
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return queueSnapshots.GetEnumerator();
		}
	}
}
