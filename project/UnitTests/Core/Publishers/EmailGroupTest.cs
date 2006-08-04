using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class EmailGroupTest
	{
		[Test, ExpectedException(typeof (NetReflectorException))]
		public void ReadEmailGroupFromXmlUsingInvalidNotificationType()
		{
			NetReflector.Read(@"<group name=""foo"" notification=""bar"" />");
		}

		[Test]
		public void ReadEmailGroupFromXmlUsingAlwaysNotificationType()
		{
			EmailGroup group = (EmailGroup) NetReflector.Read(@"<group name=""foo"" notification=""Always"" />");
			Assert.AreEqual("foo", group.Name);
			Assert.AreEqual(EmailGroup.NotificationType.Always, group.Notification);
		}

		[Test]
		public void ReadEmailGroupFromXmlUsingChangeNotificationType()
		{
			EmailGroup group = (EmailGroup) NetReflector.Read(@"<group name=""foo"" notification=""Change"" />");
			Assert.AreEqual(EmailGroup.NotificationType.Change, group.Notification);
		}

		[Test]
		public void ReadEmailGroupFromXmlUsingFailedNotificationType()
		{
			EmailGroup group = (EmailGroup) NetReflector.Read(@"<group name=""foo"" notification=""Failed"" />");
			Assert.AreEqual(EmailGroup.NotificationType.Failed, group.Notification);
		}
	}
}