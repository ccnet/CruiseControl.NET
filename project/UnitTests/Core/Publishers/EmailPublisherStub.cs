using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[ReflectorType("emailstub")]
	public class EmailPublisherStub : EmailPublisher
	{
		[ReflectorProperty("save", Required=false)]
		public bool SaveToFile = false;

		public override void SendMessage(string from, string to, string replyto, string subject, string message)
		{
			CruiseControl.Core.Util.Log.Debug("email message = " + message);
			if (SaveToFile)
			{
				using (StreamWriter writer = File.CreateText("emailstub.html"))
				{
					writer.Write(message);
				}
			}
		}
	}
}