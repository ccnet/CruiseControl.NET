using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class CvsTest : CustomAssertion
	{
		private string CreateSourceControlXml(string cvsroot, string branch)
		{
			cvsroot = (cvsroot == null) ? String.Empty : "<cvsroot>" + cvsroot + "</cvsroot>";
			branch = (branch == null) ? String.Empty : "<branch>" + branch + "</branch>";

			return string.Format(
				@"    <sourceControl type=""cvs"" autoGetSource=""true"">
      <executable>..\tools\cvs.exe</executable>
      <workingDirectory>..</workingDirectory>
      {0}
	  {1}
    </sourceControl>
"
				, cvsroot, branch);	
		}

		[Test]
		public void ValuePopulation()
		{
			Cvs cvs = CreateCvs(CreateSourceControlXml("myCvsRoot", "branch"));
			Assert.AreEqual(@"..\tools\cvs.exe", cvs.Executable);
			Assert.AreEqual("..",cvs.WorkingDirectory);
			Assert.AreEqual("myCvsRoot", cvs.CvsRoot);
			Assert.AreEqual("branch", cvs.Branch);
			Assert.AreEqual(true, cvs.AutoGetSource);
		}

		[Test]
		public void CreateProcess()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			ProcessInfo actualProcess = cvs.CreateHistoryProcessInfo(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.Arguments;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void HistoryArgs()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);

			Cvs cvs = new Cvs();
			cvs.CvsRoot = "myCvsRoot";
			cvs.Branch = "branch"; 
			string args = cvs.BuildHistoryProcessInfoArgs(from);
			string expected = string.Format(@"-d myCvsRoot -q log -N ""-d>{0}"" -rbranch", cvs.FormatCommandDate(from));
			Assert.AreEqual(expected, args);
		}

		[Test]
		public void CreateProcess_MissingCvsRoot()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			ProcessInfo actualProcess = cvs.CreateHistoryProcessInfo(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.Arguments;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Executable_default()
		{
			Assert.AreEqual("cvs.exe", new Cvs().Executable);
		}

		[Test]
		public void VerifyDateIsFormatedCorrectly()
		{
			DateTime dt = DateTime.Parse("2003-01-01 01:01:01 GMT", CultureInfo.InvariantCulture);
			Assert.AreEqual("2003-01-01 01:01:01 GMT", CreateCvs().FormatCommandDate(dt));
		}

		[Test]
		public void VerifyProcessInfoForGetSource()
		{
			IMock mockHistoryParser = new DynamicMock(typeof(IHistoryParser));
			IMock mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor));
			CollectingConstraint args = new CollectingConstraint();
			mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("output", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false), args); // ProcessResult Execute(ProcessInfo processInfo)
			IntegrationResult result = new IntegrationResult();
			
			Cvs cvs = new Cvs((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
			cvs.AutoGetSource = true;
			cvs.WorkingDirectory = @"C:\temp";
			cvs.Executable = @"C:\Program Files\TortoiseCVS";
			cvs.GetSource(result);

			ProcessInfo info = (ProcessInfo) args.Parameter;
			Assert.AreEqual(@"C:\temp", info.WorkingDirectory);
			Assert.AreEqual(@"C:\Program Files\TortoiseCVS", info.FileName);
			Assert.AreEqual(@"-q update -d -P -C", info.Arguments);

			mockHistoryParser.Verify();
			mockProcessExecutor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			IMock mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor));
			mockProcessExecutor.ExpectNoCall("Execute", typeof(ProcessInfo));
			IntegrationResult result = new IntegrationResult();
			
			Cvs cvs = new Cvs(null, (ProcessExecutor) mockProcessExecutor.MockInstance);
			cvs.AutoGetSource = false;
			cvs.GetSource(result);

			mockProcessExecutor.Verify();			
		}

		private Cvs CreateCvs()
		{
			return CreateCvs(CreateSourceControlXml(null, null));
		}

		private Cvs CreateCvs(string xml)
		{
			Cvs cvs = new Cvs();
			NetReflector.Read(xml, cvs);
			return cvs;
		}
	} 
	
}
