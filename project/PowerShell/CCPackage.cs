// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCPackage.cs" company="The CruiseControl.NET Team">
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
    /// Information about a package.
    /// </summary>
    public class CCPackage
        : PackageDetails
    {
        #region Private fields
        /// <summary>
        /// The client to use.
        /// </summary>
        private readonly CruiseServerClientBase client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CCPackage"/> class from being created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="name">The name.</param>
        /// <param name="buildLabel">The build label.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="numberOfFiles">The number of files.</param>
        /// <param name="size">The size.</param>
        /// <param name="fileName">Name of the file.</param>
        private CCPackage(CruiseServerClientBase client, string name, string buildLabel, DateTime dateTime, int numberOfFiles, long size, string fileName)
            : base(fileName)
        {
            this.client = client;
            this.Name = name;
            this.BuildLabel = buildLabel;
            this.DateTime = dateTime;
            this.NumberOfFiles = numberOfFiles;
            this.Size = size;
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
        #endregion

        #region Internal methods
        #region Wrap()
        /// <summary>
        /// Wraps the specified package status.
        /// </summary>
        /// <param name="owningClient">The owning client.</param>
        /// <param name="packageStatus">The package status.</param>
        /// <param name="project">The project.</param>
        /// <returns>
        /// The new <see cref="CCPackage"/>.
        /// </returns>
        internal static CCPackage Wrap(CruiseServerClientBase owningClient, PackageDetails packageStatus, CCProject project)
        {
            var package = new CCPackage(
                owningClient,
                packageStatus.Name, 
                packageStatus.BuildLabel, 
                packageStatus.DateTime, 
                packageStatus.NumberOfFiles, 
                packageStatus.Size, 
                packageStatus.FileName) { Project = project };
            return package;
        }
        #endregion
        #endregion
    }
}
