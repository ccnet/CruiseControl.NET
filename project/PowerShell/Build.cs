// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="The CruiseControl.NET Team">
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
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// The details on a build.
    /// </summary>
    public class Build
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
        /// Initializes a new instance of the <see cref="Build"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="summary">The summary.</param>
        public Build(CruiseServerClientBase client, BuildSummary summary)
            : base(summary)
        {
            this.client = client;    
        }
        #endregion
    }
}
