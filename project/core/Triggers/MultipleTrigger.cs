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
		public Operator Operatior = Operator.Or;

		[ReflectorArray("triggers", Required=false)]
		public ITrigger[] Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		public BuildCondition ShouldRunIntegration()
		{
			BuildCondition overallCondition = BuildCondition.NoBuild;
			foreach (ITrigger trigger in triggers)
			{
				BuildCondition condition = trigger.ShouldRunIntegration();

				if (Operatior == Operator.And && condition == BuildCondition.NoBuild) return condition;

				if (condition > overallCondition) // Assumes ordering of elements of enum
				{
					overallCondition = condition;
				}
			}
			return overallCondition;
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
	}

	public enum Operator
	{
		Or,
		And
	}
}