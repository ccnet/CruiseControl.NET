// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerFolder.cs" company="The CruiseControl.NET Team">
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

    /// <summary>
    /// A root level folder for a server.
    /// </summary>
    public class ServerFolder
        : ClientFolder
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerFolder"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="version">The version.</param>
        public ServerFolder(string path, Version version)
            : base(path)
        {
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
        #endregion
    }
}
