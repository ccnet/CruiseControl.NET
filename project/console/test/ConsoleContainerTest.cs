using System.Diagnostics;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Console.Test
{
	[TestFixture]
	public class ConsoleContainerTest
	{
		private IMock mockArgs;
		private ConsoleContainer container;

		[SetUp]
		public void SetUp()
		{
			Trace.Listeners.Clear();

			mockArgs = new DynamicMock(typeof(IArgumentParser));
			mockArgs.SetupResult("ConfigFile", "ccnet.config");			
			container = new ConsoleContainer((IArgumentParser) mockArgs.MockInstance);
		}

		[TearDown]
		public void TearDown()
		{
			mockArgs.Verify();
		}

		[Test]
		public void CreateLocalCruiseServer()
		{
			mockArgs.ExpectAndReturn("IsRemote", false);

			ICruiseServer server = container.CreateServer();
			Assert.AreEqual(typeof(CruiseServer), server.GetType());
		}

		[Test]
		public void CreateRemoteCruiseServer()
		{
			mockArgs.ExpectAndReturn("IsRemote", true);

			ICruiseServer server = container.CreateServer();
			Assert.AreEqual(typeof(RemoteCruiseServer), server.GetType());
		}
	}
}