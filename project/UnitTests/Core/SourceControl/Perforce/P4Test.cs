using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
{
	// ToDo - tidy up these tests by using mocks for process Executor, and make appropriate methods on P4 private
	// This already performed for 'Label', 'Get Source', 'Initialize'
	[TestFixture]
	public class P4Test : CustomAssertion
	{
		private DynamicMock processExecutorMock;
		private DynamicMock p4InitializerMock;
		private DynamicMock processInfoCreatorMock;
		private DynamicMock projectMock;
		private IProject project;
		private DynamicMock p4PurgerMock;

		[SetUp]
		public void Setup()
		{
			processExecutorMock = new DynamicMock(typeof(ProcessExecutor)); 
			processExecutorMock.Strict = true;
			p4InitializerMock = new DynamicMock(typeof(IP4Initializer));
			p4PurgerMock = new DynamicMock(typeof(IP4Purger));
			processInfoCreatorMock = new DynamicMock(typeof(IP4ProcessInfoCreator));
			projectMock = new DynamicMock(typeof(IProject));
			project = (IProject) projectMock.MockInstance;
		}

		private void VerifyAll()
		{
			processExecutorMock.Verify();
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
  <port>anotherserver:2666</port>
  <workingDirectory>myWorkingDirectory</workingDirectory>
</sourceControl>
";
			P4 p4 = CreateP4WithNoArgContructor(xml);
			Assert.AreEqual(@"c:\bin\p4.exe", p4.Executable);
			Assert.AreEqual("//depot/myproject/...", p4.View);
			Assert.AreEqual("myclient", p4.Client);
			Assert.AreEqual("me", p4.User);
			Assert.AreEqual("anotherserver:2666", p4.Port);
			Assert.AreEqual("myWorkingDirectory", p4.WorkingDirectory);
		}

		private P4 CreateP4WithNoArgContructor(string p4root)
		{
			P4 perforce = new P4();
			NetReflector.Read(p4root, perforce);
			return perforce;
		}

		private P4 CreateP4()
		{
			return new P4((ProcessExecutor) processExecutorMock.MockInstance, 
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
			Assert.AreEqual("", p4.Port);
		}

		[Test]
		[ExpectedException(typeof(NetReflectorException))]
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

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00 ";

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

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00 //myotherdepot/proj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00 ";

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
  <port>anotherserver:2666</port>
</sourceControl>
";

			DateTime from = new DateTime(2003, 11, 20, 2, 10, 32);
			DateTime to = new DateTime(2004, 10, 31, 5, 5, 1);

			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me"
				+ " changes -s submitted //depot/myproject/...@2003/11/20:02:10:32,@2004/10/31:05:05:01 ";
			
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
		public void CreateGetDescribeProcessWithSpecifiedArgs() {
			string xml = @"
<sourceControl name=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <port>anotherserver:2666</port>
</sourceControl>
";
			string changes = "3327 3328 332";
			
			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me"
				+ " describe -s " + changes;
			
			P4 p4 = CreateP4WithNoArgContructor(xml);
			ProcessInfo process = p4.CreateDescribeProcess(changes);

			Assert.AreEqual("c:\\bin\\p4.exe", process.FileName);
			Assert.AreEqual(expectedArgs, process.Arguments);
		}

		[Test]
		[ExpectedException(typeof(CruiseControlException))]
		public void CreateGetDescribeProcessWithEvilCode()
		{
			string changes = "3327 3328 332; echo 'rm -rf /'";
			new P4().CreateDescribeProcess(changes);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void CreateGetDescribeProcessWithNoChanges()
		{
			string changes = "";
			new P4().CreateDescribeProcess(changes);
			// this should never happen, but here's a test just in case.
		}

		[Test]
// Owen: I'm tired of looking at this comment in the build log.  The test seems to pass.  Either keep it or delete it.
//		[Ignore("This test is terrible - *never* test classes by testing a mock. Redesign until you can test properly (or use TDD and you won't get this anyway)")]
		public void GetModifications()
		{
			DateTime from = new DateTime(2002, 11, 1);
			DateTime to = new DateTime(2002, 11, 14);

			DynamicMock mock = new DynamicMock(typeof(P4));
			mock.Ignore("GetModifications");
			mock.Ignore("CreateChangeListProcess");

			string changes = @"
info: Change 3328 on 2002/10/31 by someone@somewhere 'Something important '
info: Change 3327 on 2002/10/31 by someone@somewhere 'Joe's test '
info: Change 332 on 2002/10/31 by someone@somewhere 'thingy'
exit: 0
";
			mock.ExpectAndReturn("Execute", changes, new IsTypeOf(typeof(ProcessInfo)));
			mock.ExpectAndReturn("Execute", P4Mother.P4_LOGFILE_CONTENT, new IsAnything()); 
			mock.SetupResult("View", "ViewDataForDodgyUnitTest");

			P4 p4 = (P4)mock.MockInstance;
			Modification[] result = p4.GetModifications(from, to);

			mock.Verify();
			Assert.AreEqual(7, result.Length);
		}

		[Test]
		public void LabelSourceControlIfApplyLabelTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = true;

			string label = "foo-123";

			ProcessInfo labelSpecProcess = new ProcessInfo("spec");
			ProcessInfo labelSpecProcessWithStdInContent = new ProcessInfo("spec");
			labelSpecProcessWithStdInContent.StandardInputContent = @"Label:	foo-123

Description:
	Created by CCNet

Options:	unlocked

View:
 //depot/myproject/...
";
			ProcessInfo labelSyncProcess = new ProcessInfo("sync");

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSpecProcess, p4, "label -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSpecProcessWithStdInContent);
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSyncProcess, p4, "labelsync -l foo-123");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSyncProcess);

			// Execute
			p4.LabelSourceControl(label,null);

			// Verify
			VerifyAll();
		}

		[Test]
		public void LabelSourceControlIfApplyLabelTrueWithMultiLineViews()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproj/...,//myotherdepot/proj/...";
			p4.ApplyLabel = true;

			string label = "foo-123";

			ProcessInfo labelSpecProcess = new ProcessInfo("spec");
			ProcessInfo labelSpecProcessWithStdInContent = new ProcessInfo("spec");
			labelSpecProcessWithStdInContent.StandardInputContent = @"Label:	foo-123

Description:
	Created by CCNet

Options:	unlocked

View:
 //depot/myproj/...
 //myotherdepot/proj/...
";
			ProcessInfo labelSyncProcess = new ProcessInfo("sync");

			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSpecProcess, p4, "label -i");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSpecProcessWithStdInContent);
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", labelSyncProcess, p4, "labelsync -l foo-123");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), labelSyncProcess);

			// Execute
			p4.LabelSourceControl(label,null);

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

			string label = "123";

			try
			{
				p4.LabelSourceControl(label,null);
				Assert.Fail("Perforce labelling should fail if a purely numeric label is attempted to be applied");
			}
			catch (CruiseControlException) { }

			VerifyAll();
		}

		[Test]
		public void DontLabelSourceControlIfApplyLabelFalse()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.ApplyLabel = false;

			string label = "foo-123";

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof(P4), typeof(string));
			processExecutorMock.ExpectNoCall("Execute", typeof(ProcessInfo));
			p4.LabelSourceControl(label,null);

			VerifyAll();
		}

		[Test]
		public void DontLabelSourceControlIfApplyLabelNotSetEvenIfInvalidLabel()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";

			string label = "123";

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof(P4), typeof(string));
			processExecutorMock.ExpectNoCall("Execute", typeof(ProcessInfo));
			p4.LabelSourceControl(label,null);

			VerifyAll();
		}

		[Test]
		public void GetSourceIfGetSourceTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = true;

			ProcessInfo processInfo = new ProcessInfo("getSource");
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "sync");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfo);
			p4.GetSource(new IntegrationResult());

			VerifyAll();
		}

		[Test]
		public void GetForceSourceIfGetSourceTrue()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = true;
			p4.ForceSync = true;

			ProcessInfo processInfo = new ProcessInfo("getSource");
			processInfoCreatorMock.ExpectAndReturn("CreateProcessInfo", processInfo, p4, "sync -f");
			processExecutorMock.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), processInfo);
			p4.GetSource(new IntegrationResult());

			VerifyAll();
		}

		[Test]
		public void DontGetSourceIfGetSourceFalse()
		{
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4.AutoGetSource = false;

			processInfoCreatorMock.ExpectNoCall("CreateProcessInfo", typeof(P4), typeof(string));
			processExecutorMock.ExpectNoCall("Execute", typeof(ProcessInfo));
			p4.GetSource(new IntegrationResult());
			VerifyAll();
		}

		[Test]
		public void ShouldCallInitializerWithGivenWorkingDirectoryIfAlternativeNotSet()
		{
			// Setup
			P4 p4 = CreateP4();
			p4.View = "//depot/myproject/...";
			p4InitializerMock.Expect("Initialize",  p4, "myProject", "workingDirFromProject");
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
			p4InitializerMock.Expect("Initialize",  p4, "myProject", "workingDirFromProject");
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
			p4InitializerMock.Expect("Initialize",  p4, "myProject", "p4sOwnWorkingDirectory");
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
			p4PurgerMock.Expect("Purge",  p4, "workingDirFromProject");
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
			p4PurgerMock.Expect("Purge",  p4, "workingDirFromProject");
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
			p4PurgerMock.Expect("Purge",  p4, "p4sOwnWorkingDirectory");
			projectMock.ExpectNoCall("WorkingDirectory");

			// Execute
			p4.Purge(project);

			// Verify
			VerifyAll();
		}
	}
}
