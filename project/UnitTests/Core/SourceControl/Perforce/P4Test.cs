using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Perforce
{
	// ToDo - tidy up these tests by using mocks for process Executor, and make appropriate methods on P4 private
	// This already performed for 'Label', 'Get Source', 'Initialize'
	[TestFixture]
	public class P4Test : ProcessExecutorTestFixtureBase
	{
		private DynamicMock p4InitializerMock;
		private DynamicMock processInfoCreatorMock;
		private DynamicMock projectMock;
		private IProject project;
		private DynamicMock p4PurgerMock;

		[SetUp]
		public void Setup()
		{
			CreateProcessExecutorMock("p4");
			p4InitializerMock = new DynamicMock(typeof (IP4Initializer));
			p4PurgerMock = new DynamicMock(typeof (IP4Purger));
			processInfoCreatorMock = new DynamicMock(typeof (IP4ProcessInfoCreator));
			projectMock = new DynamicMock(typeof (IProject));
			project = (IProject) projectMock.MockInstance;
		}

		private void VerifyAll()
		{
			Verify();
			p4InitializerMock.Verify();
			p4PurgerMock.Verify();
			processInfoCreatorMock.Verify();
			projectMock.Verify();
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir("ccnet");
		}

		[Test]
		public void ReadConfig()
		{
			string xml = @"
<sourceControl type=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <password>mypassword</password>
  <port>anotherserver:2666</port>
  <workingDirectory>myWorkingDirectory</workingDirectory>
  <p4WebURLFormat>http://perforceWebServer:8080/@md=d&amp;cd=//&amp;c=3IB@/{0}?ac=10</p4WebURLFormat>
  <timeZoneOffset>-5.5</timeZoneOffset>
  <useExitCode>true</useExitCode>
  <errorPattern>Error: (.*)</errorPattern>
  <acceptableErrors>
    <acceptableError>(.*)\.accept1</acceptableError>
    <acceptableError>(.*)\.accept2</acceptableError>
  </acceptableErrors>
</sourceControl>
";
			P4 p4 = CreateP4WithNoArgContructor(xml);
			Assert.AreEqual(@"c:\bin\p4.exe", p4.Executable);
			Assert.AreEqual("//depot/myproject/...", p4.View);
			Assert.AreEqual("myclient", p4.Client);
			Assert.AreEqual("me", p4.User);
			Assert.AreEqual("mypassword", p4.Password);
			Assert.AreEqual("anotherserver:2666", p4.Port);
			Assert.AreEqual("myWorkingDirectory", p4.WorkingDirectory);
			Assert.AreEqual("http://perforceWebServer:8080/@md=d&cd=//&c=3IB@/{0}?ac=10", p4.P4WebURLFormat);
			Assert.AreEqual(-5.5, p4.TimeZoneOffset);
            Assert.AreEqual(true, p4.UseExitCode);
            Assert.AreEqual("Error: (.*)", p4.ErrorPattern);
            Assert.AreEqual(@"(.*)\.accept1", p4.AcceptableErrors[0]);
            Assert.AreEqual(@"(.*)\.accept2", p4.AcceptableErrors[1]);
		}

        [Test]
        public void ReadConfigWithEmptyErrorsArguments()
        {
            string xml = @"
<sourceControl type=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <errorPattern/>
  <acceptableErrors/>
</sourceControl>
";
            P4 p4 = CreateP4WithNoArgContructor(xml);
            Assert.AreEqual("", p4.ErrorPattern);
            Assert.AreEqual(0, p4.AcceptableErrors.Length);
        }

		private P4 CreateP4WithNoArgContructor(string p4root)
		{
			P4 perforce = new P4();
			NetReflector.Read(p4root, perforce);
			return perforce;
		}

		private P4 CreateP4()
		{
			return new P4((ProcessExecutor) mockProcessExecutor.MockInstance,
			              (IP4Initializer) p4InitializerMock.MockInstance,
			              (IP4Purger) p4PurgerMock.MockInstance,
			              (IP4ProcessInfoCreator) processInfoCreatorMock.MockInstance);
		}

		[Test]
		public void ReadConfigDefaults()
		{
			string xml = @"
<sourceControl name=""p4"">
  <view>//depot/anotherproject/...</view>
</sourceControl>
";
			P4 p4 = CreateP4WithNoArgContructor(xml);
			Assert.AreEqual("p4", p4.Executable);
			Assert.AreEqual("//depot/anotherproject/...", p4.View);
			Assert.AreEqual("", p4.Client);
			Assert.AreEqual("", p4.User);
			Assert.AreEqual("", p4.Password);
			Assert.AreEqual("", p4.Port);
            Assert.AreEqual(false, p4.UseExitCode);
            Assert.AreEqual(@"^error: .*", p4.ErrorPattern);
            Assert.AreEqual(@"file\(s\) up-to-date\.", p4.AcceptableErrors[0]);
		}

		[Test, ExpectedException(typeof (NetReflectorException))]
		public void ReadConfigBarfsWhenViewIsExcluded()
		{
			string xml = @"
<sourceControl name=""p4"">
</sourceControl>
";
			CreateP4WithNoArgContructor(xml);
		}

		[Test]
		public void CreateGetChangeListsProcess()
		{
			P4 p4 = new P4();
			p4.View = "//depot/myproj/...";
			DateTime from = new DateTime(2002, 10, 20, 2, 0, 0);
			DateTime to = new DateTime(2002, 10, 31, 5, 5, 0);

			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00";

			Assert.AreEqual("p4", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetChangeListsProcessInDifferentTimeZone()
		{
			P4 p4 = new P4();
			p4.View = "//depot/myproj/...";
			DateTime from = new DateTime(2002, 10, 20, 2, 0, 0);
			DateTime to = new DateTime(2002, 10, 31, 5, 5, 0);
			p4.TimeZoneOffset = -4.5;

			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/19:21:30:00,@2002/10/31:00:35:00";

			Assert.AreEqual("p4", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetChangeListsProcessWithMultiLineView()
		{
			P4 p4 = new P4();
			p4.View = "//depot/myproj/...,//myotherdepot/proj/...";
			DateTime from = new DateTime(2002, 10, 20, 2, 0, 0);
			DateTime to = new DateTime(2002, 10, 31, 5, 5, 0);

			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00 //myotherdepot/proj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00";

			Assert.AreEqual("p4", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetChangeListsProcessWithDifferentArgs()
		{
			string xml = @"
<sourceControl name=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <password>mypassword</password>
  <port>anotherserver:2666</port>
</sourceControl>
";

			DateTime from = new DateTime(2003, 11, 20, 2, 10, 32);
			DateTime to = new DateTime(2004, 10, 31, 5, 5, 1);

			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me -P mypassword"
				+ " changes -s submitted //depot/myproject/...@2003/11/20:02:10:32,@2004/10/31:05:05:01";

			P4 p4 = CreateP4WithNoArgContructor(xml);
			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			Assert.AreEqual("c:\\bin\\p4.exe", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetDescribeProcess()
		{
			string changes = "3327 3328 332";
			ProcessInfo process = new P4().CreateDescribeProcess(changes);

			string expectedArgs = "-s describe -s " + changes;
			Assert.AreEqual("p4", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetDescribeProcessWithSpecifiedArgs()
		{
			string xml = @"
<sourceControl name=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <password>mypassword</password>
  <port>anotherserver:2666</port>
</sourceControl>
";
			string changes = "3327 3328 332";

			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me -P mypassword"
				+ " describe -s " + changes;

			P4 p4 = CreateP4WithNoArgContructor(xml);
			ProcessInfo process = p4.CreateDescribeProcess(changes);

			Assert.AreEqual("c:\\bin\\p4.exe", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		[ExpectedException(typeof (CruiseControlException))]
		public void CreateGetDescribeProcessWithEvilCode()
		{
			string changes = "3327 3328 332; echo 'rm -rf /'";
			new P4().CreateDescribeProcess(changes);
		}

		[Test, ExpectedException(typeof (Exception))]
		public void CreateGetDescribeProcessWithNoChanges()
		{
			new P4().CreateDescribeProcess("");
			// this should never happen, but here's a test just in case.
		}

		[Test]
		public void GetModifications()
		{
			string changes = @"
info: Change 3328 on 2002/10/31 by someone@somewhere 'Something important '
info: Change 3327 on 2002/10/31 by someone@somewhere 'Joe's test '
info: Change 332 on 2002/10/31 by someone@somewhere 'thingy'
exit: 0
";

			P4 p4 = CreateP4();

			ProcessInfo info = new ProcessInfo("p4.exe");
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", info, p4, "changes -s submitted ViewData@0001/01/01:00:00:00");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult(changes, "", 0, false), info);
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", info, p4, "describe -s 3328 3327 332");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult(P4Mother.P4_LOGFILE_CONTENT, "", 0, false), info);

			p4.View = "ViewData";
			p4.P4WebURLFormat = "http://perforceWebServer:8080/@md=d&amp;cd=//&amp;c=3IB@/{0}?ac=10";
			Modification[] result = p4.GetModifications(new IntegrationResult(), new IntegrationResult());

			VerifyAll();
			Assert.AreEqual(7, result.Length);
			Assert.AreEqual("http://perforceWebServer:8080/@md=d&amp;cd=//&amp;c=3IB@/3328?ac=10", result[0].Url);
			Assert.AreEqual("http://perforceWebServer:8080/@md=d&amp;cd=//&amp;c=3IB@/3327?ac=10", result[3].Url);
		}

		[Test]
		public void LabelSourceControlIfApplyLabelTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = true;

			ProcessInfo labelSpecProcess = new ProcessInfo("spec");
			ProcessInfo labelSpecProcessWithStdInContent = new ProcessInfo("spec");
			labelSpecProcessWithStdInContent.StandardInputContent = "Label:	foo-123\n\nDescription:\n	Created by CCNet\n\nOptions:	unlocked\n\nView:\n //depot/myproject/...\n";
			ProcessInfo labelSyncProcess = new ProcessInfo("sync");

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSpecProcess, p4, "label -i");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSpecProcessWithStdInContent);
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSyncProcess, p4, "labelsync -l foo-123");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSyncProcess);

			// Execute
			p4.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo-123"));

			// Verify
			VerifyAll();
		}

		[Test]
		public void LabelSourceControlIfApplyLabelTrueWithMultiLineViews()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproj/...,//myotherdepot/proj/...";
			p4.ApplyLabel = true;

			ProcessInfo labelSpecProcess = new ProcessInfo("spec");
			ProcessInfo labelSpecProcessWithStdInContent = new ProcessInfo("spec");
			labelSpecProcessWithStdInContent.StandardInputContent = "Label:	foo-123\n\nDescription:\n	Created by CCNet\n\nOptions:	unlocked\n\nView:\n //depot/myproj/...\n //myotherdepot/proj/...\n";
			ProcessInfo labelSyncProcess = new ProcessInfo("sync");

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSpecProcess, p4, "label -i");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSpecProcessWithStdInContent);
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSyncProcess, p4, "labelsync -l foo-123");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSyncProcess);

			// Execute
			p4.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo-123"));

			// Verify
			VerifyAll();
		}

		[Test]
		public void ViewForSpecificationsSupportsSingleLineView()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproj/...";

			Assert.AreEqual(1, p4.ViewForSpecifications.Length);
			Assert.AreEqual("//depot/myproj/...", p4.ViewForSpecifications[0]);
		}

		[Test]
		public void ViewForSpecificationsSupportsMultiLineView()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproj/...,//myotherdepot/proj/...";

			Assert.AreEqual(2, p4.ViewForSpecifications.Length);
			Assert.AreEqual("//depot/myproj/...", p4.ViewForSpecifications[0]);
			Assert.AreEqual("//myotherdepot/proj/...", p4.ViewForSpecifications[1]);
		}

		[Test]
		public void LabelSourceControlFailsIfLabelIsOnlyNumeric()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = true;

			try
			{
				p4.LabelSourceControl(IntegrationResultMother.CreateSuccessful("123"));
				Assert.Fail("Perforce labelling should fail if a purely numeric label is attempted to be applied");
			}
			catch (CruiseControlException)
			{}

			VerifyAll();
		}

		[Test]
		public void DontLabelSourceControlIfApplyLabelFalse()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = false;

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof (P4), typeof (string));
			mockProcessExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			p4.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo-123"));

			VerifyAll();
		}

		[Test]
		public void DontLabelSourceControlIfIntegrationFailed()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = true;

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof (P4), typeof (string));
			mockProcessExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			p4.LabelSourceControl(IntegrationResultMother.CreateFailed("foo-123"));

			VerifyAll();
		}

		[Test]
		public void DontLabelSourceControlIfApplyLabelNotSetEvenIfInvalidLabel()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof (P4), typeof (string));
			mockProcessExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			p4.LabelSourceControl(IntegrationResultMother.CreateSuccessful("123"));

			VerifyAll();
		}

		[Test]
		public void GetSourceIfGetSourceTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = true;

            DateTime modificationsToDate = new DateTime(2002, 10, 31, 5, 5, 0);
			ProcessInfo processInfo = new ProcessInfo("getSource");
            processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "sync @2002/10/31:05:05:00");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfo);
            p4.GetSource(IntegrationResultMother.CreateSuccessful(modificationsToDate));

			VerifyAll();
		}

		[Test]
		public void GetForceSourceIfGetSourceTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = true;
			p4.ForceSync = true;

            DateTime modificationsToDate = new DateTime(2002, 10, 31, 5, 5, 0);
			ProcessInfo processInfo = new ProcessInfo("getSource");
            processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "sync -f @2002/10/31:05:05:00");
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfo);
            p4.GetSource(IntegrationResultMother.CreateSuccessful(modificationsToDate));

			VerifyAll();
		}

		[Test]
		public void DontGetSourceIfGetSourceFalse()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = false;

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof (P4), typeof (string));
			mockProcessExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			p4.GetSource(new IntegrationResult());
			VerifyAll();
		}

		[Test]
		public void ShouldCallInitializerWithGivenWorkingDirectoryIfAlternativeNotSet()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4InitializerMock.Expect("Initialize", p4, "myProject", "workingDirFromProject");
			projectMock.ExpectAndReturn("Name", "myProject");
			projectMock.ExpectAndReturn("WorkingDirectory", "workingDirFromProject");

			// Execute
			p4.Initialize(project);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallInitializerWithGivenWorkingDirectoryIfAlternativeSetToEmpty()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.WorkingDirectory = "";
			p4InitializerMock.Expect("Initialize", p4, "myProject", "workingDirFromProject");
			projectMock.ExpectAndReturn("Name", "myProject");
			projectMock.ExpectAndReturn("WorkingDirectory", "workingDirFromProject");

			// Execute
			p4.Initialize(project);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallInitializerWithConfiguredWorkingDirectoryIfAlternativeIsConfigured()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.WorkingDirectory = "p4sOwnWorkingDirectory";
			p4InitializerMock.Expect("Initialize", p4, "myProject", "p4sOwnWorkingDirectory");
			projectMock.ExpectAndReturn("Name", "myProject");
			projectMock.ExpectNoCall("WorkingDirectory");

			// Execute
			p4.Initialize(project);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallPurgerWithGivenWorkingDirectoryIfAlternativeNotSet()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4PurgerMock.Expect("Purge", p4, "workingDirFromProject");
			projectMock.ExpectAndReturn("WorkingDirectory", "workingDirFromProject");

			// Execute
			p4.Purge(project);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallPurgerWithGivenWorkingDirectoryIfAlternativeSetToEmpty()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.WorkingDirectory = "";
			p4PurgerMock.Expect("Purge", p4, "workingDirFromProject");
			projectMock.ExpectAndReturn("WorkingDirectory", "workingDirFromProject");

			// Execute
			p4.Purge(project);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallPurgerWithConfiguredWorkingDirectoryIfAlternativeIsConfigured()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.WorkingDirectory = "p4sOwnWorkingDirectory";
			p4PurgerMock.Expect("Purge", p4, "p4sOwnWorkingDirectory");
			projectMock.ExpectNoCall("WorkingDirectory");

			// Execute
			p4.Purge(project);

			// Verify
			VerifyAll();
		}
	}
}