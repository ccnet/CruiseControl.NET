#pragma warning disable 1591
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    [Serializable]
    public class SessionInvalidException
        : SecurityException
    {
		public SessionInvalidException() : base("The session token is either invalid or is for a session that has expired.") {}
		public SessionInvalidException(string s) : base(s) {}
		public SessionInvalidException(string s, Exception e) : base(s, e) {}
        public SessionInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
