using System;
using ThoughtWorks.CruiseControl.Core.Util;
using NUnit.Framework;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[TestFixture]
	public class NunitTaskTest : CustomAssertion
	{
		private NUnitTask _nunitTask;

		[Test]
		public void LoadWithSingleAssemblyNunitPath()
		{
			string xml="<nunit><path>d:\temp\nunit-console.exe</path><assemblies><assembly>foo.dll</assembly></assemblies></nunit>";
			_nunitTask= NetReflector.Read(xml) as NUnitTask;
			Assert.AreEqual("d:\temp\nunit-console.exe",_nunitTask.NUnitPath);
			Assert.AreEqual(1,_nunitTask.Assembly.Length);
			Assert.AreEqual("foo.dll",_nunitTask.Assembly[0]);

		}
		[Test]
		public void LoadWithMultipleAssemblies()
		{
			string xml=@"<nunit>
							 <path>d:\temp\nunit-console.exe</path>
				             <assemblies>
			                     <assembly>foo.dll</assembly>
								 <assembly>bar.dll</assembly>
							</assemblies>
						 </nunit>";

			_nunitTask= NetReflector.Read(xml) as NUnitTask;
			AssertEqualArrays(new string[]{"foo.dll","bar.dll"},_nunitTask.Assembly);
		}
		
	}
}

