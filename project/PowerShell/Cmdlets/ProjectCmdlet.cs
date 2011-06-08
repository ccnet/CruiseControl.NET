// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCmdlet.cs" company="The CruiseControl.NET Team">
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
    /// Base class for cmdlets that require a project as a parameter.
    /// </summary>
    public abstract class ProjectCmdlet
        : ConnectionCmdlet
    {
        #region Public constants
        /// <summary>
        /// The name of the common parameter set.
        /// </summary>
        public const string ProjectParameterSet = "ProjectParameterSet";
        #endregion

        #region Public properties
        #region ProjectName
        /// <summary>
        /// Gets or sets an optional name to filter the projects by.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ConnectionCmdlet.ConnectionParameterSet)]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = CommonCmdlet.CommonParameterSet)]
        public string ProjectName { get; set; }
        #endregion

        #region Project
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ProjectCmdlet.ProjectParameterSet)]
        [ValidateNotNull]
        public CCProject Project { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessProject()
        /// <summary>
        /// Processes the project.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        protected void ProcessProject(Action<CCProject> action)
        {
            if (this.Project != null)
            {
                action(this.Project);
            }
            else
            {
                var connection = this.Connection
                                 ?? new CCConnection(ClientHelpers.GenerateClient(this.Address, this), new Version());

                var project =
                    connection.GetProjects().FirstOrDefault(
                        p => p.Name.Equals(this.ProjectName, StringComparison.CurrentCultureIgnoreCase));
                if (project == null)
                {
                    var record = new ErrorRecord(
                        new Exception("Unable to find project '" + this.ProjectName + "'"),
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
