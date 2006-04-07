using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("multiTrigger")]
	public class MultipleTrigger : ITrigger
	{
		private ITrigger[] triggers;

		public MultipleTrigger(ITrigger[] triggers)
		{
			this.triggers = triggers;
		}

		public MultipleTrigger() : this(new ITrigger[0])
		{}

		[ReflectorProperty("operator", Required=false)]
		public Operator Operator = Operator.Or;

		[ReflectorArray("triggers", Required=false)]
		public ITrigger[] Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		public void IntegrationCompleted()
		{
			foreach (ITrigger trigger in triggers)
			{
				trigger.IntegrationCompleted();
			}
		}

		public DateTime NextBuild
		{
			get
			{
				DateTime earliestDate = DateTime.MaxValue;
				foreach (ITrigger trigger in triggers)
				{
					if (trigger.NextBuild <= earliestDate)
						earliestDate = trigger.NextBuild;
				}
				return earliestDate;
			}
		}

		public IntegrationRequest Fire()
		{
			IntegrationRequest request = null;
			foreach (ITrigger trigger in triggers)
			{
				IntegrationRequest triggerRequest = trigger.Fire();

				if (Operator == Operator.And && triggerRequest == null) return null;

				if (triggerRequest != null)
				{
					if (request == null || triggerRequest.BuildCondition == BuildCondition.ForceBuild)
						request = triggerRequest;
				}
			}
			return request;
		}

	}

	public enum Operator
	{
		Or,
		And
	}
}