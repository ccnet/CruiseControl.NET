using System;

namespace tw.ccnet.core
{
	public delegate void IntegrationCompletedEventHandler(object sender, IntegrationResult result);
	public delegate void IntegrationExceptionEventHandler(object sender, CruiseControlException ex);

	public interface IIntegrationCompletedEventHandler
	{
		IntegrationCompletedEventHandler IntegrationCompletedEventHandler { get ; }
	}

	public interface IIntegrationExceptionEventHandler
	{
		IntegrationExceptionEventHandler IntegrationExceptionEventHandler { get ; }
	}
}
