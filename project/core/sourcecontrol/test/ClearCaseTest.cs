using System;
using System.Globalization;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class ClearCaseTest
	{
		public static readonly string EXECUTABLE = "cleartool.exe";
		public static readonly string VIEWPATH = @"C:\DOES_NOT_EXIST";
		public static readonly string VIEWNAME = @"VIEWNAME_DOES_NOT_EXIST";
		public static readonly string PROJECT_VOB_NAME = "ProjectVobName";
		public static readonly string USE_BASELINE = false.ToString();
		public static readonly string USE_LABEL = true.ToString();

		public static readonly string XML_STUB =
			@"<sourceControl type=""clearCase"">
    <executable>{0}</executable>
	<viewPath>{1}</viewPath>
    <useBaseline>{2}</useBaseline>
    <useLabel>{3}</useLabel>
    <projectVobName>{4}</projectVobName>
	<viewName>{5}</viewName>
</sourceControl>";

		public static readonly string CLEARCASE_XML = string.Format(XML_STUB,
		                                                            EXECUTABLE,
		                                                            VIEWPATH,
		                                                            USE_BASELINE,
		                                                            USE_LABEL,
		                                                            PROJECT_VOB_NAME,
		                                                            VIEWNAME);

		private ClearCase _clearCase;

		[SetUp]
		protected void Setup()
		{
			_clearCase = new ClearCase();
			NetReflector.Read(CLEARCASE_XML, _clearCase);
		}

		[Test]
		public void CanCreateTemporaryBaselineProcessInfo()
		{
			const string name = "baselinename";
			ProcessInfo info = _clearCase.CreateTempBaselineProcessInfo(name);
			Assert.AreEqual(string.Format("{0} mkbl -view {1} -identical {2}",
			                              EXECUTABLE,
			                              VIEWNAME,
			                              name),
			                info.FileName + " " + info.Arguments);
		}

		[Test]
		public void CanCreateRemoveBaselineProcessInfo()
		{
			ProcessInfo info = _clearCase.CreateRemoveBaselineProcessInfo();

			Assert.AreEqual(string.Format("{0} rmbl -force {1}@\\{2}",
			                              EXECUTABLE,
			                              _clearCase.TempBaseline,
			                              _clearCase.ProjectVobName),
			                info.FileName + " " + info.Arguments);
		}

		[Test]
		public void CanCreateRenameBaselineProcesInfo()
		{
			const string newName = "HiImANewBaselineName";
			ProcessInfo info = _clearCase.CreateRenameBaselineProcessInfo(newName);

			Assert.AreEqual(string.Format("{0} rename baseline:{1}@\\{2} {3}",
			                              EXECUTABLE,
			                              _clearCase.TempBaseline,
			                              _clearCase.ProjectVobName,
			                              newName),
			                info.FileName + " " + info.Arguments);
		}

		[Test]
		[ExpectedException(typeof (NetReflectorException))]
		public void CanCatchInvalidBaselineConfiguration()
		{
			ClearCase clearCase = new ClearCase();
			const string invalidXml = "<sourcecontrol type=\"ClearCase\"><useBaseline>NOT_A_BOOLEAN</useBaseline></sourcecontrol>";
			NetReflector.Read(invalidXml, clearCase);
		}

		[Test]
		[ExpectedException(typeof (CruiseControlException))]
		public void CanValidateBaselineName1()
		{
			const string name = "";
			_clearCase.ValidateBaselineName(name);
		}

		[Test]
		[ExpectedException(typeof (CruiseControlException))]
		public void CanValidateBaselineName2()
		{
			const string name = null;
			_clearCase.ValidateBaselineName(name);
		}

		[Test]
		[ExpectedException(typeof (CruiseControlException))]
		public void CanValidateBaselineName3()
		{
			const string name = "name with spaces";
			_clearCase.ValidateBaselineName(name);
		}

		[Test]
		[ExpectedException(typeof (NetReflectorException))]
		public void CanCatchInvalidLabelConfiguration()
		{
			ClearCase clearCase = new ClearCase();
			const string invalidXml = "<sourcecontrol type=\"ClearCase\"><useLabel>NOT_A_BOOLEAN</useLabel></sourcecontrol>";
			NetReflector.Read(invalidXml, clearCase);
		}

		[Test]
		[ExpectedException(typeof (CruiseControlException))]
		public void CanEnforceProjectVobSetIfBaselineTrue()
		{
			_clearCase.UseBaseline = true;
			_clearCase.ProjectVobName = null;

			_clearCase.LabelSourceControl("foo", null);
		}

		[Test]
		public void CanCreateHistoryProcess()
		{
			DateTime expectedStartDate = DateTime.Parse("02-Feb-2002.05:00:00", CultureInfo.InvariantCulture);

			string expectedArguments = "lshist  -r  -nco -since " + expectedStartDate.ToString(ClearCase.DATETIME_FORMAT) + " -fmt \"%u"
				+ ClearCaseHistoryParser.DELIMITER + "%Vd" + ClearCaseHistoryParser.DELIMITER
				+ "%En" + ClearCaseHistoryParser.DELIMITER
				+ "%Vn" + ClearCaseHistoryParser.DELIMITER + "%o" + ClearCaseHistoryParser.DELIMITER
				+ "!%l" + ClearCaseHistoryParser.DELIMITER + "!%a" + ClearCaseHistoryParser.DELIMITER
				+ "%Nc" + ClearCaseHistoryParser.END_OF_STRING_DELIMITER + "\\n\" " + VIEWPATH;
			ProcessInfo processInfo = _clearCase.CreateHistoryProcessInfo(expectedStartDate, DateTime.Now);
			Assert.AreEqual("cleartool.exe", processInfo.FileName);
			Assert.AreEqual(expectedArguments, processInfo.Arguments);
		}

		[Test]
		public void TestConfig()
		{
			Assert.AreEqual(EXECUTABLE, _clearCase.Executable);
			Assert.AreEqual(VIEWPATH, _clearCase.ViewPath);
			Assert.AreEqual(VIEWNAME, _clearCase.ViewName);
			Assert.AreEqual(USE_BASELINE, _clearCase.UseBaseline.ToString());
			Assert.AreEqual(USE_LABEL, _clearCase.UseLabel.ToString());
			Assert.AreEqual(PROJECT_VOB_NAME, _clearCase.ProjectVobName);
		}

		[Test]
		public void CanCreateLabelType()
		{
			string label = "This-is-a-test";
			ProcessInfo labelTypeProcess = _clearCase.CreateLabelTypeProcessInfo(label);
			Assert.AreEqual(" mklbtype -c \"CRUISECONTROL Comment\" " + label, labelTypeProcess.Arguments);
			Assert.AreEqual("cleartool.exe", labelTypeProcess.FileName);
		}

		[Test]
		public void CanCreateLabelProcess()
		{
			const string label = "This-is-a-test";
			ProcessInfo labelProcess = _clearCase.CreateMakeLabelProcessInfo(label);
			Assert.AreEqual(@" mklabel -recurse " + label + " " + VIEWPATH, labelProcess.Arguments);
			Assert.AreEqual("cleartool.exe", labelProcess.FileName);
		}

		[Test]
		public void CanIgnoreVobError()
		{
			Assert.IsFalse(_clearCase.HasFatalError(ClearCaseMother.VOB_ERROR_ONLY));
		}

		[Test]
		public void CanDetectError()
		{
			Assert.IsTrue(_clearCase.HasFatalError(ClearCaseMother.REAL_ERROR_WITH_VOB));
		}
	}
}