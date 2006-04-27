using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	[Serializable]
	public class Message
	{
		private string message;

		public Message(string message)
		{
			this.message = message;
		}

		public override string ToString()
		{
			return message;
		}
	}
}