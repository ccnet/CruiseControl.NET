using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Shared.Services
{
	[Serializable]
	public class InvalidCommandException : CruiseCommandException
	{
		public InvalidCommandException() : base("") {}
		public InvalidCommandException(string s) : base(s) {}
		public InvalidCommandException(string s, Exception e) : base(s, e) {}
		public InvalidCommandException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		public InvalidCommandException(ICruiseCommand cmd) : base("Invalid Command Type: " + cmd.GetType()) { }
	}
}
