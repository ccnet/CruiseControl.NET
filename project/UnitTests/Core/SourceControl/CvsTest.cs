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
	public class CvsTest : ProcessExecutorTestFixtureBase
	{
		private Cvs cvs;
		private IMock mockHistoryParser;
		private DateTime from;
		private DateTime to;

		[SetUp]
		protected void CreateCvs()
		{
			mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
			CreateProcessExecutorMock(Cvs.DefaultCvsExecutable);
			cvs = new Cvs((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
			from = new DateTime(2001, 1, 21, 20, 0, 0);
			to = from.AddDays(1);
		}

		[TearDown]
		protected void VerifyAll()
		{
			base.Verify();
			mockHistoryParser.Verify();
		}

		[Test]
		public void PopulateFromXml()
		{
			string xml = @"<sourceControl type=""cvs"" autoGetSource=""true"">
      <executable>..\tools\cvs.exe</executable>
      <workingDirectory>..</workingDirectory>
	  <useHistory>true</useHistory>
      <cvsroot>myCvsRoot</cvsroot>
	  <branch>branch</branch>
    </sourceControl>";
			NetReflector.Read(xml, cvs);
			Assert.AreEqual(@"..\tools\cvs.exe", cvs.Executable);
			Assert.AreEqual("..", cvs.WorkingDirectory);
			Assert.AreEqual("myCvsRoot", cvs.CvsRoot);
			Assert.AreEqual("branch", cvs.Branch);
			Assert.AreEqual(true, cvs.AutoGetSource);
			Assert.AreEqual(true, cvs.UseHistory);
			Assert.AreEqual(true, cvs.CleanCopy);
		}

		[Test]
		public void VerifyLogCommandArgumentsWithoutCvsRoot()
		{
			ExpectToExecuteArguments(string.Format(@"-q log -N -b ""-d>{0}""", cvs.FormatCommandDate(from)));
			ExpectToParseAndReturnNoModifications();

			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void VerifyLogCommandArgsWithCvsRootAndBranch()
		{
			ExpectToExecuteArguments(string.Format(@"-d myCvsRoot -q log -N ""-d>{0}"" -rbranch", cvs.FormatCommandDate(from)));
			ExpectToParseAndReturnNoModifications();

			cvs.CvsRoot = "myCvsRoot";
			cvs.Branch = "branch";
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldBuildCorrectHistoryProcessIfRestrictedLogins()
		{
			ExpectToExecuteArguments(string.Format(@"-d myCvsRoot -q log -N -b ""-d>{0}"" -wexortech -wmonkey", cvs.FormatCommandDate(from)));
			ExpectToParseAndReturnNoModifications();

			cvs.CvsRoot = "myCvsRoot";
			cvs.RestrictLogins = "exortech, monkey";
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test, Ignore("owen - need to resolve workingDirectory")]
		public void VerifyLogCommandArgumentsUsingHistoryWithDirectory()
		{
			ExpectToExecuteArguments(string.Format(@"history -x MAR -a -D ""{0}""", cvs.FormatCommandDate(from)));
			ExpectToExecuteArguments(string.Format(@"-q log -N -l -b ""-d>{0}"" ""{1}""", cvs.FormatCommandDate(from), @"\temp"));
			ExpectToParseAndReturnNoModifications();

			cvs.UseHistory = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test, Ignore("owen - need to resolve workingDirectory")]
		public void VerifyLogCommandArgumentsUsingHistoryWithNoDirectory()
		{
			ExpectToExecuteArguments(string.Format(@"history -x MAR -a -D ""{0}""", cvs.FormatCommandDate(from)));
			ExpectToExecuteArguments(string.Format(@"-q log -N -l -b ""-d>{0}""", cvs.FormatCommandDate(from)));
			ExpectToParseAndReturnNoModifications();

			cvs.UseHistory = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void CvsExeShouldBeDefaultExecutable()
		{
			Assert.AreEqual("cvs.exe", cvs.Executable);
		}

		[Test]
		public void VerifyDateIsFormatedCorrectly()
		{
			DateTime dt = DateTime.Parse("2003-01-01 01:01:01 GMT", CultureInfo.InvariantCulture);
			Assert.AreEqual("2003-01-01 01:01:01 GMT", cvs.FormatCommandDate(dt));
		}

		[Test]
		public void VerifyProcessInfoForGetSource()
		{
			ExpectToExecuteArguments(@"-q update -d -P -C");

			cvs.AutoGetSource = true;
			cvs.CleanCopy = true; // set as default
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetSource(new IntegrationResult());
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			ExpectThatExecuteWillNotBeCalled();

			cvs.AutoGetSource = false;
			cvs.GetSource(new IntegrationResult());
		}

		[Test]
		public void ShouldBuildCorrectLabelProcessInfo()
		{
			ExpectToExecuteArguments("tag ver-foo");
			cvs.LabelOnSuccess = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldUseTagPrefixInLabelSpecificationIfSpecified()
		{
			ExpectToExecuteArguments("tag MyCustomPrefix_foo");

			cvs.TagPrefix = "MyCustomPrefix_";
			cvs.LabelOnSuccess = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldBuildCorrectLabelProcessInfoIfCvsRootIsSpecified()
		{
			ExpectToExecuteArguments("-d myCvsRoot tag ver-foo");
			cvs.CvsRoot = "myCvsRoot";
			cvs.LabelOnSuccess = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		private void ExpectToParseAndReturnNoModifications()
		{
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), from, to);
		}
	}
}