// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildSummary.cs" company="The CruiseControl.NET Team">
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

namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A build summary.
    /// </summary>
    [XmlRoot("summary")]
    [Serializable]
    public class BuildSummary
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSummary"/> class.
        /// </summary>
        public BuildSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSummary"/> class.
        /// </summary>
        /// <param name="original">The original.</param>
        public BuildSummary(BuildSummary original)
        {
            this.Duration = original.Duration;
            this.Label = original.Label;
            this.StartTime = original.StartTime;
            this.Status = original.Status;
            this.LogName = original.LogName;
        }
        #endregion

        #region Public properties
        #region StartTime
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [XmlAttribute("start")]
        public DateTime StartTime { get; set; }
        #endregion

        #region Duration
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        [XmlAttribute("duration")]
        public long Duration { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [XmlAttribute("status")]
        public IntegrationStatus Status { get; set; }
        #endregion

        #region Label
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [XmlAttribute("label")]
        public string Label { get; set; }
        #endregion

        #region LogName
        /// <summary>
        /// Gets or sets the name of the log.
        /// </summary>
        /// <value>
        /// The name of the log.
        /// </value>
        [XmlAttribute("log")]
        public string LogName { get; set; }
        #endregion
        #endregion
    }
}
