using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
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
		// This was necessary to resolve a bug with PVCS 7.5.1 (would not properly
		// detect modifications during periods where daylight savings was active)
		[Test]
		public void AdjustForDayLightSavingsBugDuringDayLightSavings()
		{
			Pvcs pvcs = new Pvcs();
			TimeZone timeZoneWhereItIsAlwaysDayLightSavings = CreateMockTimeZone(true);
			pvcs.CurrentTimeZone = timeZoneWhereItIsAlwaysDayLightSavings;
			
			DateTime date = new DateTime(2000, 1, 1, 1, 0, 0);
			DateTime anHourBefore = new DateTime(2000, 1, 1, 0, 0, 0);
			AssertEquals(anHourBefore, pvcs.AdjustForDayLightSavingsBug(date));
		}
		[Test]
		public void AdjustForDayLightSavingsBugOutsideDayLightSavings()
		{			
			Pvcs pvcs = new Pvcs();
			TimeZone timeZoneWhereItIsNeverDayLightSavings = CreateMockTimeZone(false);
			pvcs.CurrentTimeZone = (TimeZone) timeZoneWhereItIsNeverDayLightSavings;
			
			DateTime date = new DateTime(2000, 1, 1, 1, 0, 0);
			AssertEquals(date, pvcs.AdjustForDayLightSavingsBug(date));
		}

		[Test]
		public void ValuePopulation()
		{
			Pvcs pvcs = CreatePvcs();
			AssertEquals(@"..\etc\pvcs\mockpcli.bat", pvcs.Executable);
			AssertEquals("fooproject",pvcs.Project);
			AssertEquals("barsub", pvcs.Subproject);
		}

		[Test]
		public void CreateProcess()
		{
			Pvcs pvcs = CreatePvcs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			ProcessInfo actualProcess = pvcs.CreateHistoryProcessInfo(from, DateTime.Now);

			string expected = Pvcs.COMMAND;
			string actual = actualProcess.Arguments;
			AssertEquals(expected, actual);
		}

		private Pvcs CreatePvcs()
		{
			Pvcs pvcs = new Pvcs();
			NetReflector.Read(CreateSourceControlXml(), pvcs);
			return pvcs;
		}
		
		// TODO: stop cmd window from popping up with this test!!!
		[Test]
		[Ignore("Sort out mockpcli stuff")]
		public void GetModifications() 
		{
			Pvcs pvcs = CreatePvcs();
			Modification[] mods = pvcs.GetModifications(new DateTime(), new DateTime());
			AssertEquals(2, mods.Length);
			File.Delete(Pvcs.PVCS_LOGOUTPUT_FILE);
			Assert("input file missing", File.Exists(Pvcs.PVCS_INSTRUCTIONS_FILE));
			File.Delete(Pvcs.PVCS_INSTRUCTIONS_FILE);
		}
		
		[Test]
		public void CreatePcliContents() 
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

		private TimeZone CreateMockTimeZone(Boolean inDayLightSavings)
		{			
			Mock mock = new DynamicMock(typeof(TimeZone));
			mock.ExpectAndReturn("IsDaylightSavingTime", inDayLightSavings);
			return (TimeZone) mock.MockInstance;
		}
	}	
}
