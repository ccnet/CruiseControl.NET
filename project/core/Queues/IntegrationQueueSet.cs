
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Data structure representing the set of named integration queues.
	/// </summary>
	public class IntegrationQueueSet
	{
		private readonly SortedList queueSet = new SortedList();

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

		public void Add(string queueName, IQueueConfiguration config)
		{
			lock (this)
			{
				if (!queueSet.ContainsKey(queueName))
				{
					queueSet.Add(queueName, new IntegrationQueue(queueName, config, this));
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
				if (queue != null)
				{
					queueSetSnapshot.Queues.Add(BuildQueueSnapshot(queue));
				}
			}
			return queueSetSnapshot;
		}

		private static QueueSnapshot BuildQueueSnapshot(IIntegrationQueue queue)
		{
			QueueSnapshot queueSnapshot = new QueueSnapshot(queue.Name);

            for (int index = 0; index < queue.Count; index++)
            {
                IIntegrationQueueItem integrationQueueItem = (IIntegrationQueueItem)queue[index];
                // The first request in the queue shows it's real activity of CheckingModifications or Building
                // Everything else is in a pending state.
                ProjectActivity projectActivity = ProjectActivity.Pending;
                if (index == 0)
                {
                    projectActivity = integrationQueueItem.Project.CurrentActivity;
                }
				QueuedRequestSnapshot queuedRequestSnapshot = new QueuedRequestSnapshot(
					integrationQueueItem.Project.Name,
                    projectActivity,
                    integrationQueueItem.IntegrationRequest.RequestTime);
				queueSnapshot.Requests.Add(queuedRequestSnapshot);
			}
			return queueSnapshot;
		}
	}
}
