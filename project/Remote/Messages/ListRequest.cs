// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListRequest.cs" company="The CruiseControl.NET Team">
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
    /// A list request message.
    /// </summary>
    [XmlRoot("listMessage")]
    [Serializable]
    public class ListRequest
        : ProjectRequest
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListRequest"/> class.
        /// </summary>
        public ListRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListRequest"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        public ListRequest(int start, int count)
        {
            this.Start = start;
            this.Count = count;
        }
        #endregion

        #region Public properties
        #region Credentials
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [XmlElement("count")]
        public int Count { get; set; }
        #endregion

        #region Start
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        [XmlElement("start")]
        public int Start { get; set; }
        #endregion
        #endregion
    }
}
