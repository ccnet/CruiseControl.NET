using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[TestFixture]
	public class NCoverCoverageTest : CustomAssertion
	{
		private NCoverCoverage _coverage;
		private Mock _processMock;
		private static readonly ProcessResult ExpectedProcessResult = new ProcessResult(String.Empty, String.Empty, 0, false);
		
		private const string NCoverInstrumentPath = @"c:\ncover\bin\ncover-console.exe";
		private const string NCoverReportPath=@"c:\ncover\bin\ncoverreport.exe";
		private const string NRecoverPath=@"c:\ncover\bin\nrecover.exe";
		private const string ReportName="MyReport";
		public NCoverCoverageTest()
		{
		}

		[SetUp]
		public void Init()
		{
			_processMock = new DynamicMock(typeof(ProcessExecutor));		    
			_coverage = new NCoverCoverage((ProcessExecutor) _processMock.MockInstance);
			_coverage.NCoverInstrumentBinPath= NCoverInstrumentPath;
			_coverage.NCoverReportBinPath= NCoverReportPath;

		}
																				  
		[Test]
		public void ShouldRunInstrumenterOnAllFiles()
		{
			string src =@"src\*.cs";
			CollectingConstraint constraint = new CollectingConstraint();
			
			_processMock.ExpectAndReturn("Execute", ExpectedProcessResult, constraint);
			_coverage.SrcFilePath = src;
			_coverage.ReportName = ReportName;
			_coverage.Instrument();
			ProcessInfo processInfo = (ProcessInfo) constraint.Parameter;
			AssertContains(src, processInfo.Arguments);
			AssertContains(_coverage.NCoverInstrumentBinPath, processInfo.FileName);
			AssertContains(ReportName, processInfo.Arguments);
			_processMock.Verify();
		}

		[Test]
		public void ShouldReportToFile()
		{
			CollectingConstraint constraint = new CollectingConstraint();
			_processMock.ExpectAndReturn("Execute", ExpectedProcessResult, constraint);
			_processMock.SetupResult("Execute", ExpectedProcessResult,typeof(ProcessInfo));
			_coverage.ReportName = ReportName;
			_coverage.Report();
			ProcessInfo processInfo = (ProcessInfo) constraint.Parameter;
			AssertContains(ReportName, processInfo.Arguments);
			AssertContains(NCoverReportPath, processInfo.FileName);
			_processMock.Verify();
		}	 

		[Test, Ignore("Need to fix this, and understand to use mocks")]
		public void ShouldCleanupAfterReporting()
		{
			string reportName ="MyReport";
			CollectingConstraint constraint = new CollectingConstraint();
			
			_processMock.SetupResult("Execute", ExpectedProcessResult,typeof(ProcessInfo));
			_processMock.ExpectAndReturn("Execute", ExpectedProcessResult, constraint);

			_coverage.ReportName = reportName;
			_coverage.Report();
			ProcessInfo processInfo = (ProcessInfo) constraint.Parameter;
			AssertContains(reportName, processInfo.Arguments);
			AssertContains(NRecoverPath, processInfo.FileName);
			_processMock.Verify();
		}
	}
}
