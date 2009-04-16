using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    [ReflectorType("userPermission")]
    public class UserPermission
        : PermissionBase, IPermission
    {
        #region Private fields
        private string userName;
        #endregion

        #region Constructors
        /// <summary>
        /// Start a new blank instance.
        /// </summary>
        public UserPermission() { }

        /// <summary>
        /// Start a fully load instance.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="defaultRight">The default right.</param>
        /// <param name="sendMessage">Their send message right.</param>
        /// <param name="forceBuild">Their force build right.</param>
        /// <param name="startProject">Their start project right.</param>
        /// <param name="stopProject">Their stop project right.</param>
        public UserPermission(string userName, SecurityRight defaultRight, SecurityRight sendMessage, SecurityRight forceBuild, SecurityRight startProject, SecurityRight stopProject)
        {
            this.userName = userName;
            base.DefaultRight = defaultRight;
            base.SendMessageRight = sendMessage;
            base.ForceBuildRight = forceBuild;
            base.StartProjectRight = startProject;
            base.StopProjectRight = stopProject;
        }
        #endregion

        #region Public properties
        #region Identifier
        /// <summary>
        /// A unique identifier for this item.
        /// </summary>
        public string Identifier
        {
            get { return userName; }
        }
        #endregion

        #region UserName
        /// <summary>
        /// The user name.
        /// </summary>
        [ReflectorProperty("name", Required = true)]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region CheckUser()
        /// <summary>
        /// Checks if the user should use this permission.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <returns>True if the permission is valid for the user, false otherwise.</returns>
        protected override bool CheckUserActual(ISecurityManager manager, string userName)
        {
            return string.Equals(userName, this.userName, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
        #endregion
    }
}
