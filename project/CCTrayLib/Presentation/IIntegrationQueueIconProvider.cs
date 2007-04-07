using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IIntegrationQueueIconProvider
	{
		StatusIcon GetStatusIconForNodeType( IntegrationQueueNodeType integrationQueueNodeType );
	}
}
