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
			AssertEquals("d:\temp\nunit-console.exe",_nunitTask.NUnitPath);
			AssertEquals(1,_nunitTask.Assembly.Length);
			AssertEquals("foo.dll",_nunitTask.Assembly[0]);

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

