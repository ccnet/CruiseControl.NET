using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines the details to use for impersonating another user account.
    /// </summary>
    /// <title>Impersonation</title>
    /// <version>1.5</version>
    [ReflectorType("impersonation")]
    public class ImpersonationDetails
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="ImpersonationDetails"/>.
        /// </summary>
        public ImpersonationDetails()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ImpersonationDetails"/>.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public ImpersonationDetails(string domainName, string userName, string password)
        {
            DomainName = domainName;
            UserName = userName;
            Password = password;
        }
        #endregion

        #region Public properties
        #region DomainName
        /// <summary>
        /// The name of the domain to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("domain")]
        public string DomainName { get; set; }
        #endregion

        #region UserName
        /// <summary>
        /// The name of the user to impersonate.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("user")]
        public string UserName { get; set; }
        #endregion

        #region Password
        /// <summary>
        /// The password of the user.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("password")]
        public string Password { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Impersonate()
        /// <summary>
        /// Start impersonating the other account.
        /// </summary>
        /// <returns></returns>
        public IDisposable Impersonate()
        {
            var impersonation = new Impersonation(DomainName, UserName, Password);
            impersonation.Impersonate();
            return impersonation;
        }
        #endregion
        #endregion
    }
}
