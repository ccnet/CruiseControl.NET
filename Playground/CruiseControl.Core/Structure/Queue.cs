namespace CruiseControl.Core.Structure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Schedules integrations based on their position in the queue.
    /// </summary>
    public class Queue
        : ServerItemContainerBase
    {
        #region Private fields
        private readonly IList<IntegrationRequest> pendingRequests = new List<IntegrationRequest>();
        private readonly IList<IntegrationRequest> activeRequests = new List<IntegrationRequest>();
        private readonly ReaderWriterLockSlim interleave = new ReaderWriterLockSlim();
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

        #region PendingRequests
        /// <summary>
        /// Gets the pending requests.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IntegrationRequest> PendingRequests
        {
            get
            {
                var locked = false;
                try
                {
                    locked = this.interleave.TryEnterReadLock(TimeSpan.FromSeconds(5));
                    if (locked)
                    {
                        return this.pendingRequests;
                    }
                    else
                    {
                        // TODO: Replace with custom exception
                        throw new Exception("Unable to retrieve pending requests - unable to acquire lock");
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
        }
        #endregion

        #region ActiveRequests
        /// <summary>
        /// Gets the active requests.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IntegrationRequest> ActiveRequests
        {
            get
            {
                var locked = false;
                try
                {
                    locked = this.interleave.TryEnterReadLock(TimeSpan.FromSeconds(5));
                    if (locked)
                    {
                        return this.activeRequests;
                    }
                    else
                    {
                        // TODO: Replace with custom exception
                        throw new Exception("Unable to retrieve active requests - unable to acquire lock");
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
        }
        #endregion
        #endregion

        #region Public methods
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
                locked = this.interleave.TryEnterWriteLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    if (this.activeRequests.Count <= this.AllowedActive.GetValueOrDefault(1))
                    {
                        this.activeRequests.Add(
                            new IntegrationRequest(context));
                        context.Completed += OnIntegrationCompleted;
                    }
                    else
                    {
                        this.pendingRequests.Add(
                            new IntegrationRequest(context));
                        context.Lock();
                    }
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
            var locked = false;
            try
            {
                locked = this.interleave.TryEnterWriteLock(TimeSpan.FromSeconds(5));
                if (locked)
                {
                    var request = this.activeRequests
                        .Single(r => ReferenceEquals(context, r.Context));
                    context.Completed -= OnIntegrationCompleted;
                    this.activeRequests.Remove(request);

                    if (this.pendingRequests.Count > 0)
                    {
                        var nextRequest = this.pendingRequests[0];
                        this.activeRequests.Add(nextRequest);
                        nextRequest.Context.Completed += OnIntegrationCompleted;
                        nextRequest.Context.Release();
                    }
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
        #endregion
    }
}
