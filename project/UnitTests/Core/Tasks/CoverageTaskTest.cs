using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture, Ignore("sreekanth: should this be deleted?  it was not included in the project.")]
	public class CoverageTaskTest
	{
		private CoverageTask _ncoverTask;

		public CoverageTaskTest()
		{}

		[Test]
		public void LoadsNunitTaskAndDevEnvBuilder()
		{
			string xml = @"<coverageTask>
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
			_ncoverTask = NetReflector.Read(xml) as CoverageTask;
			Assert.IsNotNull(_ncoverTask);
			Assert.IsNotNull(_ncoverTask.DevEnvTask);
			Assert.IsNotNull(_ncoverTask.NUnit);
		}

		[Test]
		public void ShouldInstrumentAndReport()
		{
			Mock instrumenter = new DynamicMock(typeof (ICoverage));
			instrumenter.Expect("Instrument");
			instrumenter.Expect("Report");
			Mock devEnvBuilder = new DynamicMock(typeof (DevenvTask));
			devEnvBuilder.Expect("Run", new IsAnything());

			Mock nunit = new DynamicMock(typeof (NUnitTask));
			nunit.Expect("Run", new IsAnything());
			CoverageTask task = new CoverageTask((NUnitTask) nunit.MockInstance, (DevenvTask) devEnvBuilder.MockInstance, (ICoverage) instrumenter.MockInstance);
			task.ReportFileName = "foo.txt";
			task.Run(createIntegrationResult());
			instrumenter.Verify();
			devEnvBuilder.Verify();
			nunit.Verify();
		}

		[Test]
		public void ShouldReportToProjectNameFile()
		{
			Mock instrumenter = new DynamicMock(typeof (ICoverage));
			instrumenter.Expect("Instrument");
			instrumenter.Expect("Report");
			Mock devEnvBuilder = new DynamicMock(typeof (DevenvTask));
			devEnvBuilder.Expect("Run", new IsAnything());

			Mock nunit = new DynamicMock(typeof (NUnitTask));
			nunit.Expect("Run", new IsAnything());
			CoverageTask task = new CoverageTask((NUnitTask) nunit.MockInstance, (DevenvTask) devEnvBuilder.MockInstance, (ICoverage) instrumenter.MockInstance);
			task.Run(createIntegrationResult());
			instrumenter.Verify();
			devEnvBuilder.Verify();
			nunit.Verify();
		}

		private IntegrationResult createIntegrationResult()
		{
			return new IntegrationResult("foo", @"c:\temp");
		}
	}

	[ReflectorType("mockCoverage")]
	public class MockCoverage : ICoverage
	{
		public void Instrument()
		{
			// TODO:  Add MockCoverage.Instrument implementation
		}

		public void Report()
		{
			// TODO:  Add MockCoverage.Report implementation
		}

		public NUnitTask NUnitTask
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string ReportName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

	}

}