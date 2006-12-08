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
		private IMock mockHistoryDirectoryParser;

		[SetUp]
		protected void CreateCvs()
		{
			mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
			mockHistoryDirectoryParser = new DynamicMock(typeof(CvsHistoryCommandParser));
			CreateProcessExecutorMock(Cvs.DefaultCvsExecutable);
			cvs = new Cvs((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance, (CvsHistoryCommandParser) mockHistoryDirectoryParser.MockInstance);
			from = new DateTime(2001, 1, 21, 20, 0, 0);
			to = from.AddDays(1);
		}

		[TearDown]
		protected void VerifyAll()
		{
			Verify();
			mockHistoryParser.Verify();
			mockHistoryDirectoryParser.Verify();
		}

		const string xml = @"<sourceControl type=""cvs"" autoGetSource=""true"">
      <executable>..\tools\cvs.exe</executable>
      <workingDirectory>..</workingDirectory>
	  <useHistory>true</useHistory>
      <cvsroot>myCvsRoot</cvsroot>
	  <branch>branch</branch>
    </sourceControl>";
		
		[Test]
		public void PopulateFromXml()
		{
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
		public void SerializeToXml()
		{
			NetReflector.Read(xml, cvs);
			string y = NetReflector.Write(cvs);
			XmlUtil.VerifyXmlIsWellFormed(y);
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

		[Test]
		public void VerifyLogCommandArgumentsUsingHistoryWithDirectory()
		{
			ExpectToExecuteArguments(string.Format(@"history -x MAR -a -D ""{0}""", cvs.FormatCommandDate(from)));
			ExpectToExecuteArguments(string.Format(@"-q log -N -l -b ""-d>{0}"" ""{1}""", cvs.FormatCommandDate(from), @"\temp"));
			ExpectToParseAndReturnNoModifications();
			mockHistoryDirectoryParser.ExpectAndReturn("ParseOutputFrom", new string[] { @"\temp"}, ProcessResultOutput);

			cvs.UseHistory = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void VerifyLogCommandArgumentsUsingHistoryWithNoDirectoryDoesNotExecuteLogCommand()
		{
			ExpectToExecuteArguments(string.Format(@"history -x MAR -a -D ""{0}""", cvs.FormatCommandDate(from)));
			mockHistoryDirectoryParser.ExpectAndReturn("ParseOutputFrom", new string[0], ProcessResultOutput);

			cvs.UseHistory = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void VerifyHistoryArgumentUsingCvsRoot()
		{
			ExpectToExecuteArguments(string.Format(@"-d myCvsRoot history -x MAR -a -D ""{0}""", cvs.FormatCommandDate(from)));
			mockHistoryDirectoryParser.ExpectAndReturn("ParseOutputFrom", new string[0], ProcessResultOutput);

			cvs.UseHistory = true;
			cvs.CvsRoot = "myCvsRoot";
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
			cvs.GetSource(IntegrationResult());
		}

		[Test]
		public void VerifyProcessInfoForGetSourceOnBranch()
		{
			ExpectToExecuteArguments(@"-q update -d -P -C -r branch");

			cvs.AutoGetSource = true;
			cvs.Branch = "branch";
			cvs.CleanCopy = true; // set as default
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldUseCvsRootWithGetSource()
		{
			ExpectToExecuteArguments(@"-d myCvsRoot -q update -d -P -C");

			cvs.AutoGetSource = true;
			cvs.CvsRoot = "myCvsRoot";
			cvs.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			ExpectThatExecuteWillNotBeCalled();

			cvs.AutoGetSource = false;
			cvs.GetSource(IntegrationResult());
		}
		
		[Test]
		public void ShouldRebaseWorkingDirectoryForGetSource()
		{
			DefaultWorkingDirectory = @"c:\devl\myproject";
			ExpectToExecuteArguments(@"-q update -d -P -C");

			cvs.AutoGetSource = true;
			cvs.CleanCopy = true; // set as default
			cvs.WorkingDirectory = "myproject";
			IntegrationResult result = new IntegrationResult();
			result.WorkingDirectory = @"c:\devl";
			cvs.GetSource(result);			
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

		[Test]
		public void ShouldConvertLabelsThatContainIllegalCharacters()
		{
			ExpectToExecuteArguments("-d myCvsRoot tag ver-2_1_4");
			cvs.CvsRoot = "myCvsRoot";
			cvs.LabelOnSuccess = true;
			cvs.WorkingDirectory = DefaultWorkingDirectory;
			cvs.LabelSourceControl(IntegrationResultMother.CreateSuccessful("2.1.4"));			
		}

		private void ExpectToParseAndReturnNoModifications()
		{
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), from, to);
		}
	}
}