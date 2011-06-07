// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionCmdlet.cs" company="The CruiseControl.NET Team">
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
    /// Provides common functionality for cmdlets that require a connection.
    /// </summary>
    public abstract class ConnectionCmdlet
        : CommonCmdlet
    {
        #region Public constants
        /// <summary>
        /// The name of the common parameter set.
        /// </summary>
        public const string ConnectionParameterSet = "ConnectionParameterSet";
        #endregion

        #region Public properties
        #region Address
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = CommonCmdlet.CommonParameterSet)]
        [ValidateNotNullOrEmpty]
        public string Address { get; set; }
        #endregion

        #region Connection
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ConnectionCmdlet.ConnectionParameterSet)]
        [ValidateNotNull]
        public CCConnection Connection { get; set; }
        #endregion
        #endregion
    }
}
