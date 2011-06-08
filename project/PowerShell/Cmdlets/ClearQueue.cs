// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetQueue.cs" company="The CruiseControl.NET Team">
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

namespace ThoughtWorks.CruiseControl.PowerShell.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// A cmdlet for clearing pending requests from a queue.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, Nouns.Queue, DefaultParameterSetName = CommonCmdlet.CommonParameterSet)]
    public class ClearQueue
        : ConnectionCmdlet
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the queue name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ConnectionCmdlet.ConnectionParameterSet)]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = CommonCmdlet.CommonParameterSet)]
        public string QueueName { get; set; }
        #endregion

        #region Queue
        /// <summary>
        /// Gets or sets the queue.
        /// </summary>
        /// <value>
        /// The queue.
        /// </value>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "QueueParameterSet")]
        [ValidateNotNull]
        public CCQueue Queue { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes a record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Clearing queue");

            Action<CCQueue> action = q =>
                {
                    var queue = q.Refresh();
                    foreach (var request in queue.Requests)
                    {
                        queue.CancelRequest(request);
                    }

                    this.WriteObject(queue.Refresh());
                };

            if (this.Queue != null)
            {
                action(this.Queue);
            }
            else
            {
                var connection = this.Connection
                                 ?? new CCConnection(ClientHelpers.GenerateClient(this.Address, this), new Version());

                var project = connection.GetQueues().FirstOrDefault(
                        p => p.Name.Equals(this.QueueName, StringComparison.CurrentCultureIgnoreCase));
                if (project == null)
                {
                    var record = new ErrorRecord(
                        new Exception("Unable to find queue '" + this.QueueName + "'"),
                        "1",
                        ErrorCategory.ResourceUnavailable,
                        this);
                    this.WriteError(record);
                }
                else
                {
                    action(project);
                }
            }
        }
        #endregion
        #endregion
    }
}
