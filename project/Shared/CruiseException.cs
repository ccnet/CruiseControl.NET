using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Shared
{
	[Serializable]
	public class CruiseException : ApplicationException
	{
		public CruiseException() : base("") {}
		public CruiseException(string s) : base(s) {}
		public CruiseException(string s, Exception e) : base(s, e) {}
		public CruiseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}