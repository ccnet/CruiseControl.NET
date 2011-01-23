using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Inherit the security settings for a project from the server settings.
    /// </summary>
    /// <title>Inherited Project Security</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;security type="inheritedProjectSecurity" /&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of the project security.</description>
    /// <value>inheritedProjectSecurity</value>
    /// </key>
    [ReflectorType("inheritedProjectSecurity")]
    public class InheritedProjectAuthorisation
        : IProjectAuthorisation
    {
        #region Constructors
        /// <summary>
        /// Start a new blank instance.
        /// </summary>
        public InheritedProjectAuthorisation() { }
        #endregion

        #region Public properties
        #region RequiresServerSecurity
        /// <summary>
        /// Does this authorisation require security to be configured on the server?
        /// </summary>
        public bool RequiresServerSecurity
        {
            get { return false; }
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
            return manager.RequiresSession;
        }
        #endregion

        #region CheckPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <param name="defaultRight">The default right to use.</param>
        /// <param name="manager"></param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public virtual bool CheckPermission(ISecurityManager manager,
            string userName,
            SecurityPermission permission,
            SecurityRight defaultRight)
        {
            return manager.CheckServerPermission(userName, permission);
        }
        #endregion
        #endregion
    }
}
