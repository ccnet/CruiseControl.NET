// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCQueue.cs" company="The CruiseControl.NET Team">
//   Copyright (C) 2011 by The CruiseControl.NET Team
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Information about a queue.
    /// </summary>
    public class CCQueue
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;

        /// <summary>
        /// The current requests,
        /// </summary>
        private readonly List<QueuedRequestSnapshot> requests = new List<QueuedRequestSnapshot>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CCQueue"/> class from being created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="name">The name.</param>
        /// <param name="requests">The requests.</param>
        private CCQueue(CruiseServerClientBase client, string name, IEnumerable<QueuedRequestSnapshot> requests)
        {
            this.client = client;
            this.Name = name;
            this.requests.AddRange(
                requests.Select(r => new QueuedRequestSnapshot(r.ProjectName, r.Activity, r.RequestTime)));
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }
        #endregion

        #region Connection
        /// <summary>
        /// Gets the connection.
        /// </summary>
        [Browsable(false)]
        public CCConnection Connection { get; private set; }
        #endregion

        #region Requests
        /// <summary>
        /// Gets the requests.
        /// </summary>
        public IEnumerable<QueuedRequestSnapshot> Requests
        {
            get { return this.requests; }
        }
        #endregion

        #region RequestsPending
        /// <summary>
        /// Gets the number of requests pending.
        /// </summary>
        public int RequestsPending
        {
            get { return this.requests.Count; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Refresh()
        /// <summary>
        /// Generates a refreshed istance.
        /// </summary>
        /// <returns>
        /// The new <see cref="CCProject"/> instance.
        /// </returns>
        public CCQueue Refresh()
        {
            var statuses = this.client.GetCruiseServerSnapshot();
            var status = statuses.QueueSetSnapshot.Queues.FirstOrDefault(s => s.QueueName.Equals(this.Name));
            return status == null ? null : Wrap(this.client, status, this.Connection);
        }
        #endregion

        #region CancelRequest()
        /// <summary>
        /// Cancels a pending request request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void CancelRequest(QueuedRequestSnapshot request)
        {
            if (request.Activity.IsPending())
            {
                this.client.CancelPendingRequest(request.ProjectName);
            }
        }
        #endregion
        #endregion

        #region Internal methods
        #region Wrap()
        /// <summary>
        /// Wraps the specified queue status.
        /// </summary>
        /// <param name="owningClient">The owning client.</param>
        /// <param name="queueStatus">The queue status.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// The new <see cref="CCProject"/>.
        /// </returns>
        internal static CCQueue Wrap(CruiseServerClientBase owningClient, QueueSnapshot queueStatus, CCConnection connection)
        {
            var queue = new CCQueue(owningClient, queueStatus.QueueName, queueStatus.Requests) { Connection = connection };
            return queue;
        }
        #endregion
        #endregion
    }
}