using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using ThoughtWorks.CruiseControl.Shared;

namespace ThoughtWorks.CruiseControl.Shared
{
	[Serializable]
	public class CruiseCommandException : CruiseException
	{
		public CruiseCommandException() : base("") {}
		public CruiseCommandException(string s) : base(s) {}
		public CruiseCommandException(string s, Exception e) : base(s, e) {}
		public CruiseCommandException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}