using System;
using tw.ccnet.remote;

namespace tw.ccnet.core.label
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

		protected string IncrementLabel(string label)
		{
			int newLabel = int.Parse(label);
			newLabel++;
			return newLabel.ToString();
		}
	}
}
