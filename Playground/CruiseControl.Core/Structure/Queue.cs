namespace CruiseControl.Core.Structure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Xaml;
    using NLog;

    /// <summary>
    /// Schedules integrations based on their position in the queue.
    /// </summary>
    public class Queue
        : ServerItemContainerBase
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IList<IntegrationContext> pendingRequests = new List<IntegrationContext>();
        private readonly IList<IntegrationContext> activeRequests = new List<IntegrationContext>();
        private readonly ReaderWriterLockSlim interleave = new ReaderWriterLockSlim();
        private IntegrationContext currentContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class.
        /// </summary>
        public Queue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public Queue(string name, params ServerItem[] children)
            : base(name, children)
        {
        }
        #endregion

        #region Public properties
        #region AllowedActive
        /// <summary>
        /// Gets or sets the number of allowed active children.
        /// </summary>
        /// <value>The allowed active.</value>
        [DefaultValue(null)]
        public int? AllowedActive { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region SetPriority()
        /// <summary>
        /// Sets the priority on an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        public static void SetPriority(ServerItem item, int? priority)
        {
            var memberIdentifier = new AttachableMemberIdentifier(typeof(Queue), "Priority");
            if (priority.HasValue)
            {
                AttachablePropertyServices.SetProperty(
                    item,
                    memberIdentifier,
                    priority);
            }
            else
            {
                AttachablePropertyServices.RemoveProperty(
                    item,
                    memberIdentifier);
            }
        }
        #endregion

        #region GetPriority()
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The priority if the item has one; <c>null</c> otherwise.
        /// </returns>
        public static int? GetPriority(ServerItem item)
        {
            int? priority;
            return AttachablePropertyServices.TryGetProperty(
                item,
                new AttachableMemberIdentifier(typeof(Queue), "Priority"),
                out priority) ? priority : null;
        }
        #endregion

        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public override void AskToIntegrate(IntegrationContext context)
        {
            var locked = false;
            try
            {
                logger.Debug("Adding integration request for '{0}' to '{1}'", context.Item.Name, this.Name);
                locked = this.interleave.TryEnterWriteLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    if ((this.activeRequests.Count < this.AllowedActive.GetValueOrDefault(1)) &&
                        this.AskHost())
                    {
                        logger.Info("Activating '{0}' in '{1}'", context.Item.Name, this.Name);
                        this.activeRequests.Add(context);
                        context.Completed += OnIntegrationCompleted;
                    }
                    else
                    {
                        logger.Info("Adding '{0}' to pending in '{1}'", context.Item.Name, this.Name);
                        this.pendingRequests.Add(context);
                        context.Lock();
                    }
                }
                else
                {
                    var message = "Unable to add new request to '" +
                        this.Name +
                        "' - unable to acquire lock";
                    logger.Error(message);
                    // TODO: Replace with custom exception
                    throw new Exception(message);
                }
            }
            finally
            {
                if (locked)
                {
                    this.interleave.ExitWriteLock();
                }
            }
        }
        #endregion

        #region GetPendingRequests()
        /// <summary>
        /// Gets the pending requests.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IntegrationContext> GetPendingRequests()
        {
            var locked = false;
            try
            {
                logger.Debug("Getting pending requests for '{0}'", this.Name);
                locked = this.interleave.TryEnterReadLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    return this.pendingRequests.ToArray();
                }
                else
                {
                    var message = "Unable to retrieve pending requests from '" +
                        this.Name +
                        "' - unable to acquire lock";
                    logger.Error(message);
                    // TODO: Replace with custom exception
                    throw new Exception(message);
                }
            }
            finally
            {
                if (locked)
                {
                    this.interleave.ExitReadLock();
                }
            }
        }
        #endregion

        #region GetActiveRequests()
        /// <summary>
        /// Gets the active requests.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IntegrationContext> GetActiveRequests()
        {
            var locked = false;
            try
            {
                logger.Debug("Getting active requests for '{0}'", this.Name);
                locked = this.interleave.TryEnterReadLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    return this.activeRequests.ToArray();
                }
                else
                {
                    var message = "Unable to retrieve active requests from '" +
                        this.Name +
                        "' - unable to acquire lock";
                    logger.Error(message);
                    // TODO: Replace with custom exception
                    throw new Exception(message);
                }
            }
            finally
            {
                if (locked)
                {
                    this.interleave.ExitReadLock();
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region OnIntegrationCompleted()
        /// <summary>
        /// Called when an integration has completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnIntegrationCompleted(object sender, EventArgs e)
        {
            var context = sender as IntegrationContext;
            if ((this.currentContext != null) && !this.currentContext.IsCompleted)
            {
                logger.Debug("Telling host integration has completed in '{0}'", this.Name);
                this.currentContext.Complete();
                this.currentContext = null;
            }

            IntegrationContext nextRequest = null;
            var locked = false;
            try
            {
                locked = this.interleave.TryEnterWriteLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    logger.Debug("Removing '{0}' from '{1}'", context.Item.Name, this.Name);
                    context.Completed -= OnIntegrationCompleted;
                    this.activeRequests.Remove(context);

                    if ((this.pendingRequests.Count > 0) && this.AskHost())
                    {
                        nextRequest = this.pendingRequests[0];
                        this.pendingRequests.Remove(nextRequest);
                        logger.Info("Activating '{0}' in '{1}'", nextRequest.Item.Name, this.Name);
                        this.activeRequests.Add(nextRequest);
                        nextRequest.Completed += OnIntegrationCompleted;
                    }
                }
                else
                {
                    var message = "Unable to update queue '" +
                        this.Name +
                        "' - unable to acquire lock";
                    logger.Error(message);
                    // TODO: Replace with custom exception
                    throw new Exception(message);
                }
            }
            finally
            {
                if (locked)
                {
                    this.interleave.ExitWriteLock();
                }
            }

            // Release the next request
            if (nextRequest != null)
            {
                nextRequest.Release();
            }
        }
        #endregion

        #region OnContextReleased()
        /// <summary>
        /// Called when the current context has been released.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnContextReleased(object sender, EventArgs e)
        {
            var integrate = !this.currentContext.WasCancelled;
            var locked = false;
            IntegrationContext nextRequest = null;
            try
            {
                locked = this.interleave.TryEnterWriteLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    if (this.pendingRequests.Count > 0)
                    {
                        nextRequest = this.pendingRequests[0];
                        this.pendingRequests.Remove(nextRequest);
                        if (integrate)
                        {
                            logger.Info("Activating '{0}' in '{1}'", nextRequest.Item.Name, this.Name);
                            this.activeRequests.Add(nextRequest);
                            nextRequest.Completed += OnIntegrationCompleted;
                        }
                    }
                }
                else
                {
                    var message = "Unable to update queue '" +
                        this.Name +
                        "' - unable to acquire lock";
                    logger.Error(message);
                    // TODO: Replace with custom exception
                    throw new Exception(message);
                }
            }
            finally
            {
                if (locked)
                {
                    this.interleave.ExitWriteLock();
                }
            }

            // Release or cancel the next request
            if (nextRequest != null)
            {
                if (integrate)
                {
                    nextRequest.Release();
                }
                else
                {
                    logger.Info("Cancelling '{0}' in '{1}'", nextRequest.Item.Name, this.Name);
                    nextRequest.Cancel();
                }
            }
        }
        #endregion

        #region AskHost()
        /// <summary>
        /// Asks the host if an integration can start.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the integration can start; <c>false</c> otherwise.
        /// </returns>
        private bool AskHost()
        {
            var integrate = this.Host == null;
            if (!integrate && (this.currentContext == null))
            {
                this.currentContext = new IntegrationContext(this);
                logger.Debug("Asking host if '{0}' can integrate", this.Name);
                this.Host.AskToIntegrate(this.currentContext);
                if (this.currentContext.IsLocked)
                {
                    this.currentContext.Released += OnContextReleased;
                }
                else
                {
                    integrate = !this.currentContext.WasCancelled;
                    if (!integrate)
                    {
                        this.currentContext = null;
                    }
                }
            }

            return integrate;
        }
        #endregion
        #endregion
    }
}
