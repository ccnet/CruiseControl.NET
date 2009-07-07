using System.Net;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Perforce
{
	[TestFixture]
	public class ProcessP4InitializerTest
	{
		private DynamicMock processExecutorMock;
		private DynamicMock processInfoCreatorMock;
		private ProcessP4Initializer p4Initializer;

		[SetUp]
		public void Setup()
		{
			processExecutorMock = new DynamicMock(typeof(ProcessExecutor));
			processInfoCreatorMock = new DynamicMock(typeof(IP4ProcessInfoCreator));
			p4Initializer = new ProcessP4Initializer((ProcessExecutor) processExecutorMock.MockInstance,  (IP4ProcessInfoCreator) processInfoCreatorMock.MockInstance);
		}

		private void VerifyAll()
		{
			processExecutorMock.Verify();
			processInfoCreatorMock.Verify();
		}

		[Test]
		public void CreatesAClientWithGivenClientNameIfSpecified()
		{
			// Setup
			DynamicMock p4Mock = new DynamicMock(typeof(P4));
			P4 p4 = (P4) p4Mock.MockInstance;
			p4.Client = "myClient";

			p4Mock.SetupResult("ViewForSpecifications", new string[] { "//mydepot/...", "//myotherdepot/..." });

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = "Client: myClient\n\nRoot:   c:\\my\\working\\dir\n\nView:\n //mydepot/... //myClient/mydepot/...\n //myotherdepot/... //myClient/myotherdepot/...\n";

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfoWithStdInContent);

			// Execute
			p4Initializer.Initialize(p4, "myProject", @"c:\my\working\dir");

			// Verify
			p4Mock.Verify();
			VerifyAll();
		}

		[Test]
		public void CreatesAClientWithConstructedClientNameIfOneNotSpecifiedAndSavesClientNameInConfig()
		{
			// Setup
			P4 p4 = new P4();
			p4.View = "//mydepot/...";
			string projectName = "myProject";

			string expectedClientName = string.Format("CCNet-{0}-{1}", Dns.GetHostName(), projectName);

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = string.Format("Client: {0}\n\nRoot:   c:\\my\\working\\dir\n\nView:\n //mydepot/... //{0}/mydepot/...\n", expectedClientName);

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfoWithStdInContent);

			// Execute
			p4Initializer.Initialize(p4, projectName, @"c:\my\working\dir");

			// Verify
			Assert.AreEqual(expectedClientName, p4.Client);
			VerifyAll();
		}

		[Test]
		public void ShouldCheckToSeeWorkingDirectoryIsAnAbsolutePath()
		{
			P4 p4 = new P4();
			try
			{
				p4Initializer.Initialize(p4, "myProject", "thisIsNotAnAbsoluteDirectory");
				Assert.Fail("Should check for non absolute working directory");
			}
			catch (CruiseControlException e)
			{
				Assert.IsTrue(e.Message.ToLower().IndexOf("absolute path") > -1, "Should mention something about an absolute directory");
			}

			VerifyAll();
		}

		[Test]
		public void ShouldCheckViewIsValid()
		{
			P4 p4 = new P4();
			p4.View = "ThisIsNotAValidView";
			try
			{
				p4Initializer.Initialize(p4, "myProject", @"c:\my\working\dir");
				Assert.Fail("Should check for a valid view");
			}
			catch (CruiseControlException e)
			{
				Assert.IsTrue(e.Message.ToLower().IndexOf("valid view") > -1, "Should mention something about a valid view");
			}
			VerifyAll();
		}

		// CCNET-174
		[Test]
		public void ShouldAllowViewsWithSpaces()
		{
			P4 p4 = new P4();
			p4.View = @"""//blah/my path with spaces/...""";
			processInfoCreatorMock.SetupResult("CreateProcessInfo", new ProcessInfo(""), typeof(P4), typeof(string));
			processExecutorMock.SetupResult("Execute", new ProcessResult("", "", 0, false), typeof(ProcessInfo));

			p4Initializer.Initialize(p4, "myProject", @"c:\my\working\dir");
			VerifyAll();
		}

		[Test]
		public void ShouldThrowExceptionIfProcessFails()
		{
			// Setup
			P4 p4 = new P4();
			p4.View = "//mydepot/...";
			string projectName = "myProject";

			string expectedClientName = string.Format("CCNet-{0}-{1}", Dns.GetHostName(), projectName);

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = string.Format("Client: {0}\n\nRoot:   c:\\my\\working\\dir\n\nView:\n //mydepot/... //{0}/mydepot/...\n", expectedClientName);

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("This is standard out", "This is standard error", 1, false), processInfoWithStdInContent);

			// Execute
			try
			{
				p4Initializer.Initialize(p4, projectName, @"c:\my\working\dir");
				Assert.Fail("Should throw an exception since process result has a non zero exit code");
			}
			catch (CruiseControlException e)
			{
				Assert.IsTrue(e.Message.IndexOf("This is standard out") > -1);
				Assert.IsTrue(e.Message.IndexOf("This is standard error") > -1);
			}

			// Verify
			Assert.AreEqual(expectedClientName, p4.Client);
			VerifyAll();
		}
	}
}
