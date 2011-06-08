// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewConnection.cs" company="The CruiseControl.NET Team">
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
    using System.Collections.Generic;
    using System.Management.Automation;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Starts a new connection to a server.
    /// </summary>
    [Cmdlet(VerbsCommon.New, Nouns.Connection, DefaultParameterSetName = CommonCmdlet.CommonParameterSet)]
    public class NewConnection
        : CommonCmdlet
    {
        #region Public properties
        #region Address
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = CommonCmdlet.CommonParameterSet)]
        [ValidateNotNullOrEmpty]
        public string Address { get; set; }
        #endregion

        #region UserName
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [Parameter]
        public string UserName { get; set; }
        #endregion

        #region Password
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Parameter]
        public string Password { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region ProcessRecord()
        /// <summary>
        /// Processes a record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.WriteVerbose("Starting a new server connection");
            var client = ClientHelpers.GenerateClient(this.Address, this);

            this.WriteVerbose("Retrieving server version");
            var version = client.GetServerVersion();

            if (!string.IsNullOrEmpty(this.UserName))
            {
                this.WriteVerbose("Logging in");
                var credentials = new List<NameValuePair>
                    { 
                        new NameValuePair(LoginRequest.UserNameCredential, this.UserName) 
                    };
                if (!string.IsNullOrEmpty(this.Password))
                {
                    credentials.Add(new NameValuePair(LoginRequest.PasswordCredential, this.Password));
                }

                client.Login(credentials);
            }

            var connection = new CCConnection(client, new Version(version));
            this.WriteObject(connection);
        }
        #endregion
        #endregion
    }
}
