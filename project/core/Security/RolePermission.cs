using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines the permissions for a role (a group of users).
    /// </summary>
    /// <title>Role Permission</title>
    /// <version>1.5</version>
    /// <remarks>
    /// <includePage>General Security Permissions</includePage>
    /// </remarks>
    /// <example>
    /// <code title="Role Definition Example">
    /// &lt;rolePermission name="admin" forceBuild="Allow" startProject="Deny" defaultRight="Inherit"&gt;
    /// &lt;users&gt;
    /// &lt;userName name="johndoe"/&gt;
    /// &lt;/users&gt;
    /// &lt;/rolePermission&gt;
    /// </code>
    /// <code title="Reference Example">
    /// &lt;rolePermission name="admin" ref="admin"/&gt;
    /// </code>
    /// </example>
    [ReflectorType("rolePermission")]
    public class RolePermission
        : PermissionBase, IPermission
    {
        #region Private fields
        private string roleName;
        private UserName[] users = new UserName[0];
        #endregion

        #region Constructors
        /// <summary>
        /// Start a new blank instance.
        /// </summary>
        public RolePermission() { }

        /// <summary>
        /// Start a fully load instance.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="defaultRight">The default right.</param>
        /// <param name="sendMessage">Their send message right.</param>
        /// <param name="forceBuild">Their force build right.</param>
        /// <param name="startProject">Their start project right.</param>
        /// <param name="users">The users in this role.</param>
        public RolePermission(string roleName, SecurityRight defaultRight, SecurityRight sendMessage, SecurityRight forceBuild, SecurityRight startProject, params UserName[] users)
        {
            this.roleName = roleName;
            base.DefaultRight = defaultRight;
            base.SendMessageRight = sendMessage;
            base.ForceBuildRight = forceBuild;
            base.StartProjectRight = startProject;
            this.users = users;
        }
        #endregion

        #region Public properties
        #region Identifier
        /// <summary>
        /// A unique identifier for this item.
        /// </summary>
        public string Identifier
        {
            get { return roleName; }
        }
        #endregion

        #region RoleName
        /// <summary>
        /// The name of the role.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("name", Required = true)]
        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }
        #endregion

        #region Users
        /// <summary>
        /// The users in this role.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorArray("users", Required=false)]
        public UserName[] Users
        {
            get { return users; }
            set { users = value; }
        }
        #endregion
        #endregion

        #region Protected methods
        #region CheckUserActual()
        /// <summary>
        /// Checks if the user should use this permission.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <returns>True if the permission is valid for the user, false otherwise.</returns>
        protected override bool CheckUserActual(ISecurityManager manager, string userName)
        {
            bool userFound = false;
            foreach (UserName user in users)
            {
                if (string.Equals(userName, user.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    userFound = true;
                    break;
                }
            }
            return userFound;
        }
        #endregion
        #endregion
    }
}
