using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseControlRemotingException : CruiseControlException
	{
		public CruiseControlRemotingException(string message, string url, Exception e) : base(CreateMessage(message, url), e) {}
		public CruiseControlRemotingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		private static string CreateMessage(string message, string url)
		{
			return String.Format("Cannot connect to CruiseControl server {0}.  {1}", url, message);
		}
	}
}
