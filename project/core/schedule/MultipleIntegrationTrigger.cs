using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("multipleIntegrationTriggers")]
	public class MultipleIntegrationTrigger : IIntegrationTrigger
	{
		private IIntegrationTrigger[] triggers;

		public MultipleIntegrationTrigger()
		{
			this.triggers = new IIntegrationTrigger[0];
		}

		[ReflectorArray("triggers", Required=false)]
		public IIntegrationTrigger[] Triggers
		{
			get
			{
				return triggers;
			}
			set
			{
				triggers = value;
			}
		}
		
		public BuildCondition ShouldRunIntegration()
		{
			BuildCondition overallCondition = BuildCondition.NoBuild;
			foreach (IIntegrationTrigger trigger in triggers)
			{
				// Assumes ordering of elements of enum
				BuildCondition condition = trigger.ShouldRunIntegration();
				if (condition > overallCondition)
				{
					overallCondition = condition;
				}
			}
			return overallCondition;
		}

		public void IntegrationCompleted()
		{
			foreach (IIntegrationTrigger trigger in triggers)
			{
				trigger.IntegrationCompleted();
			}
		}
	}
}
