// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientHelpers.cs" company="The CruiseControl.NET Team">
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
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Helper utilities for working with clients.
    /// </summary>
    public static class ClientHelpers
    {
        #region Public methods
        #region GenerateClient()
        /// <summary>
        /// Generates a client connection.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="common">The common parameters.</param>
        /// <returns>
        /// The client connection.
        /// </returns>
        public static CruiseServerClientBase GenerateClient(string address, CommonCmdlet common)
        {
            // Build up the address
            var actualAddress = address;
            if (!actualAddress.Contains("//"))
            {
                // Address does not contain the protocol
                if (actualAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ||
                    actualAddress.Equals("127.0.0.1", StringComparison.InvariantCultureIgnoreCase))
                {
                    actualAddress = "tcp://" + actualAddress;
                    if (!actualAddress.Contains(":"))
                    {
                        // Add the default port
                        actualAddress += ":21234";
                    }
                }
                else
                {
                    actualAddress = "http://" + actualAddress;
                }
            }

            // Generate the client
            var clientFactory = new CruiseServerClientFactory();
            var settings = new ClientStartUpSettings
                {
                    UseEncryption = common.Encrypted,
                    BackwardsCompatable = common.BackwardsCompatable
                };
            var client = clientFactory.GenerateClient(actualAddress, settings);
            client.TargetServer = common.Target;
            return client;
        }
        #endregion
        #endregion
    }
}
