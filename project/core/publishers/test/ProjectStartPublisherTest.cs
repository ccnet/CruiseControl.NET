using System;
using System.Runtime.Remoting.Channels.Tcp;
using Exortech.NetReflector;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using NMock.Remoting;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class ProjectStartPublisherTest : CustomAssertion
	{
		[Test]
		public void LoadMinXmlFromConfig()
		{
			string xml = @"
<startproject>
	<project>myproject</project>
	<url>tcp://localhost:2334/MockCruise.rem</url>
</startproject>";

			object result = NetReflector.Read(XmlUtil.CreateDocumentElement(xml));
			AssertNotNull(result);
			AssertEquals(typeof(ProjectStartPublisher), result.GetType());

			ProjectStartPublisher projectStart = result as ProjectStartPublisher;
			AssertEquals("myproject", projectStart.Project);
			AssertEquals("tcp://localhost:2334/MockCruise.rem", projectStart.Url);
		}

		[Test]
		public void StartProject()
		{
			RemotingMock mock = new RemotingMock(typeof(ICruiseManager));
			mock.Expect("ForceBuild", "myproject");

			using (MockServer server = new MockServer(mock.MarshalByRefInstance, new TcpChannel(4444), "Cruise2.rem"))
			{
				ExecutePublisher("myproject", "tcp://localhost:4444/Cruise2.rem");
			}

			mock.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlRemotingException))]
		public void RemoteCruiseNotStarted()
		{
			ExecutePublisher("myproject", "tcp://localhost:4445/Cruise2.rem");
		}

		[Test, ExpectedException(typeof(CruiseControlRemotingException))]
		public void InvalidRemoteUrl()
		{
			ExecutePublisher("myproject", "foo.bar");
		}

		[Test, ExpectedException(typeof(CruiseControlRemotingException))]
		public void InvalidRemoteUrlUsingHttp()
		{
			ExecutePublisher("myproject", "tcp://localhost:4445/Cruise2.rem");
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void RemoteProjectThrowsException()
		{
			RemotingMock mock = new RemotingMock(typeof(ICruiseManager));
			mock.ExpectAndThrow("Run", new CruiseControlException("expected"), "myproject", new IsAnything());

			using (MockServer server = new MockServer(mock.MarshalByRefInstance, new TcpChannel(4446), "Cruise3.rem"))
			{
				ExecutePublisher("myproject", "tcp://localhost:4446/Cruise3.rem");
			}

			mock.Verify();
		}

		// resource has wrong name

		private void ExecutePublisher(string project, string url)
		{
			ProjectStartPublisher publisher = new ProjectStartPublisher();
			ExecutePublisher(project, url, publisher);
		}

		private void ExecutePublisherThrowsException(string project, string url, Exception ex)
		{
			Mock managerMock = new DynamicMock(typeof(ICruiseManager));
			managerMock.ExpectAndThrow("ForceBuild", new CruiseControlException("expected exception"));
			ProjectStartPublisherExtension publisher = new ProjectStartPublisherExtension((ICruiseManager) managerMock.MockInstance);
			ExecutePublisher(project, url, publisher);
		}

		private void ExecutePublisher(string project, string url, ProjectStartPublisher publisher)
		{
			publisher.Project = project;
			publisher.Url = url;
			publisher.PublishIntegrationResults(null, IntegrationResultMother.CreateSuccessful());
			publisher.WaitForCompletion();
		}

		class ProjectStartPublisherExtension : ProjectStartPublisher
		{
			ICruiseManager _manager;

			public ProjectStartPublisherExtension(ICruiseManager manager)
			{
				_manager = manager;
			}

			protected override ICruiseManager GetRemoteCruiseManager()
			{
				return _manager;
			}
		}
	}
}
