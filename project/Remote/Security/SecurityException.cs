using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    [Serializable]
    public class SecurityException
        : CruiseControlException
    {
		public SecurityException() : base("A security failure has occurred") {}
		public SecurityException(string s) : base(s) {}
		public SecurityException(string s, Exception e) : base(s, e) {}
        public SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
