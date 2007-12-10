using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NUnitTaskTest : ProcessExecutorTestFixtureBase
	{
		private NUnitTask task;

		[Test]
		public void LoadWithSingleAssemblyNunitPath()
		{
			string xml = @"<nunit>
	<path>d:\temp\nunit-console.exe</path>
	<assemblies>
		<assembly>foo.dll</assembly>
	</assemblies>
	<outputfile>c:\testfile.xml</outputfile>
	<timeout>50</timeout>
</nunit>";
			task = NetReflector.Read(xml) as NUnitTask;
			Assert.AreEqual(@"d:\temp\nunit-console.exe", task.NUnitPath);
			Assert.AreEqual(1, task.Assemblies.Length);
			Assert.AreEqual("foo.dll", task.Assemblies[0]);
			Assert.AreEqual(@"c:\testfile.xml", task.OutputFile);
			Assert.AreEqual(50, task.Timeout);
		}

		[Test]
		public void LoadWithMultipleAssemblies()
		{
			string xml = @"<nunit>
							 <path>d:\temp\nunit-console.exe</path>
				             <assemblies>
			                     <assembly>foo.dll</assembly>
								 <assembly>bar.dll</assembly>
							</assemblies>
						 </nunit>";

			task = NetReflector.Read(xml) as NUnitTask;
			AssertEqualArrays(new string[] {"foo.dll", "bar.dll"}, task.Assemblies);
		}

		[Test]
		public void HandleNUnitTaskFailure()
		{
			CreateProcessExecutorMock(NUnitTask.DefaultPath);
			ExpectToExecuteAndReturnWithMonitor(SuccessfulProcessResult(), new ProcessMonitor());
			IIntegrationResult result = IntegrationResult();
			result.ArtifactDirectory = Path.GetTempPath();

			task = new NUnitTask((ProcessExecutor) mockProcessExecutor.MockInstance);
			task.Assemblies = new string[] {"foo.dll"};
			task.Run(result);

			Assert.AreEqual(1, result.TaskResults.Count);
			Assert.AreEqual(ProcessResultOutput, result.TaskOutput);
			Verify();
		}
	}
}
