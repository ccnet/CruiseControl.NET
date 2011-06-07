// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProject.cs" company="The CruiseControl.NET Team">
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
    /// A cmdlet for getting one or more projects.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Project, DefaultParameterSetName = CommonCmdlet.CommonParameterSet)]
    public class GetProject
        : ConnectionCmdlet
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets an optional name to filter the projects by.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Parameter]
        public string Name { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes a record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Getting project");
            var connection = this.Connection
                             ?? new CCConnection(ClientHelpers.GenerateClient(this.Address, this), new Version());

            var projects = connection.GetProjects();
            this.WriteObject(
                !string.IsNullOrEmpty(this.Name) ? projects.Where(p => p.Name.IndexOf(this.Name, StringComparison.CurrentCultureIgnoreCase) >= 0) : projects, 
                true);
        }
        #endregion
        #endregion
    }
}
