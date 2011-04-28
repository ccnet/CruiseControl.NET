// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnapIn.cs" company="The CruiseControl.NET Team">
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
    using System.ComponentModel;
    using System.Management.Automation;

    /// <summary>
    /// Registers the snap-in with PowerShell.
    /// </summary>
    [RunInstaller(true)]
    public class SnapIn
        : PSSnapIn
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get { return "CruiseControl.Net.Client"; }
        }
        #endregion

        #region Vendor
        /// <summary>
        /// Gets the vendor.
        /// </summary>
        public override string Vendor
        {
            get { return "The CruiseControl.NET team"; }
        }
        #endregion

        #region Description
        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get { return "Manage CruiseControl.NET servers."; }
        }
        #endregion
        #endregion
    }
}
