using System;
using System.Globalization;
using System.IO;
using NMock;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VssTest : CustomAssertion
	{
		public const string VSS_XML =
			@"<sourceControl type=""vss"">
    <executable>..\tools\vss\ss.exe</executable>
    <ssdir>..\tools\vss</ssdir>
    <project>$/fooProject</project>
    <username>Admin</username>
    <password>admin</password>
	<applyLabel>true</applyLabel>
</sourceControl>";	

		private Vss _vss;
		
		[SetUp]
		protected void SetUp()
		{
			_vss = CreateVss();
		}
		
		[Test]
		public void CreateHistoryProcess()
		{	
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = _vss.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"history $/fooProject -R -Vd02/22/2002;20:00~01/21/2001;20:00 -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}
		
		[Test]
		public void ValuesSet()
		{
			AssertEquals(@"..\tools\vss\ss.exe", _vss.Executable);
			AssertEquals(@"admin", _vss.Password);
			AssertEquals(@"$/fooProject", _vss.Project);
			AssertEquals(@"..\tools\vss", _vss.SsDir);
			AssertEquals(@"Admin", _vss.Username);
			AssertEquals("incorrect applyLabel value", true, _vss.ApplyLabel);
		}

		[Test]
		public void FormatDateInCultureInvariantFormat()
		{
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002;20:00";
			string actual = _vss.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002;12:00";
			actual = _vss.FormatCommandDate(date);
			AssertEquals(expected, actual);
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			_vss.CultureInfo = new CultureInfo("en-US");
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "2/22/2002;8:00P";
			string actual = _vss.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "2/22/2002;12:00P";
			actual = _vss.FormatCommandDate(date);
			AssertEquals(expected, actual);
		}

		[Test]
		public void CreateLabelProcess() 
		{
			string label = "testLabel";
			DateTime dateTime = new DateTime(2003, 4, 15, 11, 12, 13, 0);
			ProcessInfo actual = _vss.CreateLabelProcessInfo(label, dateTime);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"label $/fooProject -LtestLabel -Vd04/15/2003;11:12 -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void CreateLabelProcessForCurrentVersion()
		{
			string label = "testLabel";
			ProcessInfo actual = _vss.CreateLabelProcessInfo(label);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"label $/fooProject -LtestLabel -YAdmin,admin -I-Y";				

			Assertion.AssertNotNull("process was null", actual);
			Assertion.AssertEquals(expectedExecutable, actual.FileName);
			Assertion.AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void StripQuotesFromSSDir()
		{
			Vss vss = new Vss();
			vss.SsDir = @"""C:\Program Files\Microsoft Visual Studio\VSS""";
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio\VSS", vss.SsDir);
		}

		[Test]
		public void SSDirEnvironmentVariableValueShouldNotChangeIfSSDirIsNotSpecified()
		{
			ProcessInfo orginal = new ProcessInfo("foo", "bar");

			Vss vss = new Vss();
			vss.Executable = "ss.exe"; // necessary to stop registry setting from being read
			ProcessInfo actual = vss.CreateHistoryProcessInfo(DateTime.Now, DateTime.Now);
			AssertEquals(orginal.EnvironmentVariables[Vss.SS_DIR_KEY], actual.EnvironmentVariables[Vss.SS_DIR_KEY]);
		}

		[Test]
		public void ReadDefaultExecutableFromRegistry()
		{
			IMock mockRegistry = new DynamicMock(typeof(IRegistry));
			mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", @"C:\Program Files\Microsoft Visual Studio\VSS\win32\SSSCC.DLL", Vss.SS_REGISTRY_PATH, Vss.SS_REGISTRY_KEY);

			Vss vss = new Vss();
			AssertEquals(@"C:\Program Files\Microsoft Visual Studio\VSS\win32\ss.exe", vss.GetExecutable((IRegistry)mockRegistry.MockInstance));
			mockRegistry.Verify();
		}

		[Test]
		public void ShouldWorkWhenStandardErrorIsNull()
		{
			Mock mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor));
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 1, false), new NMock.Constraints.IsAnything());
			
			Vss vss = new Vss(new VssHistoryParser(), (ProcessExecutor)mockProcessExecutor.MockInstance);
			vss.Executable = "foo";
			vss.GetModifications(DateTime.Now, DateTime.Now);
		}

		private Vss CreateVss()
		{
			Vss vss = new Vss();
			NetReflector.Read(VSS_XML, vss);
			vss.CultureInfo = CultureInfo.InvariantCulture;
			return vss;
		}
	}
}
