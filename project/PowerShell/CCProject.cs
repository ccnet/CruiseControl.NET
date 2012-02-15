// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCProject.cs" company="The CruiseControl.NET Team">
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
    using ThoughtWorks.CruiseControl.Remote.Parameters;

    /// <summary>
    /// Information about a project.
    /// </summary>
    public class CCProject
        : ProjectStatus
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CCProject"/> class from being created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <param name="activity">The activity.</param>
        /// <param name="buildStatus">The build status.</param>
        /// <param name="status">The status.</param>
        /// <param name="webURL">The web URL.</param>
        /// <param name="lastBuildDate">The last build date.</param>
        /// <param name="lastBuildLabel">The last build label.</param>
        /// <param name="lastSuccessfulBuildLabel">The last successful build label.</param>
        /// <param name="nextBuildTime">The next build time.</param>
        /// <param name="buildStage">The build stage.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="queuePriority">The queue priority.</param>
        /// <param name="parameters">The project parameters</param>
        private CCProject(CruiseServerClientBase client, string name, string category, ProjectActivity activity, IntegrationStatus buildStatus, ProjectIntegratorState status, string webURL, DateTime lastBuildDate, string lastBuildLabel, string lastSuccessfulBuildLabel, DateTime nextBuildTime, string buildStage, string queue, int queuePriority, ParameterBase[] parameters)
            : base(name, category, activity, buildStatus, status, webURL, lastBuildDate, lastBuildLabel, lastSuccessfulBuildLabel, nextBuildTime, buildStage, queue, queuePriority, parameters)
        {
            this.client = client;
        }
        #endregion

        #region Public properties
        #region Connection
        /// <summary>
        /// Gets the connection.
        /// </summary>
        public CCConnection Connection { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region ForceBuild()
        /// <summary>
        /// Forces a build.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public void ForceBuild(BuildCondition condition)
        {
            this.client.ForceBuild(this.Name, new List<NameValuePair>(), condition);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Aborts a build.
        /// </summary>
        public void AbortBuild()
        {
            this.client.AbortBuild(this.Name);
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the project.
        /// </summary>
        public void Start()
        {
            this.client.StartProject(this.Name);
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the project.
        /// </summary>
        public void Stop()
        {
            this.client.StopProject(this.Name);
        }
        #endregion

        #region Refresh()
        /// <summary>
        /// Generates a refreshed istance.
        /// </summary>
        /// <returns>
        /// The new <see cref="CCProject"/> instance.
        /// </returns>
        public CCProject Refresh()
        {
            var statuses = this.client.GetProjectStatus();
            var status = statuses.FirstOrDefault(s => s.Name.Equals(this.Name));
            return status == null ? null : Wrap(this.client, status, this.Connection);
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <returns>
        /// Retrieves the log.
        /// </returns>
        public string GetLog()
        {
            var log = this.client.GetServerLog(this.Name);
            return log;
        }
        #endregion

        #region GetBuilds()
        /// <summary>
        /// Gets some project builds.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="count">The number to get.</param>
        /// <returns>
        /// The builds.
        /// </returns>
        public IList<CCBuild> GetBuilds(int start, int count)
        {
            var builds = this.client.GetBuildSummaries(this.Name, start, count);
            return builds.Select(b => new CCBuild(this.client, b, this)).ToList();
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
            var packages = this.client.RetrievePackageList(this.Name);
            var filtered = packages.OrderByDescending(p => p.DateTime).Skip(start).Take(count);
            return filtered.Select(p => CCPackage.Wrap(this.client, p, this)).ToList();
        }
        #endregion

        #region GetConfiguration()
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>
        /// The configuration for the project.
        /// </returns>
        public string GetConfiguration()
        {
            var config = this.client.GetProject(this.Name);
            return config;
        }
        #endregion
        #endregion

        #region Internal methods
        #region Wrap()
        /// <summary>
        /// Wraps the specified project status.
        /// </summary>
        /// <param name="owningClient">The owning client.</param>
        /// <param name="projectStatus">The project status.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// The new <see cref="CCProject"/>.
        /// </returns>
        internal static CCProject Wrap(CruiseServerClientBase owningClient, ProjectStatus projectStatus, CCConnection connection)
        {
            var project = new CCProject(
                owningClient,
                projectStatus.Name, 
                projectStatus.Category, 
                projectStatus.Activity, 
                projectStatus.BuildStatus, 
                projectStatus.Status, 
                projectStatus.WebURL, 
                projectStatus.LastBuildDate, 
                projectStatus.LastBuildLabel, 
                projectStatus.LastSuccessfulBuildLabel, 
                projectStatus.NextBuildTime, 
                projectStatus.BuildStage, 
                projectStatus.Queue,
                projectStatus.QueuePriority,
                projectStatus.Parameters.ToArray()) { Connection = connection };
            return project;
        }
        #endregion
        #endregion
    }
}
