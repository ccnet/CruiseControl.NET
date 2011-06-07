// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCConnection.cs" company="The CruiseControl.NET Team">
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
    using System.Linq;

    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Defines a connection to a CruiseControl.NET server.
    /// </summary>
    public class CCConnection
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CCConnection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="version">The version.</param>
        public CCConnection(CruiseServerClientBase client, Version version)
        {
            this.client = client;
            this.Version = version;
        }
        #endregion

        #region Public properties
        #region Version
        /// <summary>
        /// Gets the version.
        /// </summary>
        public Version Version { get; private set; }
        #endregion

        #region Address
        /// <summary>
        /// Gets the address.
        /// </summary>
        public string Address
        {
            get { return this.client.Address; }
        }
        #endregion

        #region Target
        /// <summary>
        /// Gets the target.
        /// </summary>
        public string Target
        {
            get { return this.client.TargetServer; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetLog()
        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <returns>
        /// Retrieves the log.
        /// </returns>
        public string GetLog()
        {
            var log = this.client.GetServerLog();
            return log;
        }
        #endregion

        #region GetProjects()
        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <returns>
        /// The projects for the server.
        /// </returns>
        public ICollection<CCProject> GetProjects()
        {
            var snapshot = this.client.GetCruiseServerSnapshot();
            var projects = snapshot.ProjectStatuses.Select(p => CCProject.Wrap(this.client, p, this));
            return projects.ToList();
        }
        #endregion
        #endregion
    }
}
