using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of named integration queues as stored in the snapshot.
	/// </summary>
	[Serializable]
	public class NamedQueueSnapshotList : IEnumerable
	{
		private ArrayList queueSnapshots;

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedQueueSnapshotList"/> class.
		/// </summary>
		public NamedQueueSnapshotList()
		{
			queueSnapshots = new ArrayList();
		}

		public int Count
		{
			get { return queueSnapshots.Count; }
		}

		public void Add(NamedQueueSnapshot namedQueueSnapshot)
		{ 
			queueSnapshots.Add(namedQueueSnapshot);
		}

		public NamedQueueSnapshot this[int index]
		{ 
			get { return queueSnapshots[index] as NamedQueueSnapshot; }
		}

		public NamedQueueSnapshot this[string queueName]
		{ 
			get 
			{
				foreach (NamedQueueSnapshot queueSnapshot in queueSnapshots)
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
