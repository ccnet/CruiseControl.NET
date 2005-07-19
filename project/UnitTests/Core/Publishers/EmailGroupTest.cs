using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class EmailGroupTest
	{
		[ExpectedException(typeof(ArgumentException))]
		public void TestSetNotificationException()
		{
			EmailGroup group = new EmailGroup();
			group.SetNotification("invalidnotification");			
		}

		[Test]
		public void TestSetNotificationAlways()
		{
			EmailGroup group = new EmailGroup();
			group.SetNotification("Always");			
		}

		[Test]
		public void TestSetNotificationChange()
		{
			EmailGroup group = new EmailGroup();
			group.SetNotification("Change");			
		}

		[Test]
		public void TestSetNotificationFailed()
		{
			EmailGroup group = new EmailGroup();
			group.SetNotification("Failed");			
		}
		
	}
}
