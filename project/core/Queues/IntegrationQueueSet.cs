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

		public IntegrationQueueSnapshot GetIntegrationQueueSnapshot()
		{
			lock (this)
			{
				return BuildQueueContentSnapshot();
			}
		}

		private IntegrationQueueSnapshot BuildQueueContentSnapshot()
		{
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			foreach (IIntegrationQueue queue in queueSet.Values)
			{
				if (queue != null && queue.Count > 0)
				{
					snapshot.Queues.Add(BuildQueueSnapshot(queue));
				}
			}
			return snapshot;
		}

		private NamedQueueSnapshot BuildQueueSnapshot(IIntegrationQueue queue)
		{
			NamedQueueSnapshot namedQueueSnapshot = new NamedQueueSnapshot(queue.Name);

			foreach (IIntegrationQueueItem integrationQueueItem in queue)
			{
				QueuedItemSnapshot queuedItemSnapshot = new QueuedItemSnapshot(
					queue.Name,
					integrationQueueItem.Project.Name,
					integrationQueueItem.Project.QueuePriority,
					integrationQueueItem.IntegrationRequest.BuildCondition,
					integrationQueueItem.IntegrationRequest.Source);
				namedQueueSnapshot.Items.Add(queuedItemSnapshot);
			}
			return namedQueueSnapshot;
		}
	}
}