using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.tasks;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[TestFixture]
	public class CoverageTaskTest : Assertion
	{
		private CoverageTask _ncoverTask;
		public CoverageTaskTest()
		{
		}
		[Test]
		public void LoadsNunitTaskAndDevEnvBuilder() 
		{
			string xml =@"<coverageTask>
							<coverage type =""mockCoverage""></coverage>	
							<reportName>foo.html</reportName> 
							<nunit>
								<path>d:\temp\nunit-console.exe</path>
								<assemblies><assembly>foo.dll</assembly></assemblies>
							</nunit>
							<devenv>
								<executable>c:\vs.net\devenv.com</executable>
								<solutionfile>mySolution.sln</solutionfile>
								<configuration>Debug</configuration>
								<buildTimeoutSeconds>4</buildTimeoutSeconds>
							</devenv>
						</coverageTask>";
			_ncoverTask= NetReflector.Read(xml) as CoverageTask;
			AssertNotNull(_ncoverTask);
			AssertNotNull(_ncoverTask.DevEnvBuilder);
			AssertNotNull(_ncoverTask.Nunit);
		}

		[Test]
		public void ShouldInstrumentAndReport()
		{
		    Mock instrumenter = new DynamicMock(typeof(ICoverage));
			instrumenter.Expect("Instrument");
			instrumenter.Expect("Report");
			Mock devEnvBuilder = new DynamicMock(typeof(DevenvBuilder));
			devEnvBuilder.Expect("Run",new IsAnything());
			
			Mock nunit = new DynamicMock(typeof(NUnitTask));
			nunit.Expect("Run",new IsAnything());
			CoverageTask task = new CoverageTask((NUnitTask)nunit.MockInstance, (DevenvBuilder)devEnvBuilder.MockInstance, (ICoverage)instrumenter.MockInstance);
			task.ReportFileName ="foo.txt";
			task.Run(new IntegrationResult("foo"));
			instrumenter.Verify();
			devEnvBuilder.Verify();
			nunit.Verify();
		}
		[Test]
		public void ShouldReportToProjectNameFile()
		{
			Mock instrumenter = new DynamicMock(typeof(ICoverage));
			instrumenter.Expect("Instrument");
			instrumenter.Expect("Report");
			Mock devEnvBuilder = new DynamicMock(typeof(DevenvBuilder));
			devEnvBuilder.Expect("Run",new IsAnything());
			
			Mock nunit = new DynamicMock(typeof(NUnitTask));
			nunit.Expect("Run",new IsAnything());
			CoverageTask task = new CoverageTask((NUnitTask)nunit.MockInstance, (DevenvBuilder)devEnvBuilder.MockInstance, (ICoverage)instrumenter.MockInstance);
			task.Run(new IntegrationResult("foo"));
			instrumenter.Verify();
			devEnvBuilder.Verify();
			nunit.Verify();
		}

		
	    
	}


	[ReflectorType("mockCoverage")]
	public class MockCoverage:ICoverage
	{

		public void Instrument()
		{
			// TODO:  Add MockCoverage.Instrument implementation
		}

		public void Report()
		{
			// TODO:  Add MockCoverage.Report implementation
		}

		public string ReportName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}


	}

}
