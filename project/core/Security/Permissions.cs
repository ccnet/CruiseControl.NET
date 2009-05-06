using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Security;
using Exortech.NetReflector;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines a set of permissions.
    /// </summary>
    [ReflectorType("Permissions")]
    public class Permissions
    {
        #region Private fields
        private SecurityRight defaultRight = SecurityRight.Inherit;
        private SecurityRight sendMessage = SecurityRight.Inherit;
        private SecurityRight forceAbortBuild = SecurityRight.Inherit;
        private SecurityRight startStopProject = SecurityRight.Inherit;
        private SecurityRight changeProjectConfiguration = SecurityRight.Inherit;
        private SecurityRight viewSecurity = SecurityRight.Inherit;
        private SecurityRight modifySecurity = SecurityRight.Inherit;
        private SecurityRight viewProject = SecurityRight.Inherit;
        #endregion

        #region Public properties
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
            get { return forceAbortBuild; }
            set { forceAbortBuild = value; }
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
            get { return startStopProject; }
            set { startStopProject = value; }
        }
        #endregion

        #region ChangeProjectRight
        /// <summary>
        /// The right.
        /// </summary>
        [ReflectorProperty("changeProject", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight ChangeProjectRight
        {
            get { return changeProjectConfiguration; }
            set { changeProjectConfiguration = value; }
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

        #region ModifySecurityRight
        /// <summary>
        /// The right to view security.
        /// </summary>
        [ReflectorProperty("modifySecurity", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight ModifySecurityRight
        {
            get { return modifySecurity; }
            set { modifySecurity = value; }
        }
        #endregion

        #region ViewProjectRight
        /// <summary>
        /// The right to view a project.
        /// </summary>
        [ReflectorProperty("viewProject", Required = false)]
        [DefaultValue(SecurityRight.Inherit)]
        public SecurityRight ViewProjectRight
        {
            get { return viewProject; }
            set { viewProject = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetPermission()
        /// <summary>
        /// Retrieves the actual permission.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public SecurityRight GetPermission(SecurityPermission permission)
        {
            switch (permission)
            {
                case SecurityPermission.ViewProject:
                    return ViewProjectRight;
                case SecurityPermission.ForceAbortBuild:
                    return ForceBuildRight;
                case SecurityPermission.SendMessage:
                    return SendMessageRight;
                case SecurityPermission.StartStopProject:
                    return StartProjectRight;
                case SecurityPermission.ChangeProjectConfiguration:
                    return ChangeProjectRight;
                case SecurityPermission.ViewSecurity:
                    return ViewSecurityRight;
                case SecurityPermission.ModifySecurity:
                    return ModifySecurityRight;
                default:
                    return DefaultRight;
            }
        }
        #endregion
        #endregion
    }
}
