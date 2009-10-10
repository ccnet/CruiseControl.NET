using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Mapped functionality for an LDAP service
    /// </summary>
    public interface ILdapService
    {

        string DomainName { get; set; }


        /// <summary>
        /// Retrieves the information of the specified user
        /// </summary>
        /// <param name="userNameToRetrieveFrom"></param>
        /// <returns></returns>
        LdapUserInfo RetrieveUserInformation(string userNameToRetrieveFrom);


        /// <summary>
        /// Tries to authenticate the user to the specified LDAP service (domain)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        bool Authenticate(string userName, string password, string domainName);

    }
}
