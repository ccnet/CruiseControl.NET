namespace tw.ccnet.service.test 
{
	using NUnit.Framework;
	using NMock;
	using System.Configuration.Install;
	using System.ServiceProcess;

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

			CruiseControlInstaller i = new CruiseControlInstaller();			            

			//verify
			mockSPI.Verify();
			mockSI.Verify();		
		}
	}
}
