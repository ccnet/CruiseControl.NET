using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	public class DefaultLabeller : ILabeller
	{
		public static string INITIAL_LABEL = "1";

		public string Generate(IntegrationResult previousResult)
		{
			if (previousResult == null || previousResult.Label == null) return INITIAL_LABEL;

			return (previousResult.Status == IntegrationStatus.Success) ?
				IncrementLabel(previousResult.Label) : previousResult.Label;
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IntegrationResult result)
		{
			result.Label = Generate(result);
		}

		protected string IncrementLabel(string label)
		{
			int newLabel = int.Parse(label);
			newLabel++;
			return newLabel.ToString();
		}
	}
}
