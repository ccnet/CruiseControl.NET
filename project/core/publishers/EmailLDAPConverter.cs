using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("LDAPConverter")]
    public class EmailLDAPConverter : IEmailConverter
    {
        private string domainName = "";
        private string ldap_Mail = "mail";
        private string ldap_QueryField = "MailNickName";
        private string ldap_LogOnUser = "";
        private string ldap_LogOnPassword = "";


        /// <summary>
        /// The domain to query for the LDAP service
        /// </summary>
        [ReflectorProperty("domainName", Required = true)]
        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        /// <summary>
        /// The field in the LDAP service to use for mapping the source control userid 
        /// Defaults to MailNickName
        /// </summary>
        [ReflectorProperty("ldapQueryField", Required = false)]
        public string LdapQueryField
        {
            get { return ldap_QueryField; }
            set { ldap_QueryField = value; }
        }



        /// <summary>
        /// user for loggin into the ldap service
        /// </summary>
        [ReflectorProperty("ldapLogOnUser", Required = false)]
        public string LdapLogOnUser
        {
            get { return ldap_LogOnUser; }
            set { ldap_LogOnUser = value; }
        }


        [ReflectorProperty("ldapLogOnPassword", Required = false)]
        public string LdapLogOnPassword
        {
            get { return ldap_LogOnPassword; }
            set { ldap_LogOnPassword = value; }
        }



        public EmailLDAPConverter()
        {

        }


        /// <summary>
        /// Apply the conversion from username to email address.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The email address.</returns>
        public string Convert(string username)
        {           
            string LDAPPath = @"LDAP://" + domainName;
            string LDAPFilter = @"(&(objectClass=user)(SAMAccountName=" + username + "))";
            string[] LDAPProperties = { ldap_Mail, ldap_QueryField };

            System.DirectoryServices.DirectoryEntry domain;
            if (ldap_LogOnUser.Length > 0 )
            {
                domain = new System.DirectoryServices.DirectoryEntry(LDAPPath,ldap_LogOnUser,ldap_LogOnPassword);
            }
            else
            {
                domain = new System.DirectoryServices.DirectoryEntry(LDAPPath);
            }
            

            System.DirectoryServices.DirectorySearcher searcher = new System.DirectoryServices.DirectorySearcher(domain);
            System.DirectoryServices.SearchResult result;

            searcher.Filter = LDAPFilter;
            searcher.PropertiesToLoad.AddRange(LDAPProperties);
            
            result = searcher.FindOne();

            searcher.Dispose();

            // Check the result
            if (result != null)
            {
                return result.Properties[ldap_Mail][0].ToString();
            }
            else
            {
                Core.Util.Log.Debug(string.Format("No email adress found for user {0} in domain {1}",username,domainName));
                return null;
            }
        }
    }
}
