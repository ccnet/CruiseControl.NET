using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	[ReflectorType("defaultlabeller")]
	public class DefaultLabeller : ILabeller
	{
		private string _labelPrefix = "";

		public static string INITIAL_LABEL = "1";

		[ReflectorProperty("prefix", Required=false)]
		public string LabelPrefix
		{
			get { return _labelPrefix; }
			set { _labelPrefix = value; }
		}

		public string Generate(IIntegrationResult previousResult)
		{
			if (previousResult == null || previousResult.Label == null)
			{
				return _labelPrefix + INITIAL_LABEL;
			}
			else if(previousResult.Status == IntegrationStatus.Success)
			{
				return _labelPrefix + IncrementLabel(previousResult.Label);
			} 
			else
			{
				return previousResult.Label;
			}
		}

		public bool ShouldRun(IIntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		public string IncrementLabel(string label)
		{
			string numericLabel = Regex.Replace(label,@".*?(\d+$)", "$1");
			int newLabel = int.Parse(numericLabel);
			newLabel++;
			return newLabel.ToString();
		}
	}
}
