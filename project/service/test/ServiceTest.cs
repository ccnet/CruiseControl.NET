using NUnit.Framework;

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
