using System;
using System.Diagnostics;
using System.Text;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using Exortech.NetReflector;
using tw.ccnet.core.util;
using tw.ccnet.core.test;

namespace tw.ccnet.core.publishers.test
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

			object result = new XmlPopulator().Populate(XmlUtil.CreateDocumentElement(xml));
			AssertNotNull(result);
			AssertEquals(typeof(NetSendPublisher), result.GetType());

			NetSendPublisher netsend = result as NetSendPublisher;
			AssertEquals("orogers", netsend.Names);
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

			object result = new XmlPopulator().Populate(XmlUtil.CreateDocumentElement(xml));
			AssertNotNull(result);
			AssertEquals(typeof(NetSendPublisher), result.GetType());

			NetSendPublisher netsend = result as NetSendPublisher;
			AssertEquals("orogers", netsend.Names);
			AssertEquals("DOH!", netsend.FailedMessage);
			AssertEquals("YAHOO!", netsend.FixedMessage);
		}

		[Test]
		public void ShouldSendMessage()
		{
			NetSendPublisher publisher = new NetSendPublisher();
			AssertFalse("message should not be sent", publisher.ShouldSendMessage(IntegrationResultMother.CreateSuccessful()));
			Assert("message should be sent", publisher.ShouldSendMessage(IntegrationResultMother.CreateFailed()));
			Assert("message should be sent", publisher.ShouldSendMessage(IntegrationResultMother.CreateFixed()));
		}

		[Test]
		public void BuildFailedMessage()
		{
			IntegrationResult result = CreateFailedIntegrationResult();
			NetSendPublisher publisher = new NetSendPublisher();
			string expected = "BUILD FAILED!\nLast comment: mod\nLast committer: owen";
			AssertEquals(expected, publisher.GetMessage(result));

			publisher.FailedMessage = "foo";
			AssertEquals("foo\nLast comment: mod\nLast committer: owen", publisher.GetMessage(result));
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
			AssertEquals(expected, publisher.GetMessage(IntegrationResultMother.CreateFailed()));
		}

		[Test]
		public void BuildFixedMessage()
		{
			IntegrationResult result = IntegrationResultMother.CreateFixed();
			NetSendPublisher publisher = new NetSendPublisher();
			AssertEquals("BUILD FIXED!", publisher.GetMessage(result));

			publisher.FixedMessage = "fixerama";
			AssertEquals("fixerama", publisher.GetMessage(result));
		}

		[Test]
		public void Publish()
		{
			DynamicMock mockPublisher = new DynamicMock(typeof(NetSendPublisher));
			mockPublisher.Ignore("PublishIntegrationResults");
			mockPublisher.ExpectAndReturn("ExecuteProcess", 0, new NMock.Constraints.IsTypeOf(typeof(Process)));

			NetSendPublisher publisher = (NetSendPublisher)mockPublisher.MockInstance;
			publisher.Names = "localhost";
			publisher.PublishIntegrationResults(null, IntegrationResultMother.CreateFailed());

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

			publisher.PublishIntegrationResults(null, IntegrationResultMother.CreateFailed());

			mockPublisher.Verify();
			AssertEquals(@"send ""machine1""", ((Process)process1.Parameter).StartInfo.Arguments.Substring(0, 15));
			AssertEquals(@"send ""machine2""", ((Process)process2.Parameter).StartInfo.Arguments.Substring(0, 15));
			AssertEquals(@"send ""machine3""", ((Process)process3.Parameter).StartInfo.Arguments.Substring(0, 15));
		}

		[Test, Ignore("move to acceptance tests")]
		public void Send()
		{
			NetSendPublisher publisher = new NetSendPublisher();
			publisher.Names = "localhost";
			publisher.PublishIntegrationResults(null, CreateFailedIntegrationResult());

			Win32Window window = Win32Window.Find("Messenger Service ");
			AssertNotNull(window);
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
