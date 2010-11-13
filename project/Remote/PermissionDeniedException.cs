
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    [Serializable]
    public class PermissionDeniedException
        : SecurityException
    {
        private const string permissionData = "PERMISSION_NAME";
        private string permission;

        public PermissionDeniedException(string permission)
            : this(permission, string.Format(System.Globalization.CultureInfo.CurrentCulture,"Permission to execute '{0}' has been denied.", permission), null)
        {
        }
        public PermissionDeniedException(string permission, string s) : this(permission, s, null) { }
        public PermissionDeniedException(string permission, string s, Exception e)
            : base(s, e)
        {
            this.permission = permission;
        }
        public PermissionDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.permission = info.GetString(permissionData);
        }

        public string Permission
        {
            get { return permission; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(permissionData, permission);
        }
    }
}
