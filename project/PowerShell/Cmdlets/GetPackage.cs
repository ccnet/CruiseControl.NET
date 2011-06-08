// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPackage.cs" company="The CruiseControl.NET Team">
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
    using System.Management.Automation;

    /// <summary>
    /// A cmdlet for getting one or more packages.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, Nouns.Package, DefaultParameterSetName = CommonCmdlet.CommonParameterSet)]
    public class GetPackage
        : ProjectCmdlet
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPackage"/> class.
        /// </summary>
        public GetPackage()
        {
            this.Count = 50;
        }
        #endregion

        #region Public properties
        #region Start
        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        /// <value>
        /// The start position.
        /// </value>
        [Parameter]
        public int Start { get; set; }
        #endregion

        #region Count
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Parameter]
        public int Count { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes a record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Getting project packages");
            this.ProcessProject(p =>
                {
                    var builds = p.GetPackages(this.Start, this.Count);
                    this.WriteObject(builds, true);
                });
        }
        #endregion
        #endregion
    }
}
