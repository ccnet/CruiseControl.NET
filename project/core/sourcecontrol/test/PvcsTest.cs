using System.Text;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class PvcsTest : CustomAssertion
	{
		private IMock mockParser;
		private IMock mockExecutor;
		private Pvcs pvcs;

		[SetUp]
		protected void CreatePvcs()
		{
			mockParser = new DynamicMock(typeof(IHistoryParser));
			mockExecutor = new DynamicMock(typeof(ProcessExecutor));
			pvcs = new Pvcs((IHistoryParser) mockParser.MockInstance, (ProcessExecutor) mockExecutor.MockInstance);
		}

		[TearDown]
		protected void VerifyMocks()
		{
			mockParser.Verify();
			mockExecutor.Verify();
		}

		[Test]
		public void ValuePopulation()
		{
			string xml = @"    <sourceControl type=""pvcs"">
      <executable>..\etc\pvcs\mockpcli.bat</executable>
	  <project>fooproject</project>
	  <subproject>barsub</subproject>
    </sourceControl>
	";

			NetReflector.Read(xml, pvcs);

			Assert.AreEqual(@"..\etc\pvcs\mockpcli.bat", pvcs.Executable);
			Assert.AreEqual("fooproject",pvcs.Project);
			Assert.AreEqual("barsub", pvcs.Subproject);
		}

		// Daylight savings time bug
		// This was necessary to resolve a bug with PVCS 7.5.1 (would not properly
		// detect modifications during periods where daylight savings was active)
		[Test]
		public void AdjustForDayLightSavingsBugDuringDayLightSavings()
		{
			TimeZone timeZoneWhereItIsAlwaysDayLightSavings = CreateMockTimeZone(true);
			pvcs.CurrentTimeZone = timeZoneWhereItIsAlwaysDayLightSavings;
			
			DateTime date = new DateTime(2000, 1, 1, 1, 0, 0);
			DateTime anHourBefore = new DateTime(2000, 1, 1, 0, 0, 0);
			Assert.AreEqual(anHourBefore, pvcs.AdjustForDayLightSavingsBug(date));
		}
		[Test]
		public void AdjustForDayLightSavingsBugOutsideDayLightSavings()
		{			
			TimeZone timeZoneWhereItIsNeverDayLightSavings = CreateMockTimeZone(false);
			pvcs.CurrentTimeZone = timeZoneWhereItIsNeverDayLightSavings;
			
			DateTime date = new DateTime(2000, 1, 1, 1, 0, 0);
			Assert.AreEqual(date, pvcs.AdjustForDayLightSavingsBug(date));
		}

		[Test]
		public void CreateProcess()
		{
			ProcessInfo actualProcess = pvcs.CreatePVCSProcessInfo();
			string expected = Pvcs.COMMAND;
			string actual = actualProcess.Arguments;
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void CreatePcliContentsForGeneratingPvcsTemp() 
		{
			pvcs.Project = "fooproject";
			pvcs.Subproject = "barsub";
			string expected = 				
@"set -vProject ""fooproject""
set -vSubProject ""barsub""
run ->pvcspretemp.txt listversionedfiles -z -aw $Project $SubProject
";

			string actual = pvcs.CreatePcliContentsForGeneratingPvcsTemp();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreatePcliContentsForGettingVLog()
		{
			string expected = @"run -e vlog  ""-xo+epvcsout.txt"" ""-dbeforedate*afterdate"" ""@pvcstemp.txt"" ";
			string actual = pvcs.CreatePcliContentsForCreatingVLog("beforedate", "afterdate");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetModifications() 
		{
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), new IsAnything());
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), new IsAnything());
			mockParser.ExpectAndReturn("Parse", new Modification[] { new Modification(), new Modification() }, new IsAnything(), new IsAnything(), new IsAnything());
			File.CreateText(Pvcs.PVCS_PRETEMPFILE).Close();
			File.CreateText(Pvcs.PVCS_LOGOUTPUT_FILE).Close();

			Modification[] mods = pvcs.GetModifications(new DateTime(2004, 6, 1, 1, 1, 1), new DateTime(2004, 6, 1, 2, 2, 2));
			Assert.AreEqual(2, mods.Length);
			File.Delete(Pvcs.PVCS_LOGOUTPUT_FILE);
			Assert.IsTrue(File.Exists(Pvcs.PVCS_INSTRUCTIONS_FILE));
			File.Delete(Pvcs.PVCS_INSTRUCTIONS_FILE);
			File.Delete(Pvcs.PVCS_PRETEMPFILE);
			File.Delete(Pvcs.PVCS_TEMPFILE);
		}

		[Test]
		public void TransformPvcsTempFile()
		{
			string initial = @"""\\FSHOME\Data\projects\prototype\AbstractCommand.cs-arc(D:\Projects\prototype\AbstractCommand.cs)""
""\\FSHOME\Data\projects\prototype\AbstractModule.cs-arc(D:\Projects\prototype\AbstractModule.cs)""
""\\FSHOME\Data\projects\prototype\AbstractService.cs-arc(D:\Projects\prototype\AbstractService.cs)""
";
			StringBuilder output = new StringBuilder();
			pvcs.TransformVersionedFileList(new StringReader(initial), new StringWriter(output));

			string expected = @"""\\\FSHOME\Data\projects\prototype\AbstractCommand.cs-arc(D:\Projects\prototype\AbstractCommand.cs)""
""\\\FSHOME\Data\projects\prototype\AbstractModule.cs-arc(D:\Projects\prototype\AbstractModule.cs)""
""\\\FSHOME\Data\projects\prototype\AbstractService.cs-arc(D:\Projects\prototype\AbstractService.cs)""
";
			Assert.AreEqual(expected, output.ToString());
		}

		private TimeZone CreateMockTimeZone(bool inDayLightSavings)
		{			
			Mock mock = new DynamicMock(typeof(TimeZone));
			mock.ExpectAndReturn("IsDaylightSavingTime", inDayLightSavings, new NMock.Constraints.IsTypeOf(typeof(DateTime)));
			return (TimeZone) mock.MockInstance;
		}
	}	
}
