#define USE_MOCK

using System;
using System.Globalization;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;

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
</sourceControl>";	

		private IMock mockProcessExecutor;
		private IMock mockRegistry;
		private Vss vss;
		
		[SetUp]
		protected void SetUp()
		{
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); mockProcessExecutor.Strict = true;
			mockRegistry = new DynamicMock(typeof(IRegistry)); mockProcessExecutor.Strict = true;
			mockRegistry.SetupResult("GetExpectedLocalMachineSubKeyValue", DEFAULT_SS_EXE_PATH, typeof(string), typeof(string));
			vss = new Vss(new VssHistoryParser(), (ProcessExecutor) mockProcessExecutor.MockInstance, (IRegistry) mockRegistry.MockInstance);
			vss.CultureInfo = CultureInfo.InvariantCulture;
			vss.Project = "$/fooProject";
			vss.Username = "Admin";
			vss.Password = "admin";
		}
		
		[TearDown]
		protected void TearDown()
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

			AssertNotNull("process was null", actual);
			AssertEquals(DEFAULT_SS_EXE_PATH, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}
		
		[Test]
		public void ValuesSet()
		{
			NetReflector.Read(VSS_XML, vss);
			AssertEquals(@"..\tools\vss\ss.exe", vss.Executable);
			AssertEquals(@"admin", vss.Password);
			AssertEquals(@"$/root", vss.Project);
			AssertEquals(@"..\tools\vss", vss.SsDir);
			AssertEquals(@"Admin", vss.Username);
			AssertEquals("incorrect applyLabel value", true, vss.ApplyLabel);
			AssertEquals(5, vss.Timeout);
			AssertEquals(true, vss.AutoGetSource);
			AssertEquals(@"C:\temp", vss.WorkingDirectory);
		}

		[Test]
		public void FormatDateInCultureInvariantFormat()
		{
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002;20:00";
			string actual = vss.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002;12:00";
			actual = vss.FormatCommandDate(date);
			AssertEquals(expected, actual);
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			vss.CultureInfo = new CultureInfo("en-US");
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "2/22/2002;8:00P";
			string actual = vss.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "2/22/2002;12:00P";
			actual = vss.FormatCommandDate(date);
			AssertEquals(expected, actual);
		}

		[Test]
		public void CreateLabelProcess() 
		{
			string label = "testLabel";
			DateTime dateTime = new DateTime(2003, 4, 15, 11, 12, 13, 0);

			ProcessInfo actual = vss.CreateLabelProcessInfo(label, dateTime);

			string expectedArgs = @"label $/fooProject -LtestLabel -Vd04/15/2003;11:12 -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(DEFAULT_SS_EXE_PATH, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void CreateLabelProcessForCurrentVersion()
		{
			string label = "testLabel";

			ProcessInfo actual = vss.CreateLabelProcessInfo(label);

			string expectedArgs = @"label $/fooProject -LtestLabel -YAdmin,admin -I-Y";				
			AssertNotNull("process was null", actual);
			AssertEquals(DEFAULT_SS_EXE_PATH, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void StripQuotesFromSSDir()
		{
			vss.SsDir = @"""C:\Program Files\Microsoft Visual Studio\VSS""";
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio\VSS", vss.SsDir);
		}

		[Test]
		public void SSDirEnvironmentVariableValueShouldNotChangeIfSSDirIsNotSpecified()
		{
			ProcessInfo orginal = new ProcessInfo("foo", "bar");

			ProcessInfo actual = vss.CreateHistoryProcessInfo(DateTime.Now, DateTime.Now);
			AssertEquals(orginal.EnvironmentVariables[Vss.SS_DIR_KEY], actual.EnvironmentVariables[Vss.SS_DIR_KEY]);
		}

		[Test]
		public void ReadDefaultExecutableFromRegistry()
		{
			mockRegistry.ExpectAndReturn("GetExpectedLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio\VSS\win32\SSSCC.DLL", Vss.SS_REGISTRY_PATH, Vss.SS_REGISTRY_KEY);

			Vss vss = new Vss();
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio\VSS\win32\ss.exe", vss.Executable);
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
#if USE_MOCK
			CollectingConstraint constraint = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("Getting App.ico", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false), constraint);
#else
			Vss vss = new Vss();
#endif
			vss.AutoGetSource = true;
			vss.Project = "$/Refactoring";
			vss.Username = "orogers";
			vss.Password = "";
			vss.WorkingDirectory = @"c:\source\";
			vss.GetSource(IntegrationResultMother.CreateSuccessful(DateTime.Now));

#if USE_MOCK
			ProcessInfo info = (ProcessInfo) constraint.Parameter;
			AssertMatches(@"get \$/Refactoring -R -Vd.* -Yorogers, -I-N", info.Arguments);
			AssertEquals(DEFAULT_SS_EXE_PATH, info.FileName);
			AssertEquals(@"c:\source\", info.WorkingDirectory);
#endif
		}

		[Test]
		public void OnlyGetSourceIfAutoGetSourceIsSpecified()
		{
			mockProcessExecutor.ExpectNoCall("Execute", typeof(ProcessInfo));

			vss.GetSource(IntegrationResultMother.CreateSuccessful(DateTime.Now));
		}

		[Test]
		public void LabelNotAppliedByDefault()
		{
			PseudoMockVss vss = new PseudoMockVss();

			vss.CreateTemporaryLabel();

			AssertEquals( "CreateLabelProcessInfo should not have been invoked", 0, vss.methodInvoked );
		}

		[Test]
		public void LabelAppliedIfApplyLabelTrue()
		{
			PseudoMockVss vss = new PseudoMockVss();
			vss.ApplyLabel = true;

			vss.CreateTemporaryLabel();

			AssertEquals( "CreateLabelProcessInfo should have been invoked once", 1, vss.methodInvoked );
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
