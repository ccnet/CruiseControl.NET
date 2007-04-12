using System.Collections;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Implementation of a named integration queue.
	/// The currently integrating project in this queue will be at queue position zero.
	/// </summary>
	public class IntegrationQueue : ArrayList, IIntegrationQueue
	{
		private readonly string name;

		public IntegrationQueue(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Add a project integration request be added to the integration queue.
		/// If no requests are on that queue already the integration is just kicked off immediately.
		/// If the request is a force build and an integration is already on the queue for that project
		/// then the queue request is ignored as it is redundant.
		/// </summary>
		/// <param name="integrationQueueItem">The integration queue item.</param>
		public void Enqueue(IIntegrationQueueItem integrationQueueItem)
		{
			lock (this)
			{
				if (Count == 0)
				{
					// We can start integration straight away as first in first served
					AddToQueue(integrationQueueItem);
				}
				else
				{
					// We need to see if we already have a integration request for this project on the queue
					// If so then we will ignore the latest request.
					// Note we start at queue position 1 since position 0 is currently integrating.
					bool isAlreadyQueued = false;
					for (int index = 1; index < Count; index++)
					{
						IIntegrationQueueItem queuedIntegrationQueueItem = GetIntegrationQueueItem(index);
						if (queuedIntegrationQueueItem.Project == integrationQueueItem.Project)
						{
							Log.Info("Project: " + integrationQueueItem.Project.Name + " already on queue: " + Name + " - cancelling new request");
							isAlreadyQueued = true;
							break;
						}
					}
					if (!isAlreadyQueued)
					{
						AddToQueue(integrationQueueItem);
					}
				}
			}
		}

		private IIntegrationQueueItem GetIntegrationQueueItem(int index)
		{
			return this[index] as IIntegrationQueueItem;
		}

		/// <summary>
		/// Releases the next integration request on the queue to start it's integration.
		/// </summary>
		public void Dequeue()
		{
			lock (this)
			{
				if (Count > 0)
				{
					// The first item in the queue has now been integrated so discard it.
					IIntegrationQueueItem integrationQueueItem = (IIntegrationQueueItem) this[0];
					NotifyExitingQueueAndRemoveItem(0, integrationQueueItem, false);
				}
			}
		}

		/// <summary>
		/// Removes a pending integration request (i.e. one that has not yet started) for this
		/// project from the queue if it is available.
		/// </summary>
		/// <param name="project">The project to have pending items removed from the queue.</param>
		public void RemovePendingRequest(IProject project)
		{
			lock (this)
			{
				bool considerFirstQueueItem = false;
				RemoveProjectItems(project, considerFirstQueueItem);
			}
		}

		/// <summary>
		/// Removes all queued integrations for this project. To be invoked when "stopping"
		/// a project.
		/// </summary>
		/// <param name="project">The project to be removed.</param>
		public void RemoveProject(IProject project)
		{
			lock (this)
			{
				bool considerFirstQueueItem = true;
				RemoveProjectItems(project, considerFirstQueueItem);
			}
		}

		/// <summary>
		/// Returns an array of the current queued integrations on the queue.
		/// </summary>
		/// <returns>Array of current queued integrations on the queue.</returns>
		public IIntegrationQueueItem[] GetQueuedIntegrations()
		{
			return (IIntegrationQueueItem[]) ToArray(typeof (IIntegrationQueueItem));
		}

		public IntegrationRequest GetNextRequest(IProject project)
		{
			if (Count == 0) return null;
			IIntegrationQueueItem item = GetIntegrationQueueItem(0);
			if (item != null && item.Project == project)
				return item.IntegrationRequest;
			return null;
		}
	
		public bool HasItemOnQueue(IProject project)
		{
			return HasItemOnQueue(project, /* pendingItemsOnly*/ false);
		}
		
		public bool HasItemPendingOnQueue(IProject project)
		{
			return HasItemOnQueue(project, /* pendingItemsOnly*/ true);
		}

		private bool HasItemOnQueue(IProject project, bool pendingItemsOnly)
		{
			lock (this)
			{
				int startIndex = pendingItemsOnly ? 1 : 0;
				if (Count > startIndex)
				{
					for	(int index = startIndex; index < Count; index++)
					{
						IIntegrationQueueItem queuedIntegrationQueueItem = this[index] as IIntegrationQueueItem;
						if (queuedIntegrationQueueItem.Project == project)
							return true;
					}
				}
				return false;
			}
		}

		private void AddToQueue(IIntegrationQueueItem integrationQueueItem)
		{
			int queuePosition = GetPrioritisedQueuePosition(integrationQueueItem.Project.QueuePriority);

			Log.Info(string.Format("Project: '{0}' is added to queue: '{1}' in position {2}.",
			                       integrationQueueItem.Project.Name, Name, queuePosition));
			integrationQueueItem.IntegrationQueueNotifier.NotifyEnteringIntegrationQueue();
			Insert(queuePosition, integrationQueueItem);
		}

		private int GetPrioritisedQueuePosition(int insertingItemPriority)
		{
			// Assume the back of the queue will be where we insert it.
			int targetQueuePosition = Count;

			// Items with priority zero always get added to the end of the queue, as will anything if the
			// queue only has one item in it as we assume that item is integrating already and cannot be moved.
			if (insertingItemPriority != 0 && Count > 1)
			{
				int compareQueuePosition;
				for (int index = 1; index < Count; index++)
				{
					IIntegrationQueueItem queuedIntegrationQueueItem = this[index] as IIntegrationQueueItem;
					compareQueuePosition = queuedIntegrationQueueItem.Project.QueuePriority;
					// If two items have the same priority it will be FIFO order for that priority
					if (compareQueuePosition == 0 || compareQueuePosition > insertingItemPriority)
					{
						targetQueuePosition = index;
						break;
					}
				}
			}
			return targetQueuePosition;
		}

		private void RemoveProjectItems(IProject project, bool considerFirstQueueItem)
		{
			// Note we are also potentially removing the item at index[0] as this method should
			// only be called when the thread performing the build has been stopped.
			int startQueueIndex = considerFirstQueueItem ? 0 : 1;
			for (int index = Count - 1; index >= startQueueIndex; index--)
			{
				IIntegrationQueueItem integrationQueueItem = (IIntegrationQueueItem) this[index];
				if (integrationQueueItem.Project.Equals(project))
				{
					Log.Info("Project: " + integrationQueueItem.Project.Name + " removed from queue: " + Name);
					bool isPendingItemCancelled = index > 0;
					NotifyExitingQueueAndRemoveItem(index, integrationQueueItem, isPendingItemCancelled);
				}
			}
		}

		private void NotifyExitingQueueAndRemoveItem(int index, IIntegrationQueueItem integrationQueueItem, bool isPendingItemCancelled)
		{
			integrationQueueItem.IntegrationQueueNotifier.NotifyExitingIntegrationQueue(isPendingItemCancelled);
			RemoveAt(index);
		}
	}
}