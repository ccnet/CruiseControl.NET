using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Xml;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using NMock.Remoting;
using tw.ccnet.remote;

namespace CCNet.CCRunner.test
{
	[TestFixture]
	public class RunnerTest : Assertion
	{
		[TearDown]
		protected void TearDown()
		{
			Environment.ExitCode = 0;
		}

    	[Test]
		public void SendScheduleToCruiseManager()
		{
			CollectingConstraint projectNameConstraint = new CollectingConstraint();
			CollectingConstraint scheduleConstraint = new CollectingConstraint();
			RemotingMock cc = new RemotingMock(typeof(ICruiseManager));
			cc.Expect("Run", projectNameConstraint, scheduleConstraint);

			using (MockServer server = new MockServer(cc.MarshalByRefInstance, new TcpChannel(2334), "MockCruise.rem"))
			{
				Runner runner = new Runner();
				runner.Url = "tcp://localhost:2334/MockCruise.rem";
				runner.Run("myProject");
			}

			AssertEquals("myProject", projectNameConstraint.Parameter);
			AssertEquals(new Schedule(), scheduleConstraint.Parameter);
			cc.Verify();
		}

		[Test, ExpectedException(typeof(Exception))]
		public void HandleServerException()
		{
			RemotingMock cc = new RemotingMock(typeof(ICruiseManager));
			cc.ExpectAndThrow("Run", new Exception("server exception"), new IsAnything(), new IsAnything());

			TcpChannel channel = new TcpChannel(2334);
			using (MockServer server = new MockServer(cc.MarshalByRefInstance, channel, "MockCruise.rem"))
			{
				Runner runner = new Runner();
				runner.Url = "tcp://localhost:2334/MockCruise.rem";
				runner.Run("myProject");
			}
		}

		[Test]
		public void HandleConnectionProblem()
		{
			Runner runner = new Runner();
			runner.Url = "tcp://localhost:2134/NoRunningServer.rem";
			try 
			{ 
				runner.Run("project"); 
				Fail("Expected exception");
			}
			catch (Exception ex)
			{
				AssertEquals(typeof(ServerConnectionException), ex.GetType());
			}
			finally
			{
				// NB. For some reason, .NET opens a client channel and doesn't close it when the server does not exist.
				if (ChannelServices.RegisteredChannels.Length > 0)
				{
					Console.WriteLine("Channel: " + ChannelServices.RegisteredChannels[0].ChannelName);
					ChannelServices.UnregisterChannel(ChannelServices.RegisteredChannels[0]);
				}
			}
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void RunWithNullProjectName()
		{
			Runner runner = new Runner();
			runner.Url = "http://foo.com/bar.xml";
			runner.Run(null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void RunWithNullUrl()
		{
			Runner runner = new Runner();
			runner.Url = null;
			runner.Run("foo");
		}

		[Test]
		public void ParseUrl()
		{
			AssertEquals("tcp://localhost:2334/MockCruise.rem", Runner.ParseUrl(new string[] { "-url:tcp://localhost:2334/MockCruise.rem", "myProject" }));
			AssertNull(Runner.ParseUrl(new string[] { "myProject" }));
		}

		[Test]
		public void ParseProject()
		{
			AssertEquals("myProject", Runner.ParseProject(new string[] { "-url:tcp://localhost:2334/MockCruise.rem", "myProject" }));
			AssertNull(Runner.ParseProject(new string[] { "-url:tcp://localhost:2334/MockCruise.rem" }));
		}

		[Test]
		public void MainShowsHelpAndExceptionMessageOnException()
		{
			StringBuilder buffer = new StringBuilder();
			Runner.Out = new StringWriter(buffer);
			Runner.Main(new string[] {});
			Assert(buffer.ToString().StartsWith(Runner.NULL_PROJECT_MSG));
			Assert(buffer.ToString().EndsWith(Runner.HELP + Console.Out.NewLine));
			AssertEquals(1, Environment.ExitCode);
		}

		// help shown with -help arg or no args
		[Test]
		public void ShowHelp()
		{
			StringBuilder buffer = new StringBuilder();
			Runner.Out = new StringWriter(buffer);
			Runner.Main(new string[] { "-boo", "foo", "-help" });
			AssertEquals(Runner.HELP + Console.Out.NewLine, buffer.ToString());
			AssertEquals(1, Environment.ExitCode);
		}

		[Test]
		public void GetUrlFromConfig()
		{
			Runner runner = new Runner();
			AssertNotNull(runner.Url);
		}
	}
}
