using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VssTest : CustomAssertion
	{
		public const string DEFAULT_SS_EXE_PATH = @"C:\Program Files\Microsoft Visual Studio\VSS\win32\ss.exe";
		public const string VSS_XML =
			@"<sourceControl type=""vss"" autoGetSource=""true"">
    <executable>..\tools\vss\ss.exe</executable>
    <ssdir>..\tools\vss</ssdir>
    <project>$/root</project>
    <username>Admin</username>
    <password>admin</password>
	<applyLabel>true</applyLabel>
	<timeout>5</timeout>
	<workingDirectory>C:\temp</workingDirectory>
	<culture>fr-FR</culture>
</sourceControl>";	

		private IMock mockProcessExecutor;
		private IMock mockRegistry;
		private Vss vss;
		
		[SetUp]
		public void SetUp()
		{
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); mockProcessExecutor.Strict = true;
			mockRegistry = new DynamicMock(typeof(IRegistry)); mockProcessExecutor.Strict = true;
			mockRegistry.SetupResult("GetExpectedLocalMachineSubKeyValue", DEFAULT_SS_EXE_PATH, typeof(string), typeof(string));
			vss = new Vss(new VssHistoryParser(new EnglishVssLocale()), (ProcessExecutor) mockProcessExecutor.MockInstance, (IRegistry) mockRegistry.MockInstance);
			vss.Project = "$/fooProject";
			vss.Culture = string.Empty; // invariant culture
			vss.Username = "Admin";
			vss.Password = "admin";
		}
		
		[TearDown]
		public void TearDown()
		{
			mockProcessExecutor.Verify();
			mockRegistry.Verify();
		}

		[Test]
		public void CreateHistoryProcess()
		{	
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = vss.CreateHistoryProcessInfo(from, to);

			string expectedArgs = @"history $/fooProject -R -Vd02/22/2002;20:00~01/21/2001;20:00 -YAdmin,admin -I-Y";				

			Assert.IsNotNull(actual);
			Assert.AreEqual(DEFAULT_SS_EXE_PATH, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}
		
		[Test]
		public void ValuesSet()
		{
			NetReflector.Read(VSS_XML, vss);
			Assert.AreEqual(@"..\tools\vss\ss.exe", vss.Executable);
			Assert.AreEqual(@"admin", vss.Password);
			Assert.AreEqual(@"$/root", vss.Project);
			Assert.AreEqual(@"..\tools\vss", vss.SsDir);
			Assert.AreEqual(@"Admin", vss.Username);
			Assert.AreEqual(true, vss.ApplyLabel);
			Assert.AreEqual(5, vss.Timeout);
			Assert.AreEqual(true, vss.AutoGetSource);
			Assert.AreEqual(@"C:\temp", vss.WorkingDirectory);
			Assert.AreEqual("fr-FR", vss.Culture);
		}

		[Test]
		public void FormatDateInCultureInvariantFormat()
		{
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002;20:00";
			string actual = vss.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002;12:00";
			actual = vss.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			vss.Culture = "en-US";
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "2/22/2002;8:00P";
			string actual = vss.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "2/22/2002;12:00P";
			actual = vss.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateLabelProcess() 
		{
			string oldLabel = "oldLabel";
			string newLabel = "newLabel";

			ProcessInfo actual = vss.CreateLabelProcessInfo(newLabel, oldLabel);

			string expectedArgs = @"label $/fooProject -LnewLabel -VLoldLabel -YAdmin,admin -I-Y";				

			Assert.IsNotNull(actual);
			Assert.AreEqual(DEFAULT_SS_EXE_PATH, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}

		[Test]
		public void CreateLabelProcessForCurrentVersion()
		{
			string label = "testLabel";

			ProcessInfo actual = vss.CreateLabelProcessInfo(label);

			string expectedArgs = @"label $/fooProject -LtestLabel -YAdmin,admin -I-Y";				
			Assert.IsNotNull(actual);
			Assert.AreEqual(DEFAULT_SS_EXE_PATH, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}

		[Test]
		public void StripQuotesFromSSDir()
		{
			vss.SsDir = @"""C:\Program Files\Microsoft Visual Studio\VSS""";
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio\VSS", vss.SsDir);
		}

		[Test]
		public void SSDirEnvironmentVariableValueShouldNotChangeIfSSDirIsNotSpecified()
		{
			ProcessInfo orginal = new ProcessInfo("foo", "bar");

			ProcessInfo actual = vss.CreateHistoryProcessInfo(DateTime.Now, DateTime.Now);
			Assert.AreEqual(orginal.EnvironmentVariables[Vss.SS_DIR_KEY], actual.EnvironmentVariables[Vss.SS_DIR_KEY]);
		}

		[Test]
		public void ReadDefaultExecutableFromRegistry()
		{
			mockRegistry.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio\VSS\win32\SSSCC.DLL", Vss.SS_REGISTRY_PATH, Vss.SS_REGISTRY_KEY);
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio\VSS\win32\ss.exe", vss.Executable);
		}

		[Test]
		public void ShouldWorkWhenStandardErrorIsNull()
		{
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("foo", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false), new IsAnything());
			
			vss.GetModifications(DateTime.Now, DateTime.Now);
		}

		[Test]
		public void ShouldWorkWhenStandardErrorIsNotNullButExitCodeIsZero()
		{
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("foo", "bar", ProcessResult.SUCCESSFUL_EXIT_CODE, false), new IsAnything());
			
			vss.GetModifications(DateTime.Now, DateTime.Now);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void ShouldFailIfProcessTimesOut()
		{
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("x", null, ProcessResult.TIMED_OUT_EXIT_CODE, true), new IsAnything());
		
			vss.GetModifications(DateTime.Now, DateTime.Now);
		}

		[Test]
		public void VerifyGetSourceProcessInfo()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("Getting App.ico", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false), constraint);
			vss.AutoGetSource = true;
			vss.Project = "$/Refactoring";
			vss.Username = "orogers";
			vss.Password = string.Empty;
			vss.WorkingDirectory = @"c:\source\";
			vss.SsDir = @"..\tools\vss";
			vss.GetSource(IntegrationResultMother.CreateSuccessful(DateTime.Now));

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertMatches(@"get \$/Refactoring -R -Vd.* -Yorogers, -I-N", info.Arguments);
			Assert.AreEqual(DEFAULT_SS_EXE_PATH, info.FileName);
			Assert.AreEqual(@"c:\source\", info.WorkingDirectory);
			Assert.AreEqual(@"..\tools\vss", info.EnvironmentVariables[Vss.SS_DIR_KEY]);
		}

		[Test]
		public void OnlyGetSourceIfAutoGetSourceIsSpecified()
		{
			mockProcessExecutor.ExpectNoCall("Execute", typeof(ProcessInfo));

			vss.GetSource(IntegrationResultMother.CreateSuccessful(DateTime.Now));
		}

		[Test]
		public void UseTemporaryDirectoryIfWorkingDirectoryIsNull()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("Getting App.ico", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false), constraint);

			vss.AutoGetSource = true;
			vss.GetSource(IntegrationResultMother.CreateSuccessful(DateTime.Now));

			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertStartsWith(Path.GetTempPath(), info.WorkingDirectory);
		}

		[Test]
		public void LabelNotAppliedByDefault()
		{
			PseudoMockVss vss = new PseudoMockVss();

			vss.CreateTemporaryLabel();

			Assert.AreEqual(0, vss.methodInvoked );
		}

		[Test]
		public void LabelAppliedIfApplyLabelTrue()
		{
			PseudoMockVss vss = new PseudoMockVss();
			vss.ApplyLabel = true;

			vss.CreateTemporaryLabel();

			Assert.AreEqual(1, vss.methodInvoked );
		}

		private class PseudoMockVss : Vss
		{
			public override ProcessInfo CreateLabelProcessInfo(string label)
			{
				methodInvoked++;
				// > cmd.exe /C "exit 0"
				// this should return immediately with a zero exit status
				return new ProcessInfo( "cmd.exe", "/C \"exit 0\"" );
			}

			internal int methodInvoked = 0;
		}
	}
}
