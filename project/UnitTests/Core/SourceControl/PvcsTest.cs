using System;
using System.Globalization;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
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
		public void VerifyDateStringParser()
		{
			DateTime expected = new DateTime(2005, 05, 01, 12, 0, 0, 0);
			DateTime actual = Pvcs.GetDate("May 1 2005 12:00:00", CultureInfo.InvariantCulture);
			Assert.AreEqual(expected, actual);

			expected = new DateTime(2005, 10, 01, 15, 0, 0, 0);
			actual = Pvcs.GetDate("Oct 1 2005 15:00:00", CultureInfo.InvariantCulture);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void VerifyDateParser()
		{
			string expected = "10/31/2001 18:52";
			string actual = Pvcs.GetDateString(new DateTime(2001, 10, 31, 18, 52, 13), CultureInfo.InvariantCulture.DateTimeFormat);
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
			string expected = "Vcs -q -xo\"" + pvcs.LogFile + "\" -xe\"" + pvcs.ErrorFile + "\"  -v\"temp\" \"@" + pvcs.TempFile + "\"";
			string actual = pvcs.CreatePcliContentsForLabeling("temp");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetModifications()
		{
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), new IsAnything());
			mockParser.ExpectAndReturn("Parse", new Modification[] {new Modification(), new Modification()}, new IsAnything(), new IsAnything(), new IsAnything());

			Modification[] mods = pvcs.GetModifications(IntegrationResultMother.CreateSuccessful(new DateTime(2004, 6, 1, 1, 1, 1)), 
				IntegrationResultMother.CreateSuccessful(new DateTime(2004, 6, 1, 2, 2, 2)));
			Assert.AreEqual(2, mods.Length);
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
			string expected = @"-vTestLabel ""fooproject\archives\test\myfile.txt-arc"" ";
			Modification mod = new Modification();
			mod.Version = "1.0";
			mod.FolderName = @"fooproject\archives\test";
			mod.FileName = "myfile.txt-arc";
			string actual = pvcs.CreateIndividualLabelString(mod,"TestLabel");
			Assert.AreEqual(expected, actual);
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

        [Test]
        public void GetLoginIdStringWithoutPassword()
        {
            pvcs.Username = "foo";
            pvcs.Password = "";
            Assert.AreEqual(" -id\"foo\" ", pvcs.GetLogin(false));
        }

		[Test]
		public void GetExeFilenameShouldNotBeRootedIfPathIsNotSpecified()
		{
			Assert.AreEqual("Get.exe", pvcs.GetExeFilename());
		}

		private TimeZone CreateMockTimeZone(bool inDayLightSavings)
		{
			Mock mock = new DynamicMock(typeof (TimeZone));
			mock.ExpectAndReturn("IsDaylightSavingTime", inDayLightSavings, new IsTypeOf(typeof (DateTime)));
			return (TimeZone) mock.MockInstance;
		}
	}
}