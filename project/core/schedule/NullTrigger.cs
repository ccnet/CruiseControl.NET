using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	public class NullTrigger : ITrigger
	{
		public BuildCondition ShouldRunIntegration()
		{
			return BuildCondition.NoBuild;
		}

		public void IntegrationCompleted()
		{
			return;
		}
	}
}
