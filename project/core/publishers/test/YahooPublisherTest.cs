using System;
using System.Collections;
using System.Web.Mail;
using System.Xml;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers.test
{
	[TestFixture]
	public class YahooPublisherTest
	{
		private YahooPublisher _publisher;
	

		[SetUp]
		public void SetUp()
		{
			_publisher = YahooPublisherMother.Create();
			
		}
		public void TestSeeUsersAreGettingPopulated()
		{
			// uncomment these lines if you want to bug narsi :-) a message will goto Narsi.
			//_publisher.SendYahooMessage("narsi321", "hi this is test message";);
			Assertion.AssertEquals(5, _publisher.YahooUserIDs.Values.Count);
			Assertion.AssertEquals(2, _publisher.YahooGroups.Values.Count);
			
		}
		// I will write more tests over here to test yahoo messenger plugin
		// this is working fine right now.

	}
}