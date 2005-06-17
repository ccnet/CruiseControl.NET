using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
{
	[TestFixture]
	public class ProessP4PurgerTest
	{
		private DynamicMock processExecutorMock;
		private DynamicMock processInfoCreatorMock;
		private ProcessP4Purger p4Purger;
		private string tempDirPath;

		[SetUp]
		public void Setup()
		{
			processExecutorMock = new DynamicMock(typeof(ProcessExecutor));
			processInfoCreatorMock = new DynamicMock(typeof(IP4ProcessInfoCreator));
			p4Purger = new ProcessP4Purger((ProcessExecutor) processExecutorMock.MockInstance,  (IP4ProcessInfoCreator) processInfoCreatorMock.MockInstance);

			tempDirPath = TempFileUtil.CreateTempDir("tempDir");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir("tempDir");
		}
		
		private void VerifyAll()
		{
			processExecutorMock.Verify();
			processInfoCreatorMock.Verify();
		}

		[Test]
		public void ShouldDeleteClientSpecAndWorkingDirectoryOnPurge()
		{
			// Setup
			DynamicMock p4Mock = new DynamicMock(typeof(P4));
			P4 p4 = (P4) p4Mock.MockInstance;
			p4.Client = "myClient";

			ProcessInfo processInfo = new ProcessInfo("deleteclient");
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -d myClient");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfo);

			Assert.IsTrue(Directory.Exists(tempDirPath));

			// Execute
			p4Purger.Purge(p4, tempDirPath);

			// Verify
			Assert.IsFalse(Directory.Exists(tempDirPath));
			VerifyAll();
		}

		[Test]
		public void ShouldNotTryAndDeleteClientSpecIfClientSpecNotSet()
		{
			// Setup
			DynamicMock p4Mock = new DynamicMock(typeof(P4));
			P4 p4 = (P4) p4Mock.MockInstance;
			p4.Client = null;

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof(P4), typeof(string));
			processExecutorMock.ExpectNoCall("Execute", typeof(ProcessInfo));

			Assert.IsTrue(Directory.Exists(tempDirPath));

			// Execute
			p4Purger.Purge(p4, tempDirPath);

			// Verify
			Assert.IsFalse(Directory.Exists(tempDirPath));
			VerifyAll();
		}

		[Test]
		public void ShouldThrowAnExceptionIfProcessFails()
		{
			// Setup
			DynamicMock p4Mock = new DynamicMock(typeof(P4));
			P4 p4 = (P4) p4Mock.MockInstance;
			p4.Client = "myClient";

			ProcessInfo processInfo = new ProcessInfo("deleteclient");
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "client -d myClient");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("This is standard out", "This is standard error", 1, false), processInfo);

			// Execute
			try
			{
				p4Purger.Purge(p4, tempDirPath);
				Assert.Fail("Should throw an exception since process result has a non zero exit code");
			}
			catch (CruiseControlException e)
			{
				Assert.IsTrue(e.Message.IndexOf("This is standard out") > -1);
				Assert.IsTrue(e.Message.IndexOf("This is standard error") > -1);
			}

			VerifyAll();
			// Don't delete wd since the client may still exist
			Assert.IsTrue(Directory.Exists(tempDirPath));
		}
	}
}