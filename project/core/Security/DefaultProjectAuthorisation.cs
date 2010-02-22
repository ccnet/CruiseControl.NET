using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// The project-specific settings include a list of permissions. These permissions associate the users, roles, etc. from the server-level
    /// security with the actions.
    /// </summary>
    /// <version>1.5</version>
    /// <title>Default Project Security</title>
    /// <remarks>
    /// <includePage>General Security Permissions</includePage>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;security type="defaultProjectSecurity"&gt;
    /// &lt;permissions&gt;
    /// &lt;rolePermission name="admin" ref="admin" /&gt;
    /// &lt;userPermission name="johndoe" forceBuild="Allow" startProject="Deny" stopProject="Deny"/&gt;
    /// &lt;/permissions&gt;
    /// &lt;/security&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of the project security.</description>
    /// <value>defaultProjectSecurity</value>
    /// </key>
    [ReflectorType("defaultProjectSecurity")]
    public class DefaultProjectAuthorisation
        : IProjectAuthorisation, IConfigurationValidation
    {
        #region Private fields
        private SecurityRight defaultRight = SecurityRight.Inherit;
        private IPermission[] permissions = new IPermission[0];
        #endregion

        #region Constructors
        /// <summary>
        /// Start a new blank instance.
        /// </summary>
        public DefaultProjectAuthorisation() { }

        /// <summary>
        /// Start a fully load instance.
        /// </summary>
        /// <param name="defaultRight">The default right.</param>
        /// <param name="assertions">The assertions.</param>
        public DefaultProjectAuthorisation(SecurityRight defaultRight, params IPermission[] assertions)
        {
            this.defaultRight = defaultRight;
            this.permissions = assertions;
        }
        #endregion

        #region Public properties
        #region DefaultRight
        /// <summary>
        /// The default right to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Inherit</default>
        [ReflectorProperty("defaultRight", Required = false)]
        public SecurityRight DefaultRight
        {
            get { return defaultRight; }
            set { defaultRight = value; }
        }
        #endregion

        #region Permissions
        /// <summary>
        /// The allowed permissions.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("permissions", Required = false)]
        public IPermission[] Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
        #endregion

        #region RequiresServerSecurity
        /// <summary>
        /// Does this authorisation require security to be configured on the server?
        /// </summary>
        public bool RequiresServerSecurity
        {
            get { return true; }
        }
        #endregion

        #region GuestAccountName
        /// <summary>
        /// The name of the account to use for guests.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("guest", Required = false)]
        public string GuestAccountName { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region RequiresSession()
        /// <summary>
        /// Does this authorisation require a valid session?
        /// </summary>
        public bool RequiresSession(ISecurityManager manager)
        {
            return true;
        }
        #endregion

        #region CheckPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <param name="defaultRight">The default right to use.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public virtual bool CheckPermission(ISecurityManager manager, 
            string userName,
            SecurityPermission permission,
            SecurityRight defaultRight)
        {
            SecurityRight currentRight = SecurityRight.Inherit;

            // Iterate through the assertions stopping when we hit the first non-inherited permission
            foreach (IPermission assertion in permissions)
            {
                if (assertion.CheckUser(manager, userName)) currentRight = assertion.CheckPermission(manager, permission);
                if (currentRight != SecurityRight.Inherit) break;
            }

            // If we don't have a result, then use the default right
            if (currentRight == SecurityRight.Inherit) currentRight = this.defaultRight;
            if (currentRight == SecurityRight.Inherit) currentRight = defaultRight;
            return (currentRight == SecurityRight.Allow);
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        public virtual void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            foreach (IPermission permission in permissions)
            {
                if (permission is IConfigurationValidation)
                {
                    (permission as IConfigurationValidation).Validate(configuration, parent, errorProcesser);
                }
            }
        }
        #endregion
        #endregion
    }
}
