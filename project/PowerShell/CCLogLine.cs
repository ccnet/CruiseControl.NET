// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CCLogLine.cs" company="The CruiseControl.NET Team">
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
    /// Defines a line in a log.
    /// </summary>
    public class CCLogLine
    {
        #region Public properties
        #region EventTime
        /// <summary>
        /// Gets or sets the event time.
        /// </summary>
        /// <value>
        /// The event time.
        /// </value>
        public DateTime? EventTime { get; set; }
        #endregion

        #region Thread
        /// <summary>
        /// Gets or sets the thread.
        /// </summary>
        /// <value>
        /// The thread.
        /// </value>
        public string Thread { get; set; }
        #endregion

        #region Level
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public string Level { get; set; }
        #endregion

        #region Message
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Parse()
        /// <summary>
        /// Parses the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        /// The parsed line.
        /// </returns>
        public static CCLogLine Parse(string line)
        {
            var firstBlock = line.IndexOf('[');
            var secondBlock = line.IndexOf(']');
            if ((firstBlock < 0) || (secondBlock < 0))
            {
                // Cannot parse - just return the line
                return new CCLogLine { Message = line };
            }

            try
            {
                // Pull the line apart and parse everything
                var time = line.Substring(0, firstBlock - 1);
                var identity = line.Substring(firstBlock + 1, secondBlock - firstBlock - 1);
                var identityParts = identity.Split(':');
                var logLine = new CCLogLine
                    {
                        Thread = identityParts[0].Trim(),
                        Level = identityParts[1].Trim(),
                        Message = line.Substring(secondBlock + 1).Trim()
                    };
                try
                {
                    var commaPos = time.IndexOf(',');
                    if (commaPos > 0)
                    {
                        time = time.Substring(0, commaPos);
                    }

                    logLine.EventTime = DateTime.Parse(time.Trim());
                }
                catch (FormatException)
                {
                    // Unable to parse the date/time :-|
                }

                return logLine;
            }
            catch
            {
                // If all else fails just return the line
                return new CCLogLine { Message = line };
            }
        }
        #endregion
        #endregion
    }
}
