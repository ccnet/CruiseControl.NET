using System;

namespace tw.ccnet.core
{
	public delegate void IntegrationEventHandler(object sender, IntegrationResult result);

	public interface IIntegrationEventHandler
	{
		IntegrationEventHandler IntegrationEventHandler { get ; }
	}
}
