using NUnit.Framework;
using NMock;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ThoughtWorks.CruiseControl.Service.Test 
{
	[TestFixture]
	public class ServiceTest 
	{	
		[Test]
		public void TestInstallService() 
		{
			DynamicMock mockSPI = new DynamicMock(typeof(ServiceProcessInstaller));
			DynamicMock mockSI = new DynamicMock(typeof(ServiceInstaller));
			
			mockSPI.Expect("Account", ServiceAccount.LocalSystem);
			mockSI.Expect("StartType", ServiceStartMode.Manual);

			//execute

			ProjectInstaller i = new ProjectInstaller();			            

			//verify
			mockSPI.Verify();
			mockSI.Verify();		
		}
	}
}
