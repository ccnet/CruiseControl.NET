using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
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

        #region RequiresSession
        /// <summary>
        /// Does this authorisation require a valid session?
        /// </summary>
        public bool RequiresSession
        {
            get { return true; }
        }
        #endregion

        #region DefaultRight
        /// <summary>
        /// The default right to use.
        /// </summary>
        [ReflectorProperty("defaultRight", Required=false)]
        public SecurityRight DefaultRight
        {
            get { return defaultRight; }
            set
            {
                if (value == SecurityRight.Inherit) throw new ArgumentOutOfRangeException("DefaultRight", "DefaultRight must be either Allow or Deny");
                defaultRight = value;
            }
        }
        #endregion

        #region Permissions
        /// <summary>
        /// The allowed permissions.
        /// </summary>
        [ReflectorProperty("permissions", Required = false)]
        public IPermission[] Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region CheckPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public virtual bool CheckPermission(ISecurityManager manager, string userName, SecurityPermission permission)
        {
            SecurityRight currentRight = SecurityRight.Inherit;

            // Iterate through the assertions stopping when we hit the first non-inherited permission
            foreach (IPermission assertion in permissions)
            {
                if (assertion.CheckUser(manager, userName)) currentRight = assertion.CheckPermission(manager, permission);
                if (currentRight != SecurityRight.Inherit) break;
            }

            // If we don't have a result, then use the default right
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
