using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class NetSendPublisherTest : CustomAssertion
	{
		[Test]
		public void LoadMinXmlFromConfig()
		{
			string xml = @"
<netsend>
	<names>orogers</names>
</netsend>";

			object result = NetReflector.Read(xml);
			Assert.IsNotNull(result);
			Assert.IsTrue(result is NetSendPublisher);

			NetSendPublisher netsend = result as NetSendPublisher;
			Assert.AreEqual("orogers", netsend.Names);
		}

		[Test]
		public void LoadMaxXmlFromConfig()
		{
			string xml = @"
<netsend>
	<names>orogers</names>
	<failedMessage>DOH!</failedMessage>
	<fixedMessage>YAHOO!</fixedMessage>
</netsend>";

			object result = NetReflector.Read(xml);
			Assert.IsNotNull(result);
			Assert.AreEqual(typeof(NetSendPublisher), result.GetType());

			NetSendPublisher netsend = result as NetSendPublisher;
			Assert.AreEqual("orogers", netsend.Names);
			Assert.AreEqual("DOH!", netsend.FailedMessage);
			Assert.AreEqual("YAHOO!", netsend.FixedMessage);
		}

		[Test]
		public void ShouldSendMessage()
		{
			NetSendPublisher publisher = new NetSendPublisher();
			AssertFalse("message should not be sent", publisher.ShouldSendMessage(IntegrationResultMother.CreateSuccessful()));
			Assert.IsTrue(publisher.ShouldSendMessage(IntegrationResultMother.CreateFailed()));
			Assert.IsTrue(publisher.ShouldSendMessage(IntegrationResultMother.CreateFixed()));
		}

		[Test]
		public void BuildFailedMessage()
		{
			IntegrationResult result = CreateFailedIntegrationResult();
			NetSendPublisher publisher = new NetSendPublisher();
			string expected = "BUILD FAILED!\nLast comment: mod\nLast committer: owen";
			Assert.AreEqual(expected, publisher.GetMessage(result));

			publisher.FailedMessage = "foo";
			Assert.AreEqual("foo\nLast comment: mod\nLast committer: owen", publisher.GetMessage(result));
		}

		private IntegrationResult CreateFailedIntegrationResult()
		{
			IntegrationResult result = IntegrationResultMother.CreateFailed();
			result.Modifications = new Modification[1];
			result.Modifications[0] = new Modification();
			result.Modifications[0].Comment = "mod";
			result.Modifications[0].UserName = "owen";
			return result;
		}

		[Test]
		public void BuildFailedMessage_NoModifications()
		{
			NetSendPublisher publisher = new NetSendPublisher();
			string expected = "BUILD FAILED!\nLast comment: Unknown\nLast committer: Unknown";
			Assert.AreEqual(expected, publisher.GetMessage(IntegrationResultMother.CreateFailed()));
		}

		[Test]
		public void BuildFixedMessage()
		{
			IntegrationResult result = IntegrationResultMother.CreateFixed();
			NetSendPublisher publisher = new NetSendPublisher();
			Assert.AreEqual("BUILD FIXED!", publisher.GetMessage(result));

			publisher.FixedMessage = "fixerama";
			Assert.AreEqual("fixerama", publisher.GetMessage(result));
		}

		[Test]
		public void Publish()
		{
			DynamicMock mockPublisher = new DynamicMock(typeof(NetSendPublisher));
			mockPublisher.Ignore("PublishIntegrationResults");
			mockPublisher.ExpectAndReturn("ExecuteProcess", 0, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));

			NetSendPublisher publisher = (NetSendPublisher)mockPublisher.MockInstance;
			publisher.Names = "localhost";
			publisher.PublishIntegrationResults(IntegrationResultMother.CreateFailed());

			mockPublisher.Verify();
		}

		[Test]
		public void PublishToMultipleNames()
		{
			CollectingConstraint process1 = new CollectingConstraint();
			CollectingConstraint process2 = new CollectingConstraint();
			CollectingConstraint process3 = new CollectingConstraint();

			DynamicMock mockPublisher = new DynamicMock(typeof(NetSendPublisher));
			mockPublisher.Ignore("PublishIntegrationResults");
			mockPublisher.ExpectAndReturn("ExecuteProcess", 0, process1);
			mockPublisher.ExpectAndReturn("ExecuteProcess", 0, process2);
			mockPublisher.ExpectAndReturn("ExecuteProcess", 0, process3);

			NetSendPublisher publisher = (NetSendPublisher)mockPublisher.MockInstance;
			publisher.Names = "machine1,machine2,machine3";

			publisher.PublishIntegrationResults(IntegrationResultMother.CreateFailed());

			mockPublisher.Verify();
			Assert.AreEqual(@"send ""machine1""", ((ProcessInfo)process1.Parameter).Arguments.Substring(0, 15));
			Assert.AreEqual(@"send ""machine2""", ((ProcessInfo)process2.Parameter).Arguments.Substring(0, 15));
			Assert.AreEqual(@"send ""machine3""", ((ProcessInfo)process3.Parameter).Arguments.Substring(0, 15));
		}

		[Test, Ignore("move to acceptance tests")]
		public void Send()
		{
			NetSendPublisher publisher = new NetSendPublisher();
			publisher.Names = "localhost";
			publisher.PublishIntegrationResults(CreateFailedIntegrationResult());

			Win32Window window = Win32Window.Find("Messenger Service ");
			Assert.IsNotNull(window);
			window.Close();
		}

		public class Win32Window
		{
			private const int WM_SYSCOMMAND = 0x0112;
			private const int SC_CLOSE = 0xF060;
			private IntPtr hWnd;

			public Win32Window(IntPtr hWnd)
			{
				this.hWnd = hWnd;
			}

			public void Close()
			{
				SendMessage(hWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
			}

			public static Win32Window Find(string title)
			{
				int hwnd = FindWindow(null, title);
				return hwnd == 0 ? null : new Win32Window(new IntPtr(hwnd));
			}

			[DllImport("user32.dll", EntryPoint="FindWindow")]
			private static extern int FindWindow(string className, string windowName);

			[DllImport("user32.dll", CharSet=CharSet.Auto)]
			public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		}
	}
}
