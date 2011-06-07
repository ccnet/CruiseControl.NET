// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonCmdlet.cs" company="The CruiseControl.NET Team">
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
    /// Exposes common parameters for cmdlets.
    /// </summary>
    public abstract class CommonCmdlet
        : PSCmdlet
    {
        #region Public constants
        /// <summary>
        /// The name of the common parameter set.
        /// </summary>
        public const string CommonParameterSet = "CommonParameterSet";
        #endregion

        #region Public properties
        #region Tagret
        /// <summary>
        /// Gets or sets the target server.
        /// </summary>
        /// <value>
        /// The target server.
        /// </value>
        [Parameter(ParameterSetName = CommonCmdlet.CommonParameterSet)]
        public string Target { get; set; }
        #endregion

        #region BackwardsCompatable
        /// <summary>
        /// Gets or sets a value indicating whether the connection is backwards compatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if backwards compatable; otherwise, <c>false</c>.
        /// </value>
        [Parameter(ParameterSetName = CommonCmdlet.CommonParameterSet)]
        public bool BackwardsCompatable { get; set; }
        #endregion

        #region Encrypted
        /// <summary>
        /// Gets or sets a value indicating whether the connection is encrypted.
        /// </summary>
        /// <value>
        /// <c>true</c> if encrypted; otherwise, <c>false</c>.
        /// </value>
        [Parameter(ParameterSetName = CommonCmdlet.CommonParameterSet)]
        public bool Encrypted { get; set; }
        #endregion
        #endregion
    }
}
