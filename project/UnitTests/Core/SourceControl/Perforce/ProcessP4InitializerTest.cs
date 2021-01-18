using System.Net;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Perforce
{
	[TestFixture]
	public class ProcessP4InitializerTest
	{
		private Mock<ProcessExecutor> processExecutorMock;
		private Mock<IP4ProcessInfoCreator> processInfoCreatorMock;
		private ProcessP4Initializer p4Initializer;
        private string path = Platform.IsWindows ? @"c:\my\working\dir" : @"/my/working/dir";

		[SetUp]
		public void Setup()
		{
			processExecutorMock = new Mock<ProcessExecutor>();
			processInfoCreatorMock = new Mock<IP4ProcessInfoCreator>();
			p4Initializer = new ProcessP4Initializer((ProcessExecutor) processExecutorMock.Object,  (IP4ProcessInfoCreator) processInfoCreatorMock.Object);
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
			p4.View = "//mydepot/...,//myotherdepot/...";

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = "Client: myClient\n\nRoot:   " + path + "\n\nView:\n //mydepot/... //myClient/mydepot/...\n //myotherdepot/... //myClient/myotherdepot/...\n";

			processInfoCreatorMock.Setup(creator => creator.CreateProcessInfo(p4, "client -i")).Returns(processInfo).Verifiable();
			processExecutorMock.Setup(executor => executor.Execute(processInfoWithStdInContent)).Returns(new ProcessResult("", "", 0, false)).Verifiable();

			// Execute
			p4Initializer.Initialize(p4, "myProject", path);

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

			string expectedClientName = string.Format(System.Globalization.CultureInfo.CurrentCulture,"CCNet-{0}-{1}", Dns.GetHostName(), projectName);

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Client: {0}\n\nRoot:   " + path + "\n\nView:\n //mydepot/... //{0}/mydepot/...\n", expectedClientName);

			processInfoCreatorMock.Setup(creator => creator.CreateProcessInfo(p4, "client -i")).Returns(processInfo).Verifiable();
			processExecutorMock.Setup(executor => executor.Execute(processInfoWithStdInContent)).Returns(new ProcessResult("", "", 0, false)).Verifiable();

			// Execute
			p4Initializer.Initialize(p4, projectName, path);

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
				p4Initializer.Initialize(p4, "myProject", path);
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
			processInfoCreatorMock.Setup(creator => creator.CreateProcessInfo(It.IsAny<P4>(), It.IsAny<string>())).Returns(new ProcessInfo("")).Verifiable();
			processExecutorMock.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", "", 0, false)).Verifiable();

			p4Initializer.Initialize(p4, "myProject", path);
			VerifyAll();
		}

		[Test]
		public void ShouldThrowExceptionIfProcessFails()
		{
			// Setup
			P4 p4 = new P4();
			p4.View = "//mydepot/...";
			string projectName = "myProject";

			string expectedClientName = string.Format(System.Globalization.CultureInfo.CurrentCulture,"CCNet-{0}-{1}", Dns.GetHostName(), projectName);

			ProcessInfo processInfo = new ProcessInfo("createclient");
			ProcessInfo processInfoWithStdInContent = new ProcessInfo("createclient");
			processInfoWithStdInContent.StandardInputContent = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Client: {0}\n\nRoot:   " + path + "\n\nView:\n //mydepot/... //{0}/mydepot/...\n", expectedClientName);

			processInfoCreatorMock.Setup(creator => creator.CreateProcessInfo(p4, "client -i")).Returns(processInfo).Verifiable();
			processExecutorMock.Setup(executor => executor.Execute(processInfoWithStdInContent)).Returns(new ProcessResult("This is standard out", "This is standard error", 1, false)).Verifiable();

			// Execute
			try
			{
				p4Initializer.Initialize(p4, projectName, path);
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
