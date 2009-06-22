using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	[Serializable]
	public class CCTrayLibException : ApplicationException
	{
		public CCTrayLibException() : base(string.Empty) {}
		public CCTrayLibException(string s) : base(s) {}
		public CCTrayLibException(string s, Exception e) : base(s, e) {}
		public CCTrayLibException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
