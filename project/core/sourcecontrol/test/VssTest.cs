using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol.test
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
		
		public void TestCreateHistoryProcess()
		{	
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			Process actual = _vss.CreateHistoryProcess(from, to);

			string expectedExecutable = @"..\tools\vss\ss.exe";
			string expectedArgs = @"history $/fooProject -R -Vd02-22-2002;8:00P~01-21-2001;8:00P -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}
		
		public void TestValuesSet()
		{
			AssertEquals(@"..\tools\vss\ss.exe", _vss.Executable);
			AssertEquals(@"admin", _vss.Password);
			AssertEquals(@"$/fooProject", _vss.Project);
			AssertEquals(@"..\tools\vss", _vss.SsDir);
			AssertEquals(@"Admin", _vss.Username);
		}

		public void TestFormatDate()
		{
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02-22-2002;8:00P";
			string actual = _vss.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02-22-2002;12:00P";
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
			string expectedArgs = @"label $/fooProject -LtestLabel -Vd04-15-2003;11:12A -YAdmin,admin -I-Y";				

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}

		public void TestExecutable_default()
		{
			AssertEquals("ss.exe", new Vss().Executable);
		}

		private Vss CreateVss()
		{
			XmlPopulator populator = new XmlPopulator();
			Vss vss = new Vss();
			populator.Populate(XmlUtil.CreateDocumentElement(VSS_XML), vss);
			return vss;
		}
	}
}
