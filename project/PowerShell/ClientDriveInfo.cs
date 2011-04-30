// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientDriveInfo.cs" company="The CruiseControl.NET Team">
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
    using System.Management.Automation;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Exposes a drive to the provider.
    /// </summary>
    public class ClientDriveInfo
        : PSDriveInfo
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;

        /// <summary>
        /// The root level folder for a server.
        /// </summary>
        private readonly ServerFolder serverFolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDriveInfo"/> class.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="drive">The drive.</param>
        public ClientDriveInfo(string address, PSDriveInfo drive)
            : base(drive)
        {
            var clientFactory = new CruiseServerClientFactory();
            this.client = clientFactory.GenerateClient(address);
            var version = new Version(this.client.GetServerVersion());
            this.serverFolder = new ServerFolder(this.client, "\\", version);
        }
        #endregion

        #region Public properties
        #region Client
        /// <summary>
        /// Gets the client.
        /// </summary>
        public CruiseServerClientBase Client
        {
            get { return this.client; }
        }
        #endregion

        #region Root
        /// <summary>
        /// Gets the root folder.
        /// </summary>
        public ServerFolder RootFolder
        {
            get { return this.serverFolder; }
        }
        #endregion
        #endregion
    }
}
