using System;
using System.Net;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
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
			P4 p4 = new P4();
			p4.Client = "myClient";
			p4.View = "//mydepot/...";

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = @"Client: myClient

Root:   c:\my\working\dir

View:
        //mydepot/... //myClient/mydepot/...
";

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfoWithStdInContent);

			// Execute
			p4Initializer.Initialize(p4, "myProject", @"c:\my\working\dir");

			// Verify
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
			processInfoWithStdInContent.StandardInputContent = string.Format(@"Client: {0}

Root:   c:\my\working\dir

View:
        //mydepot/... //{0}/mydepot/...
", expectedClientName);

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
			
		}
	}
}
