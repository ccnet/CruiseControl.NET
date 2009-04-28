using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote.Security;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    public abstract class PermissionBase
        : IConfigurationValidation
    {
        #region Private fields
        private string refId;
        private SecurityRight defaultRight = SecurityRight.Inherit;
        private SecurityRight sendMessage = SecurityRight.Inherit;
        private SecurityRight forceBuild = SecurityRight.Inherit;
        private SecurityRight startProject = SecurityRight.Inherit;
        private SecurityRight viewSecurity = SecurityRight.Inherit;
        private ISecurityManager manager;
        #endregion

        #region Public properties
        #region RefId
        /// <summary>
        /// The identifier of the referenced permission.
        /// </summary>
        [ReflectorProperty("ref", Required = false)]
        public string RefId
        {
            get { return refId; }
            set { refId = value; }
        }
        #endregion

        #region DefaultRight
        /// <summary>
        /// The default right to use.
        /// </summary>
        [ReflectorProperty("defaultRight", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight DefaultRight
        {
            get { return defaultRight; }
            set { defaultRight = value; }
        }
        #endregion

        #region SendMessageRight
        /// <summary>
        /// The right.
        /// </summary>
        [ReflectorProperty("sendMessage", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight SendMessageRight
        {
            get { return sendMessage; }
            set { sendMessage = value; }
        }
        #endregion

        #region ForceBuildRight
        /// <summary>
        /// The right.
        /// </summary>
        [ReflectorProperty("forceBuild", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight ForceBuildRight
        {
            get { return forceBuild; }
            set { forceBuild = value; }
        }
        #endregion

        #region StartProjectRight
        /// <summary>
        /// The right.
        /// </summary>
        [ReflectorProperty("startProject", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight StartProjectRight
        {
            get { return startProject; }
            set { startProject = value; }
        }
        #endregion

        #region ViewSecurityRight
        /// <summary>
        /// The right to view security.
        /// </summary>
        [ReflectorProperty("viewSecurity", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight ViewSecurityRight
        {
            get { return viewSecurity; }
            set { viewSecurity = value; }
        }
        #endregion

        #region Manager
        /// <summary>
        /// The security manager that loaded this setting.
        /// </summary>
        public ISecurityManager Manager
        {
            get { return manager; }
            set { manager = value; }
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
        public virtual bool CheckUser(ISecurityManager manager, string userName)
        {
            if (string.IsNullOrEmpty(refId))
            {
                return CheckUserActual(manager, userName);
            }
            else
            {
                IPermission refPermission = manager.RetrievePermission(refId);
                if (refPermission == null)
                {
                    throw new BadReferenceException(refId);
                }
                else
                {
                    return refPermission.CheckUser(manager, userName);
                }
            }
        }
        #endregion

        #region CheckPermission()
        /// <summary>
        /// Checks the result of this permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The security right.</returns>
        public virtual SecurityRight CheckPermission(ISecurityManager manager, SecurityPermission permission)
        {
            if (string.IsNullOrEmpty(refId))
            {
                return CheckPermissionActual(manager, permission);
            }
            else
            {
                IPermission refPermission = manager.RetrievePermission(refId);
                if (refPermission == null)
                {
                    throw new BadReferenceException(refId);
                }
                else
                {
                    return refPermission.CheckPermission(manager, permission);
                }
            }
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
            if (!string.IsNullOrEmpty(refId))
            {
                IPermission refPermission = configuration.SecurityManager.RetrievePermission(refId);
                if (refPermission == null)
                {
                    errorProcesser.ProcessError(new BadReferenceException(refId));
                }
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region CheckUserActual()
        protected abstract bool CheckUserActual(ISecurityManager manager, string userName);
        #endregion

        #region CheckPermissionActual()
        /// <summary>
        /// Checks the result of this permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>The security right.</returns>
        protected virtual SecurityRight CheckPermissionActual(ISecurityManager manager, SecurityPermission permission)
        {
            var result = SecurityRight.Inherit;
            switch (permission)
            {
                case SecurityPermission.ForceBuild:
                    result = forceBuild;
                    break;
                case SecurityPermission.SendMessage:
                    result = sendMessage;
                    break;
                case SecurityPermission.StartProject:
                    result = startProject;
                    break;
                case SecurityPermission.ViewSecurity:
                    result = viewSecurity;
                    break;
            }
            if (result == SecurityRight.Inherit) result = defaultRight;
            return result;
        }
        #endregion
        #endregion
    }
}
