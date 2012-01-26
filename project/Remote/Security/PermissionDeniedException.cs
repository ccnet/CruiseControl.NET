
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    [Serializable]
    public class PermissionDeniedException
        : SecurityException
    {
        private const string permissionData = "PERMISSION_NAME";
        private string permission;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionDeniedException" /> class.	
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <remarks></remarks>
        public PermissionDeniedException(string permission)
            : this(permission, string.Format(System.Globalization.CultureInfo.CurrentCulture,"Permission to execute '{0}' has been denied.", permission), null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionDeniedException" /> class.	
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public PermissionDeniedException(string permission, string message) : this(permission, message, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionDeniedException" /> class.	
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public PermissionDeniedException(string permission, string message, Exception e)
            : base(message, e)
        {
            this.permission = permission;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionDeniedException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public PermissionDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.permission = info.GetString(permissionData);
        }

        /// <summary>
        /// Gets the permission.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Permission
        {
            get { return permission; }
        }

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(permissionData, permission);
        }
    }
}
