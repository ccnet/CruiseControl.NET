using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	[ReflectorType("defaultlabeller")]
	public class DefaultLabeller : ILabeller
	{
		public const string INITIAL_LABEL = "1";

		[ReflectorProperty("prefix", Required=false)]
		public string LabelPrefix = string.Empty;

		[ReflectorProperty("incrementOnFailure", Required=false)]
		public bool IncrementOnFailed = false;

		public virtual string Generate(IIntegrationResult integrationResult)
		{
			IntegrationSummary lastIntegration = integrationResult.LastIntegration;
			if (integrationResult == null || lastIntegration.IsInitial())
			{
				return LabelPrefix + INITIAL_LABEL;
			}
			else if (ShouldIncrementLabel(lastIntegration))
			{
				return LabelPrefix + IncrementLabel(lastIntegration.Label);
			}
			else
			{
				return integrationResult.LastIntegration.Label;
			}
		}

		private bool ShouldIncrementLabel(IntegrationSummary previousResult)
		{
			return previousResult.Status == IntegrationStatus.Success || IncrementOnFailed;
		}

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		private string IncrementLabel(string label)
		{
			string numericLabel = Regex.Replace(label, @".*?(\d+$)", "$1");
			int newLabel = int.Parse(numericLabel);
			newLabel++;
			return newLabel.ToString();
		}
	}
}