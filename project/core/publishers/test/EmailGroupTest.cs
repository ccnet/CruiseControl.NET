using System;
using NUnit.Framework;

namespace tw.ccnet.core.publishers.test
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
