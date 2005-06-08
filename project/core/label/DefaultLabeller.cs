using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	[ReflectorType("defaultlabeller")]
	public class DefaultLabeller : ILabeller
	{
		public static readonly string INITIAL_LABEL = "1";

		[ReflectorProperty("prefix", Required=false)]
		public string LabelPrefix = string.Empty;

		[ReflectorProperty("incrementOnFailure", Required=false)]
		public bool IncrementOnFailed = false;

		public string Generate(IIntegrationResult previousResult)
		{
			if (previousResult == null || previousResult.Label == null)
			{
				return LabelPrefix + INITIAL_LABEL;
			}
			else if (ShouldIncrementLabel(previousResult))
			{
				return LabelPrefix + IncrementLabel(previousResult.Label);
			}
			else
			{
				return previousResult.Label;
			}
		}

		private bool ShouldIncrementLabel(IIntegrationResult previousResult)
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