using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class CoverageTaskIntegrationTest
	{
		private object _coverageTask;

		[Test, Ignore("Works when in config, but not in test")]
		public void LoadsNunitTaskAndDevEnvBuilder() 
		{
			string xml =@"<build type=""coverage"">
							<coverage type=""ncover"">
								<ncoverBin>C:\sree\progs\ncover-1.0.1\ncover-console\bin\Debug\ncover-console.exe</ncoverBin>
								<ncoverReportBin>C:\sree\progs\ncover-1.0.1\ncoverreport\bin\Debug\ncoverreport.exe</ncoverReportBin>
								<nrecoverBin>C:\sree\progs\ncover-1.0.1\nrecover\bin\Debug\nrecover.exe</nrecoverBin>
								<source>\sree\code\testsvn\*.cs</source>
							</coverage>
							<nunit>
								<path>C:\Program Files\NUnit V2.1\bin\nunit-console.exe</path>
								<assemblies><assembly>C:\sree\code\TestSVN\bin\Debug\TestSVNApp.exe</assembly></assemblies>
							</nunit>
							<devenv>
								<executable>C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe</executable>
								<solutionfile>C:\sree\code\TestSVN\TestSVNApp.sln</solutionfile>
								<configuration>Debug</configuration>
								<buildTimeoutSeconds>10</buildTimeoutSeconds>
							</devenv>
							<reportName>foo.html</reportName> 
						</build>";
			_coverageTask= NetReflector.Read(xml) ;
			((ITask)_coverageTask).Run(new IntegrationResult("foo", @"c:\temp"));
		}
	}
}
