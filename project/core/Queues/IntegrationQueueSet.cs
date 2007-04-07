using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Data structure representing the set of named integration queues.
	/// </summary>
	public class IntegrationQueueSet
	{
		private Hashtable queueSet = new Hashtable();
		private string[] cachedQueueNames;

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
					queueSet.Add(queueName, new IntegrationQueue());
					cachedQueueNames = null;
				}
			}
		}

		public void Clear()
		{
			queueSet.Clear();
			cachedQueueNames = null;
		}

		public string[] GetQueueNames()
		{
			lock (this)
			{
				if (cachedQueueNames == null)
				{
					cachedQueueNames = new string[queueSet.Keys.Count];
					queueSet.Keys.CopyTo(cachedQueueNames, 0);
					Array.Sort(cachedQueueNames);
				}
				return cachedQueueNames;
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
			Log.Debug("Building integration queue snapshot cache");
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			string[] orderedQueueNames = GetQueueNames();

			foreach (string queueName in orderedQueueNames)
			{
				IIntegrationQueue queue = this[queueName];
				if (queue != null && queue.Count > 0)
				{
					NamedQueueSnapshot namedQueueSnapshot = GetNamedQueueSnapshot(queue, queueName);
					snapshot.Queues.Add(namedQueueSnapshot);
				}
			}

			return snapshot;
		}

		private NamedQueueSnapshot GetNamedQueueSnapshot(IIntegrationQueue queue, string queueName)
		{
			NamedQueueSnapshot namedQueueSnapshot = new NamedQueueSnapshot(queueName);

			foreach (IIntegrationQueueItem integrationQueueItem in queue)
			{
				QueuedItemSnapshot queuedItemSnapshot = new QueuedItemSnapshot(
					queueName,
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