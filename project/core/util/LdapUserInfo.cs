using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Information retrieved about a user from the LDAP service
    /// </summary>
    public class LdapUserInfo
    {
        /// <summary>
        ///  Contents of LdapFieldMailAddress
        /// </summary>
        public string MailAddress {get;set;}
        /// <summary>
        /// Contents of LdapFieldName
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// LdapFieldSurName
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// LdapFieldCommonName
        /// </summary>
        public string CommonName { get; set; }
        /// <summary>
        /// LdapFieldGivenName
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// LdapFieldDisplayName
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// LdapFieldMailNickName
        /// </summary>
        public string MailNickName { get; set; }

    }
}
