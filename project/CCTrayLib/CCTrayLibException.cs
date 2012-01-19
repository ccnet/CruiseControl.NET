using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	[Serializable]
	public class CCTrayLibException : ApplicationException
	{
		public CCTrayLibException() : base(string.Empty) {}
		public CCTrayLibException(string message) : base(message) {}
		public CCTrayLibException(string message, Exception e) : base(message, e) {}
		public CCTrayLibException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
