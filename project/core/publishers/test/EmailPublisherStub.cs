using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[ReflectorType("emailstub")]
	public class EmailPublisherStub : EmailPublisher
	{
		[ReflectorProperty("save", Required=false)]
		public bool SaveToFile = false;

		internal override void SendMessage(string from, string to, string subject, string message)
		{
			Log.Debug("email message = " + message);
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
