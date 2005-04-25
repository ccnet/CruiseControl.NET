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
		public string LabelPrefix = "";

		public string Generate(IIntegrationResult previousResult)
		{
			if (previousResult == null || previousResult.Label == null)
			{
				return LabelPrefix + INITIAL_LABEL;
			}
			else if (previousResult.Status == IntegrationStatus.Success)
			{
				return LabelPrefix + IncrementLabel(previousResult.Label);
			}
			else
			{
				return previousResult.Label;
			}
		}

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		public string IncrementLabel(string label)
		{
			string numericLabel = Regex.Replace(label, @".*?(\d+$)", "$1");
			int newLabel = int.Parse(numericLabel);
			newLabel++;
			return newLabel.ToString();
		}
	}
}