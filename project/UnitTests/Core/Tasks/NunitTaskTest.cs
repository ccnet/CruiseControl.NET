using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NUnitTaskTest : CustomAssertion
	{
		private NUnitTask task;

		[Test]
		public void LoadWithSingleAssemblyNunitPath()
		{
			string xml = "<nunit><path>d:\temp\nunit-console.exe</path><assemblies><assembly>foo.dll</assembly></assemblies></nunit>";
			task = NetReflector.Read(xml) as NUnitTask;
			Assert.AreEqual("d:\temp\nunit-console.exe", task.NUnitPath);
			Assert.AreEqual(1, task.Assembly.Length);
			Assert.AreEqual("foo.dll", task.Assembly[0]);

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
			AssertEqualArrays(new string[] {"foo.dll", "bar.dll"}, task.Assembly);
		}
	}
}