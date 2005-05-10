using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.UnitTests;
using ThoughtWorks.CruiseControl.UnitTests.Core;

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
			mockParser = new DynamicMock(typeof (IHistoryParser));
			mockExecutor = new DynamicMock(typeof (ProcessExecutor));
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
			Assert.AreEqual("fooproject", pvcs.Project);
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
			string fileName = "pcliScript.pcli";
			ProcessInfo actualProcess = pvcs.CreatePVCSProcessInfo(pvcs.Executable, pvcs.Arguments, fileName);
			string expected = Pvcs.COMMAND + fileName;
			string actual = actualProcess.Arguments;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreatePcliContentsForGettingVLog()
		{
			string expected = "run -xe\"" + pvcs.ErrorFile + "\" -xo\"" + pvcs.LogFile + "\" -q vlog -pr\"" + pvcs.Project + "\"  -z -ds\"beforedate\" -de\"afterdate\" " + pvcs.Subproject;
			string actual = pvcs.CreatePcliContentsForCreatingVLog("beforedate", "afterdate");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreatePcliContentsForLabeling()
		{
			string expected = "Vcs -q \"-xo" + pvcs.LogFile + "\" \"-xe" + pvcs.ErrorFile + "\" -v\"temp\" \"@" + pvcs.TempFile + "\"";
			string actual = pvcs.CreatePcliContentsForLabeling("temp");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetModifications()
		{
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), new IsAnything());
			mockParser.ExpectAndReturn("Parse", new Modification[] {new Modification(), new Modification()}, new IsAnything(), new IsAnything(), new IsAnything());

			Modification[] mods = pvcs.GetModifications(new DateTime(2004, 6, 1, 1, 1, 1), new DateTime(2004, 6, 1, 2, 2, 2));
			Assert.AreEqual(2, mods.Length);
		}

		[Test]
		public void CreatePcliContentsForCopyLabels()
		{
			string expected = "run \"-xo" + pvcs.LogFile + "\" \"-xe" + pvcs.ErrorFile + "\" -y Label -pr\"" + pvcs.Project + "\"  -z -rtemp2 -vtemp1 " + pvcs.Subproject;
			string actual = pvcs.CreatePcliContentsForCopyingLabels("temp1", "temp2");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreatePcliContentsForGet()
		{
			string expected = "run -y -xe\"" + pvcs.ErrorFile + "\" -xo\"" + pvcs.LogFile + "\" -q Get -pr\"" + pvcs.Project + "\"  @\"" + pvcs.TempFile + "\" ";
			string actual = pvcs.CreatePcliContentsForGet();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateIndividualGetString()
		{
			string expected = "-r1.0 \"fooproject\\archives\\test\\myfile.txt-arc\"(\"c:\\source\\test\") ";
			Modification mod = new Modification();
			mod.Version = "1.0";
			mod.FolderName = @"fooproject\archives\test";
			mod.FileName = "myfile.txt-arc";
			string actual = pvcs.CreateIndividualGetString(mod, @"c:\source\test");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateIndividualLabelString()
		{
			string expected = @"-r1.0 ""fooproject\archives\test\myfile.txt-arc"" ";
			Modification mod = new Modification();
			mod.Version = "1.0";
			mod.FolderName = @"fooproject\archives\test";
			mod.FileName = "myfile.txt-arc";
			string actual = pvcs.CreateIndividualLabelString(mod);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void BuildSubProjectListWithOneSubproject()
		{
			pvcs.Subproject = "foo";
			Assert.AreEqual(1, pvcs.SubProjectList.Count);
			Assert.AreEqual("foo", pvcs.SubProjectList[0]);
		}

		[Test]
		public void BuildSubProjectListWithMultipleSubprojects()
		{
			pvcs.Subproject = "/foo /bar /baz";
			Assert.AreEqual(3, pvcs.SubProjectList.Count);
			Assert.AreEqual("/foo", pvcs.SubProjectList[0]);
			Assert.AreEqual("/bar", pvcs.SubProjectList[1]);
		}

		[Test]
		public void GetLoginIdStringWithoutDoubleQuotes()
		{
			pvcs.Username = "foo";
			pvcs.Password = "bar";
			Assert.AreEqual(" -id\"foo\":\"bar\" ", pvcs.GetLogin(false));
		}

		[Test]
		public void GetLoginIdStringWithDoubleQuotes()
		{
			pvcs.Username = "foo";
			pvcs.Password = "bar";
			Assert.AreEqual(" \"\"-id\"foo\":\"bar\"\"\" ", pvcs.GetLogin(true));
		}

		private TimeZone CreateMockTimeZone(bool inDayLightSavings)
		{
			Mock mock = new DynamicMock(typeof (TimeZone));
			mock.ExpectAndReturn("IsDaylightSavingTime", inDayLightSavings, new IsTypeOf(typeof (DateTime)));
			return (TimeZone) mock.MockInstance;
		}
	}
}