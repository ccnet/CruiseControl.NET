using System;
using System.Diagnostics;
using NUnit.Framework;
using tw.ccnet.core.util;
using tw.ccnet.core.test;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class StarTeamTest 
	{
		public const string ST_XML =
			@"<sourceControl type=""starteam"">
				<executable>..\tools\starteam\stcmd.exe</executable>
				<username>Admin</username>
				<password>admin</password>
				<host>10.1.1.64</host>
				<port>49201</port>
				<project>.NET LAB</project>
				<path>CC.NET/starteam-ccnet</path>				
			</sourceControl>";	

		private StarTeam _starteam;

		[SetUp]
		protected void SetUp()
		{
			_starteam = CreateStarTeam();
		}
		
		[Test]
		public void TestCreateHistoryProcess()
		{				
			//Assertion.AssertNotNull("StarTeam was null", _starteam);
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			Process actual = _starteam.CreateHistoryProcess(from, to);			

			string expectedExecutable = @"..\tools\starteam\stcmd.exe";
			string expectedArgs = "hist -nologo -x -is -filter IO -p \"Admin:admin@10.1.1.64:49201/.NET LAB/CC.NET/starteam-ccnet\" \"*\"";

			Assertion.AssertNotNull("process was null", actual);
			Assertion.AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			Assertion.AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}
		
		[Test]
		public void TestValuesSet()
		{			
			//Assertion.AssertNotNull("StarTeam was null", _starteam);
			Assertion.AssertEquals(@"..\tools\starteam\stcmd.exe", _starteam.Executable);
			Assertion.AssertEquals("Admin", _starteam.Username);
			Assertion.AssertEquals("admin", _starteam.Password);
			Assertion.AssertEquals("10.1.1.64", _starteam.Host);
			Assertion.AssertEquals(49201, _starteam.Port);
			Assertion.AssertEquals(".NET LAB", _starteam.Project);
			Assertion.AssertEquals("CC.NET/starteam-ccnet", _starteam.Path);
			
		}

		[Test]
		public void TestFormatDate()
		{
			//Assertion.AssertNotNull("StarTeam was null", _starteam);
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002 08:00:00 PM";
			string actual = _starteam.FormatCommandDate(date);
			Assertion.AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002 12:00:00 PM";
			actual = _starteam.FormatCommandDate(date);
			Assertion.AssertEquals(expected, actual);
		}

		[Test]
		public void TestExecutable_default()
		{
			Assertion.AssertEquals("stcmd.exe", new StarTeam().Executable);
		}

		[Test]
		public void TestHost_default()
		{
			Assertion.AssertEquals("127.0.0.1", new StarTeam().Host);
		}

		[Test]
		public void TestPort_default()
		{
			Assertion.AssertEquals(49201, new StarTeam().Port);
		}

		[Test]
		public void TestPath_default()
		{
			Assertion.AssertEquals(String.Empty, new StarTeam().Path);
		}
		
		private StarTeam CreateStarTeam()
		{
			XmlPopulator populator = new XmlPopulator();
			StarTeam starTeam = new StarTeam();
			populator.Populate(XmlUtil.CreateDocumentElement(ST_XML), starTeam);
			return starTeam;
		}
	} 	
}