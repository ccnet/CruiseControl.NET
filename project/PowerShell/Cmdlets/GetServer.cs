// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServer.cs" company="The CruiseControl.NET Team">
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
    using System.Management.Automation;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A cmdlet for getting one or more projects.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Server, DefaultParameterSetName = "ServerSet")]
    public class GetServer
        : Cmdlet
    {
        #region Public properties
        #region Folder
        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>
        /// The server folder.
        /// </value>
        [Parameter(ParameterSetName = "FolderSet", Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public ServerFolder Folder { get; set; }
        #endregion

        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        [Parameter(ParameterSetName = "ServerSet", Mandatory = true, Position = 1)]
        [ValidateNotNull]
        public string Server { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes a record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.Folder != null)
            {
                this.WriteObject(this.Folder);
            }
            else
            {
                try
                {
                    var clientFactory = new CruiseServerClientFactory();
                    var client = clientFactory.GenerateClient(this.Server);
                    var version = new Version(client.GetServerVersion());
                    var serverFolder = new ServerFolder(client, "\\", version);
                    this.WriteObject(serverFolder);
                }
                catch (CommunicationsException error)
                {
                    var record = new ErrorRecord(
                        error,
                        "Communications",
                        ErrorCategory.NotSpecified,
                        this.Server);
                    this.WriteError(record);
                    return;
                }
            }
        }
        #endregion
        #endregion
    }
}
