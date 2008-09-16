using System;
using System.Collections;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Config;
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
        private readonly IQueueConfiguration _configuration;
        private readonly List<string> _lockingQueueNames;
        private readonly IntegrationQueueSet _parentQueueSet;

        private static readonly object _queueLockSync = new object();

		public IntegrationQueue(string name, IQueueConfiguration configuration, IntegrationQueueSet parentQueueSet)
		{
			this.name = name;
            _configuration = configuration;
            _parentQueueSet = parentQueueSet;

            _lockingQueueNames = new List<string>();
		}

		public string Name
		{
			get { return name; }
		}

        /// <summary>
        /// Is this Queue locked by another (N) Queue(s)?
        /// </summary>
        public virtual bool IsLocked
        {
            get
            {
                lock (_queueLockSync) 
                { 
                    return _lockingQueueNames.Count != 0; 
                }
            }
        }
        
        /// <summary>
        /// The configuration settings for this queue.
        /// </summary>
        public virtual IQueueConfiguration Configuration
        {
            get { return _configuration; }
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

					int? foundIndex = null;
                    bool addItem = true;
                    IIntegrationQueueItem foundItem = null;

					for (int index = 1; index < Count; index++)
					{
						IIntegrationQueueItem queuedItem = GetIntegrationQueueItem(index);
						if (queuedItem.Project == integrationQueueItem.Project)
						{
                            foundItem = queuedItem;
                            foundIndex = index;
                            break;

						}
					}

					if (foundIndex != null)
 					{
                        switch (_configuration.HandlingMode)
                        {
                            case QueueDuplicateHandlingMode.UseFirst:
                                Log.Info(String.Format("Project: {0} already on queue: {1} - cancelling new request", integrationQueueItem.Project.Name, Name));
                                addItem = false;
                                break;
                            case QueueDuplicateHandlingMode.ApplyForceBuildsReAdd:
                                if (foundItem.IntegrationRequest.BuildCondition >= integrationQueueItem.IntegrationRequest.BuildCondition)
                                {
                                    Log.Info(String.Format("Project: {0} already on queue: {1} - cancelling new request", integrationQueueItem.Project.Name, Name));
                                    addItem = false;
                                }
                                else
                                {
                                    Log.Info(String.Format("Project: {0} already on queue: {1} with lower prority - cancelling existing request", integrationQueueItem.Project.Name, Name));
                                    NotifyExitingQueueAndRemoveItem(foundIndex.Value, foundItem, true);
                                }
                                break;
                            case QueueDuplicateHandlingMode.ApplyForceBuildsReplace:
                                addItem = false;
                                if (foundItem.IntegrationRequest.BuildCondition >= integrationQueueItem.IntegrationRequest.BuildCondition)
                                {
                                    Log.Info(String.Format("Project: {0} already on queue: {1} - cancelling new request", integrationQueueItem.Project.Name, Name));
                                }
                                else
                                {
                                    Log.Info(String.Format("Project: {0} already on queue: {1} with lower prority - replacing existing request at position {2}", integrationQueueItem.Project.Name, Name, foundIndex));
                                    NotifyExitingQueueAndRemoveItem(foundIndex.Value, foundItem, true);
                                    AddToQueue(integrationQueueItem, foundIndex);
                                }
                                break;
                            default:
                                throw new ConfigurationException("Unknown handling mode for duplicates: " + _configuration.HandlingMode);
                        }
 					}

                    if (addItem) AddToQueue(integrationQueueItem);
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

            if (IsLocked) return null;

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
						if ((queuedIntegrationQueueItem != null) && (queuedIntegrationQueueItem.Project == project))
							return true;
					}
				}
				return false;
			}
		}

        private void AddToQueue(IIntegrationQueueItem integrationQueueItem)
        {
            AddToQueue(integrationQueueItem, null);
        }

		private void AddToQueue(IIntegrationQueueItem integrationQueueItem, int? queuePosition)
		{
            if (!queuePosition.HasValue)
            {
                queuePosition = GetPrioritisedQueuePosition(integrationQueueItem.Project.QueuePriority);
                Log.Info(string.Format("Project: '{0}' is added to queue: '{1}' in position {2}.",
                                       integrationQueueItem.Project.Name, Name, queuePosition));
            }
			integrationQueueItem.IntegrationQueueNotifier.NotifyEnteringIntegrationQueue();
			Insert(queuePosition.Value, integrationQueueItem);
		}

		private int GetPrioritisedQueuePosition(int insertingItemPriority)
		{
			// Assume the back of the queue will be where we insert it.
			int targetQueuePosition = Count;

			// Items with priority zero always get added to the end of the queue, as will anything if the
			// queue only has one item in it as we assume that item is integrating already and cannot be moved.
			if (insertingItemPriority != 0 && Count > 1)
			{
				for (int index = 1; index < Count; index++)
				{
					IIntegrationQueueItem queuedIntegrationQueueItem = this[index] as IIntegrationQueueItem;
					if (queuedIntegrationQueueItem != null)
					{
						int compareQueuePosition = queuedIntegrationQueueItem.Project.QueuePriority;
						// If two items have the same priority it will be FIFO order for that priority
						if (compareQueuePosition == 0 || compareQueuePosition > insertingItemPriority)
						{
							targetQueuePosition = index;
							break;
						}
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

        /// <summary>
        /// Toggle Locks. This instructs the queue that it should acquire (or release) locks upon the other queues which it is configured 
        /// to lock when integrating.
        /// </summary>
        /// <param name="acquire">Should the queue acquire locks or release them?</param>
        public void ToggleQueueLocks(bool acquire)
        {
            if (!string.IsNullOrEmpty(_configuration.LockQueueNames) && _parentQueueSet != null)
            {
                string[] queues = _configuration.LockQueueNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < queues.Length; i++)
                {
                    if(acquire)
                    {
                        // find queue and lock it
                        _parentQueueSet[queues[i]].LockQueue(this);

                        Log.Info(string.Format("Queue: '{0}' has acquired a lock against queue '{1}'", Name, queues[i]));
                    }
                    else
                    {
                        // find queue and lock it
                        _parentQueueSet[queues[i]].UnlockQueue(this);

                        Log.Info(string.Format("Queue: '{0}' has released a lock against queue '{1}'", Name, queues[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Lock this queue, based upon a request from another queue.
        /// Acquires a fresh lock for the queue making the request (assuming none exists).
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be taken out</param>
        public void LockQueue(IIntegrationQueue requestingQueue)
        {
            lock (_queueLockSync)
            {
                if (!_lockingQueueNames.Contains(requestingQueue.Name))
                {
                    _lockingQueueNames.Add(requestingQueue.Name);
                }
            }
        }

        /// <summary>
        /// Unlock this queue, based upon a request from another queue.
        /// Releases any locks currently held by the queue making the request.
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be released</param>
        public void UnlockQueue(IIntegrationQueue requestingQueue)
        {
            lock (_queueLockSync)
            {
                _lockingQueueNames.Remove(requestingQueue.Name);
            }
        }
    }
}
