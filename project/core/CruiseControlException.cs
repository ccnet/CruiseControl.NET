using System;
using System.Runtime.Serialization;

namespace tw.ccnet.core
{
	public class CruiseControlException : ApplicationException
	{
		public CruiseControlException(string s) : base(s) {}
		public CruiseControlException(string s, Exception e) : base(s, e) {}
	}
}