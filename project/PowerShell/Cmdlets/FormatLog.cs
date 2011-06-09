// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLog.cs" company="The CruiseControl.NET Team">
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
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    /// <summary>
    /// Formats a log.
    /// </summary>
    [Cmdlet(VerbsCommon.Format, Nouns.Log, DefaultParameterSetName = CommonCmdlet.CommonParameterSet)]
    public class FormatLog
        : PSCmdlet
    {
        #region Public properties
        #region FormatFile
        /// <summary>
        /// Gets or sets the format file.
        /// </summary>
        /// <value>
        /// The format file.
        /// </value>
        [Parameter(Position = 1, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string[] FormatFile { get; set; }
        #endregion

        #region Log
        /// <summary>
        /// Gets or sets the log data.
        /// </summary>
        /// <value>
        /// The log data.
        /// </value>
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Log { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Formatting log");
            using (var xmlReader = new StringReader(this.Log))
            {
                var xpath = new XPathDocument(xmlReader);
                using (var writer = new StringWriter())
                {
                    var transform = new XslCompiledTransform();
                    Func<string, string> selector = file => !Path.IsPathRooted(file) ? Path.Combine(Environment.CurrentDirectory, file) : file;
                    foreach (var fileToLoad in this.FormatFile.Select(selector))
                    {
                        this.WriteVerbose("Loading format file " + fileToLoad);
                        using (var stream = File.OpenRead(fileToLoad))
                        {
                            using (var reader = XmlReader.Create(stream))
                            {
                                transform.Load(reader);
                                transform.Transform(xpath, null, writer);
                            }
                        }
                    }

                    this.WriteObject(writer.GetStringBuilder().ToString(), false);
                }
            }
        }
        #endregion
        #endregion
    }
}
