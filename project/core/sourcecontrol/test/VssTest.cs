using System;
using System.Diagnostics;
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

			Process actual = _vss.CreateHistoryProcess(from, to);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"history $/fooProject -R -Vd02/22/2002;20:00~01/21/2001;20:00 -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}
		
		[Test]
		public void ValuesSet()
		{
			AssertEquals(@"..\tools\vss\ss.exe", _vss.Executable);
			AssertEquals(@"admin", _vss.Password);
			AssertEquals(@"$/fooProject", _vss.Project);
			AssertEquals(@"..\tools\vss", _vss.SsDir);
			AssertEquals(@"Admin", _vss.Username);
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
		public void EnvironmentVariables() 
		{
			Process p = ProcessUtil.CreateProcess("cmd.exe", "/C set foo");
			p.StartInfo.EnvironmentVariables["foo"] = "bar";
			TextReader reader = ProcessUtil.ExecuteRedirected(p);
			string result = reader.ReadToEnd();
			p.WaitForExit();

			AssertEquals("foo=bar\r\n", result);
		}

		[Test]
		public void CreateLabelProcess() 
		{
			string label = "testLabel";
			DateTime dateTime = new DateTime(2003, 4, 15, 11, 12, 13, 0);
			Process actual = _vss.CreateLabelProcess(label, dateTime);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"label $/fooProject -LtestLabel -Vd04/15/2003;11:12 -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}

		[Test]
		public void CreateLabelProcessForCurrentVersion()
		{
			string label = "testLabel";
			Process actual = _vss.CreateLabelProcess(label);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"label $/fooProject -LtestLabel -YAdmin,admin -I-Y";				

			Assertion.AssertNotNull("process was null", actual);
			Assertion.AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			Assertion.AssertEquals(expectedArgs, actual.StartInfo.Arguments);
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
			Process orginal = new Process();

			Vss vss = new Vss();
			Process actual = vss.CreateHistoryProcess(DateTime.Now, DateTime.Now);
			AssertEquals(orginal.StartInfo.EnvironmentVariables[Vss.SS_DIR_KEY], actual.StartInfo.EnvironmentVariables[Vss.SS_DIR_KEY]);
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

		private Vss CreateVss()
		{
			Vss vss = new Vss();
			NetReflector.Read(VSS_XML, vss);
			vss.CultureInfo = CultureInfo.InvariantCulture;
			return vss;
		}
	}
}
