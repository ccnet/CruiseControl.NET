// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListBuildSummaryResponse.cs" company="The CruiseControl.NET Team">
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

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The response containing build summaries.
    /// </summary>
    [XmlRoot("buildSummaryMessage")]
    [Serializable]
    public class ListBuildSummaryResponse
        : Response
    {
        #region Private fields
        /// <summary>
        /// The summaries.
        /// </summary>
        private readonly List<BuildSummary> summaries = new List<BuildSummary>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBuildSummaryResponse"/> class.
        /// </summary>
        public ListBuildSummaryResponse()
        {
            this.summaries = new List<BuildSummary>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBuildSummaryResponse"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public ListBuildSummaryResponse(ServerRequest request)
            : base(request)
        {
            this.summaries = new List<BuildSummary>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBuildSummaryResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="summaries">The summaries.</param>
        public ListBuildSummaryResponse(Response response, IList<BuildSummary> summaries)
            : base(response)
        {
            this.summaries = new List<BuildSummary>(summaries);
        }
        #endregion

        #region Public properties
        #region Summaries
        /// <summary>
        /// Gets the summaries.
        /// </summary>
        /// <value>
        /// The summaries.
        /// </value>
        [XmlElement("summaries")]
        public IList<BuildSummary> Summaries
        {
            get { return this.summaries; }
        }
        #endregion
        #endregion
    }
}
