using System;
using System.Diagnostics;
using NUnit.Framework;
using tw.ccnet.core.util;
using tw.ccnet.core.test;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class StarTeamTest : CustomAssertion
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
			//AssertNotNull("StarTeam was null", _starteam);
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			Process actual = _starteam.CreateHistoryProcess(from, to);			

			string expectedExecutable = @"..\tools\starteam\stcmd.exe";
			string expectedArgs = "hist -nologo -x -is -filter IO -p \"Admin:admin@10.1.1.64:49201/.NET LAB/CC.NET/starteam-ccnet\" \"*\"";

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.StartInfo.FileName);
			AssertEquals(expectedArgs, actual.StartInfo.Arguments);
		}
		
		[Test]
		public void TestValuesSet()
		{			
			//AssertNotNull("StarTeam was null", _starteam);
			AssertEquals(@"..\tools\starteam\stcmd.exe", _starteam.Executable);
			AssertEquals("Admin", _starteam.Username);
			AssertEquals("admin", _starteam.Password);
			AssertEquals("10.1.1.64", _starteam.Host);
			AssertEquals(49201, _starteam.Port);
			AssertEquals(".NET LAB", _starteam.Project);
			AssertEquals("CC.NET/starteam-ccnet", _starteam.Path);
			
		}

		[Test]
		public void TestFormatDate()
		{
			//AssertNotNull("StarTeam was null", _starteam);
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002 08:00:00 PM";
			string actual = _starteam.FormatCommandDate(date);
			AssertEquals(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002 12:00:00 PM";
			actual = _starteam.FormatCommandDate(date);
			AssertEquals(expected, actual);
		}

		[Test]
		public void TestExecutable_default()
		{
			AssertEquals("stcmd.exe", new StarTeam().Executable);
		}

		[Test]
		public void TestHost_default()
		{
			AssertEquals("127.0.0.1", new StarTeam().Host);
		}

		[Test]
		public void TestPort_default()
		{
			AssertEquals(49201, new StarTeam().Port);
		}

		[Test]
		public void TestPath_default()
		{
			AssertEquals(String.Empty, new StarTeam().Path);
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