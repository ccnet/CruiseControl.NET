using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class PvcsTest : CustomAssertion
	{
		public static string CreateSourceControlXml()
		{
			return @"    <sourceControl type=""pvcs"">
      <executable>..\etc\pvcs\mockpcli.bat</executable>
	  <project>fooproject</project>
	  <subproject>barsub</subproject>
    </sourceControl>
";
		}

		// Daylight savings time bug
		// Comment this test in and forward system time to appropriate date
		// to veryify daylight saving time detected.
		// This was necessary to resolve a bug with PVCS 7.5.1 (would not properly
		// detected modifications during periods where daylight savings was active)
		[Ignore("Date dependent test due to day light savings time bug in PVCS v7.5.1")]
		public void TestDetectedDayLightSavingsTime_PVCSDayLightSavingsBug() 
		{
			Pvcs pvcs = new Pvcs();
			Assert(pvcs.IsDayLightSavings());
		}

		public void TestSubtractAnHour_PVCSDayLightSavingsBug() 
		{
			Pvcs pvcs = new Pvcs();
			DateTime date1 = new DateTime(2000, 1, 1, 1, 0, 0);
			DateTime anHourAgo = new DateTime(2000, 1, 1, 0, 0, 0);
			AssertEquals(anHourAgo, pvcs.SubtractAnHour(date1));
		}

		public void TestValuePopulation()
		{
			Pvcs pvcs = CreatePvcs();
			AssertEquals(@"..\etc\pvcs\mockpcli.bat", pvcs.Executable);
			AssertEquals("fooproject",pvcs.Project);
			AssertEquals("barsub", pvcs.Subproject);
		}

		public void TestCreateProcess()
		{
			Pvcs pvcs = CreatePvcs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			Process actualProcess = pvcs.CreateHistoryProcess(from, DateTime.Now);

			string expected = Pvcs.COMMAND;
			string actual = actualProcess.StartInfo.Arguments;
			AssertEquals(expected, actual);
		}

		private Pvcs CreatePvcs()
		{
			XmlPopulator populator = new XmlPopulator();
			Pvcs pvcs = new Pvcs();
			populator.Populate(XmlUtil.CreateDocumentElement(CreateSourceControlXml()), pvcs);
			return pvcs;
		}
		
		// TODO: stop cmd window from popping up with this test!!!
		[Ignore("Sort out mockpcli stuff")]
		public void TestGetModifications() 
		{
			Pvcs pvcs = CreatePvcs();
			Modification[] mods = pvcs.GetModifications(new DateTime(), new DateTime());
			AssertEquals(2, mods.Length);
			File.Delete(Pvcs.PVCS_LOGOUTPUT_FILE);
			Assert("input file missing", File.Exists(Pvcs.PVCS_INSTRUCTIONS_FILE));
			File.Delete(Pvcs.PVCS_INSTRUCTIONS_FILE);
		}
		
		public void TestCreatePcliContents() 
		{
			Pvcs pvcs = CreatePvcs();
			string actual = pvcs.CreatePcliContents("beforedate", "afterdate");
			string expected = CreateExpectedContents();
			AssertEquals(expected, actual);
		}
		
		private string CreateExpectedContents() 
		{
			return 
@"set -vProject ""fooproject""
set -vSubProject ""barsub""
run ->pvcstemp.txt listversionedfiles -z -aw $Project $SubProject
run -e vlog  ""-xo+epvcsout.txt"" ""-dbeforedate*afterdate"" ""@pvcstemp.txt""
";
		}
	} 
	
}
