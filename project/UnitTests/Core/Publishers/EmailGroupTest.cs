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
	}
}
