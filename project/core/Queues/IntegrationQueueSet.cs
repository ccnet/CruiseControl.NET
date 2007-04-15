using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Data structure representing the set of named integration queues.
	/// </summary>
	public class IntegrationQueueSet
	{
		private SortedList queueSet = new SortedList();

		public IIntegrationQueue this[string queueName]
		{
			get
			{
				lock (this)
				{
					return (IIntegrationQueue) queueSet[queueName];
				}
			}
		}

		public void Add(string queueName)
		{
			lock (this)
			{
				if (!queueSet.ContainsKey(queueName))
				{
					queueSet.Add(queueName, new IntegrationQueue(queueName));
				}
			}
		}

		public void Clear()
		{
			lock (this)
			{
				queueSet.Clear();
			}
		}

		public string[] GetQueueNames()
		{
			lock (this)
			{
				string[] queueNames = new string[queueSet.Keys.Count];
				queueSet.Keys.CopyTo(queueNames, 0);
				return queueNames;
			}
		}

		public QueueSetSnapshot GetIntegrationQueueSnapshot()
		{
			lock (this)
			{
				return BuildQueueSetSnapshot();
			}
		}

		private QueueSetSnapshot BuildQueueSetSnapshot()
		{
			QueueSetSnapshot queueSetSnapshot = new QueueSetSnapshot();
			foreach (IIntegrationQueue queue in queueSet.Values)
			{
				if (queue != null && queue.Count > 0)
				{
					queueSetSnapshot.Queues.Add(BuildQueueSnapshot(queue));
				}
			}
			return queueSetSnapshot;
		}

		private QueueSnapshot BuildQueueSnapshot(IIntegrationQueue queue)
		{
			QueueSnapshot queueSnapshot = new QueueSnapshot(queue.Name);

			foreach (IIntegrationQueueItem integrationQueueItem in queue)
			{
				QueuedRequestSnapshot queuedRequestSnapshot = new QueuedRequestSnapshot(
					integrationQueueItem.Project.Name);
				queueSnapshot.Requests.Add(queuedRequestSnapshot);
			}
			return queueSnapshot;
		}
	}
}