//-----------------------------------------------------------------------
// <copyright file="Impersonation.cs" company="DockOfTheBay">
//     http://www.dotbay.be
// </copyright>
// <summary>Defines the Impersonation class.</summary>
//-----------------------------------------------------------------------
// Class copied from http://dotbay.blogspot.com/2009/05/windows-impersonation-in-c.html

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Facilitates impersonation of a Windows User.
    /// </summary>
    public class Impersonation
        : IDisposable
    {
        /// <summary>
        /// Windows Token.
        /// </summary>
        private IntPtr tokenHandle = new IntPtr(0);
        
        /// <summary>
        /// The impersonated User.
        /// </summary>
        private WindowsImpersonationContext impersonatedUser;
        
        /// <summary>
        /// Initializes a new instance of the Impersonation class.
        /// </summary>
        /// <param name="domainName">Domain name of the impersonated user.</param>
        /// <param name="userName">Name of the impersonated user.</param>
        /// <param name="password">Password of the impersonated user.</param>
        /// <remarks>
        /// Uses the unmanaged LogonUser function to get the user token for
        /// the specified user, domain, and password.
        /// </remarks>
        public Impersonation(string domainName, string userName, string password)
        {
            // Use the standard logon provider.
            const int logoN32ProviderDefault = 0;
            
            // Create a primary token.
            const int logoN32LogonInteractive = 2;
            
            this.tokenHandle = IntPtr.Zero;
            
            // Call LogonUser to obtain a handle to an access token.
            bool returnValue = LogonUser(
                                userName,
                                domainName,
                                password,
                                logoN32LogonInteractive,
                                logoN32ProviderDefault,
                                ref this.tokenHandle);
            
            if (false == returnValue)
            {
                // Something went wrong.
                int ret = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception(ret);
            }
        }

        /// <summary>
        /// Starts the impersonation.
        /// </summary>
        public void Impersonate()
        {
            // Create Identity.
            WindowsIdentity newId = new WindowsIdentity(this.tokenHandle);
            
            // Start impersonating.
            this.impersonatedUser = newId.Impersonate();
        }
        
        /// <summary>
        /// Stops the impersonation and releases security token.
        /// </summary>
        public void Revert()
        {
            // Stop impersonating.
            if (this.impersonatedUser != null)
            {
                this.impersonatedUser.Undo();
            }

            // Release the token.
            if (this.tokenHandle != IntPtr.Zero)
            {
                CloseHandle(this.tokenHandle);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
                string lpszUsername,
                string lpszDomain,
                string lpszPassword,
                int dwLogonType,
                int dwLogonProvider,
                ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        // Make sure revert has been called.
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources	
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            Revert();
        }
    }
}