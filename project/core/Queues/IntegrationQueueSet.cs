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
		private Hashtable content = new Hashtable();
		private bool isQueueContentChanged = true;
		private string[] cachedQueueNames;
		private IntegrationQueueSnapshot cachedIntegrationQueueSnapshot;

		public IIntegrationQueue this[string queueName]
		{
			get
			{
				if (content.ContainsKey(queueName))
				{
					return (IIntegrationQueue) content[queueName];
				}
				else
				{
					return null;
				}
			}
		}

		public void Add(string queueName)
		{
			IIntegrationQueue queue;
			if (!content.ContainsKey(queueName))
			{
				queue = new IntegrationQueue(this);
				content.Add(queueName, queue);
			}
		}

		public void Clear()
		{
			content.Clear();
			cachedQueueNames = null;
			isQueueContentChanged = true;
		}

		public object SyncRoot
		{
			get { return content.SyncRoot; }
		}

		public bool IsQueueContentChanged
		{
			get { return isQueueContentChanged; }
			set { isQueueContentChanged = value; }
		}

		public string[] GetQueueNames()
		{
			if (cachedQueueNames == null)
			{
				cachedQueueNames = new string[content.Keys.Count];
				content.Keys.CopyTo(cachedQueueNames, 0);
				Array.Sort(cachedQueueNames);
			}
			return cachedQueueNames;
		}

		public IntegrationQueueSnapshot GetIntegrationQueueSnapshot()
		{
			if (isQueueContentChanged)
			{
				lock (SyncRoot)
				{
					cachedIntegrationQueueSnapshot = BuildQueueContentSnapshot();
					isQueueContentChanged = false;
				}
			}
			return cachedIntegrationQueueSnapshot;
		}

		private IntegrationQueueSnapshot BuildQueueContentSnapshot()
		{
			Log.Debug("Rebuilding integration queue snapshot cache");
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
			QueuedItemSnapshot queuedItemSnapshot;

			foreach (IIntegrationQueueItem integrationQueueItem in queue)
			{
				queuedItemSnapshot = new QueuedItemSnapshot(
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