using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core.Config;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines a security manager implementation that implements security internally - the security settings are stored in the same 
    /// configuration file.
    /// </summary>
    /// <title>Internal Server Security</title>
    /// <version>1.5</version>
    /// <remarks>
    /// <includePage>General Security Permissions</includePage>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;internalSecurity&gt;
    /// &lt;cache type="inMemoryCache" duration="10" mode="sliding"/&gt;
    /// &lt;users&gt;
    /// &lt;passwordUser name="johndoe" password="letmein"/&gt;
    /// &lt;simpleUser name="*"/&gt;
    /// &lt;/users&gt;
    /// &lt;permissions&gt;
    /// &lt;rolePermission name="general" forceBuild="Deny"&gt;
    /// &lt;users&gt;
    /// &lt;userName name="*"/&gt;
    /// &lt;/users&gt;
    /// &lt;/rolePermission&gt;
    /// &lt;/permissions&gt;
    /// &lt;/internalSecurity&gt;
    /// </code>
    /// </example>
    [ReflectorType("internalSecurity")]
    public class InternalSecurityManager
        : SecurityManagerBase, IConfigurationValidation
    {
        #region Private fields
        private IAuthentication[] users;
        private Dictionary<string, IAuthentication> loadedUsers;
        private List<IAuthentication> wildCardUsers;
        private IPermission[] permissions;
        private Dictionary<string, IPermission> loadedPermissions;
        private bool isInitialised = false;
        #endregion

        #region Public properties
        #region Users
        /// <summary>
        /// The users for the same.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("users")]
        public IAuthentication[] Users
        {
            get { return users; }
            set { users = value; }
        }
        #endregion

        #region Permissions
        /// <summary>
        /// The server-level permissions.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("permissions")]
        public IPermission[] Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialise the security manager.
        /// </summary>
        public override void Initialise()
        {
            SessionCache.Initialise();
            loadedUsers = new Dictionary<string, IAuthentication>();
            wildCardUsers = new List<IAuthentication>();
            if (users != null)
            {
                foreach (IAuthentication user in users)
                {
                    user.Manager = this;
                    if (user.Identifier.Contains("*"))
                    {
                        wildCardUsers.Add(user);
                    }
                    else
                    {
                        loadedUsers.Add(user.Identifier.ToLower(CultureInfo.InvariantCulture), user);
                    }
                }
            }

            loadedPermissions = new Dictionary<string, IPermission>();
            if (permissions != null)
            {
                foreach (IPermission permission in permissions)
                {
                    permission.Manager = this;
                    loadedPermissions.Add(permission.Identifier.ToLower(CultureInfo.InvariantCulture), permission);
                }
            }
            isInitialised = true;
        }
        #endregion

        #region RetrieveUser()
        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override IAuthentication RetrieveUser(string identifier)
        {
            IAuthentication setting = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                // If initialised then use the loaded dictionaries
                if (isInitialised)
                {
                    identifier = identifier.ToLower(CultureInfo.InvariantCulture);

                    if ((setting == null) && (loadedUsers.ContainsKey(identifier)))
                    {
                        setting = loadedUsers[identifier];
                    }
                    
                    if (setting == null)
                    {
                        // Attempt to find a matching wild-card
                        foreach (IAuthentication wildCard in wildCardUsers)
                        {
                            if (SecurityHelpers.IsWildCardMatch(wildCard.Identifier, identifier))
                            {
                                setting = wildCard;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (setting == null)
                    {
                        foreach (IAuthentication securitySetting in users)
                        {
                            if (securitySetting.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
                            {
                                setting = securitySetting;
                                break;
                            }
                            else if (securitySetting.Identifier.Contains("*"))
                            {
                                if (SecurityHelpers.IsWildCardMatch(securitySetting.Identifier, identifier))
                                {
                                    setting = securitySetting;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return setting;
        }
        #endregion

        #region RetrievePermission()
        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override IPermission RetrievePermission(string identifier)
        {
            IPermission setting = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                // If initialised then use the loaded dictionaries
                if (isInitialised)
                {
                    identifier = identifier.ToLower(CultureInfo.InvariantCulture);

                    if (loadedPermissions.ContainsKey(identifier))
                    {
                        setting = loadedPermissions[identifier];
                    }
                }
                else
                {
                    // Otherwise iterate through each and every item
                    foreach (IPermission securitySetting in permissions)
                    {
                        if (securitySetting.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
                        {
                            setting = securitySetting;
                            break;
                        }
                    }
                }
            }
            return setting;
        }
        #endregion

        #region ListAllUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public override List<UserDetails> ListAllUsers()
        {
            List<UserDetails> usersList = new List<UserDetails>();
            foreach (IAuthentication userDetails in users)
            {
                // Generate the details to return
                UserDetails user = new UserDetails();
                user.UserName = userDetails.UserName;
                user.DisplayName = userDetails.DisplayName;
                user.Type = userDetails.AuthenticationName;
                usersList.Add(user);
            }
            return usersList;
        }
        #endregion

        #region CheckServerPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action at the server level.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public override bool CheckServerPermission(string userName, SecurityPermission permission)
        {
            SecurityRight currentRight = SecurityRight.Inherit;

            // Iterate through the permissions stopping when we hit the first non-inherited permission
            foreach (IPermission permissionToCheck in permissions)
            {
                if (permissionToCheck.CheckUser(this, userName)) currentRight = permissionToCheck.CheckPermission(this, permission);
                if (currentRight != SecurityRight.Inherit) break;
            }

            // If we don't have a result, then use the default right
            if (currentRight == SecurityRight.Inherit) currentRight = GetDefaultRight(permission);
            return (currentRight == SecurityRight.Allow);
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser"></param>
        public virtual void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            foreach (IAuthentication user in users)
            {
                var dummy = user as IConfigurationValidation;
                if (dummy != null)
                {
                    dummy.Validate(configuration, parent.Wrap(this), errorProcesser);
                }
            }

            foreach (IPermission permission in permissions)
            {
                var dummy = permission as IConfigurationValidation;
                if (dummy != null)
                {
                    dummy.Validate(configuration, parent.Wrap(this), errorProcesser);
                }
            }
        }
        #endregion
        #endregion
    }
}
