using System;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
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
