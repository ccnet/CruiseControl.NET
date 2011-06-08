// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCBuild.cs" company="The CruiseControl.NET Team">
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
    using System.Collections.Generic;
    using System.Linq;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// The details on a build.
    /// </summary>
    public class CCBuild
        : BuildSummary
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CCBuild"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="summary">The summary.</param>
        /// <param name="project">The project.</param>
        public CCBuild(CruiseServerClientBase client, BuildSummary summary, CCProject project)
            : base(summary)
        {
            this.client = client;
            this.Project = project;
        }
        #endregion

        #region Public properties
        #region Project
        /// <summary>
        /// Gets the project.
        /// </summary>
        public CCProject Project { get; private set; }
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
            var log = this.client.GetLog(this.Project.Name, this.LogName);
            return log;
        }
        #endregion

        #region GetPackages()
        /// <summary>
        /// Gets the packages.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="count">The number to get.</param>
        /// <returns>
        /// The packages for the project.
        /// </returns>
        public IList<CCPackage> GetPackages(int start, int count)
        {
            var packages = this.client.RetrievePackageList(this.Project.Name, this.Label);
            var filtered = packages.OrderByDescending(p => p.DateTime).Skip(start).Take(count);
            return filtered.Select(p => CCPackage.Wrap(this.client, p, this.Project)).ToList();
        }
        #endregion
        #endregion
    }
}
