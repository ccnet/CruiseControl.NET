using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace tw.ccnet.core
{
	[Serializable]
	public class CruiseControlException : ApplicationException
	{
		public CruiseControlException() : base("") {}
		public CruiseControlException(string s) : base(s) {}
		public CruiseControlException(string s, Exception e) : base(s, e) {}
		public CruiseControlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}