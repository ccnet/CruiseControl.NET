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
		public void ServiceStart()
		{
			new CCServiceExtension().Start();
		}

		private class CCServiceExtension : CCService
		{
			public void Start()
			{
				OnStart(new string[] { });
			}
		}
	}
}
