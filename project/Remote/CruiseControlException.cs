using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Base cruiseControl specific exception, inheriting from <see cref="System.ApplicationException"/>.
    /// </summary>
	[Serializable]
	public class CruiseControlException : ApplicationException
	{
		public CruiseControlException() : base(string.Empty) {}
		public CruiseControlException(string s) : base(s) {}
		public CruiseControlException(string s, Exception e) : base(s, e) {}
		public CruiseControlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}