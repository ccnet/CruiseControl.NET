using System;
using System.Text;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	/// <summary>
	/// Summary description for IterationLabeller.
	/// </summary>
	[ReflectorType("iterationlabeller")]
	public class IterationLabeller : ILabeller
	{
		public static readonly int InitialLabel = 1;
		private readonly int DaysInWeek = 7;

		[ReflectorProperty("prefix", Required=false)]
		public string LabelPrefix = "";

		/// <summary>
		/// Duration of the interation measured in weeks,
		/// default is 2.
		/// </summary>
		[ReflectorProperty("duration", Required=false)]
		public int Duration = 2;

		[ReflectorProperty("releaseStartDate")]
		public DateTime ReleaseStartDate;

		[ReflectorProperty("separator", Required=false)]
		public string Separator = ".";

		/// <summary>
		/// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
		/// </summary>
		/// <param name="result"></param>
		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		public string Generate(IIntegrationResult previousResult)
		{
			if (previousResult == null || previousResult.Label == null)
			{
				return NewLabel(InitialLabel);
			}
			else if (previousResult.Status == IntegrationStatus.Success)
			{
				return NewLabel(IncrementLabel(previousResult.Label));
			}
			else
			{
				return previousResult.Label;
			}
		}

		private string NewLabel(int suffix)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(LabelPrefix);
			if (LabelPrefix != string.Empty && ! LabelPrefix.EndsWith(Separator)) buffer.Append(Separator);
			buffer.Append(CurrentIteration());
			buffer.Append(Separator);
			buffer.Append(suffix);
			return buffer.ToString();
		}

		private int IncrementLabel(string label)
		{
			string iterationPtn = @".*?((\d+)" + Separator.Replace(".", @"\.") + "(\\d+$)).*";
			string iterationLabel = Regex.Replace(label, iterationPtn, "$2");
			int numericIteration = int.Parse(iterationLabel);
			if (numericIteration < CurrentIteration())
			{
				return InitialLabel;
			}
			else
			{
				string numericLabel = Regex.Replace(label, @".*?(\d+$)", "$1");
				int newLabel = int.Parse(numericLabel);
				return newLabel + 1;
			}
		}

		private int CurrentIteration()
		{
			return GetIteration(ReleaseStartDate);
		}

		private int GetIteration(DateTime startDate)
		{
			return GetIteration(startDate, DateTime.Today);
		}

		private int GetIteration(DateTime startDate, DateTime endDate)
		{
			double daysFromStart = (endDate - startDate).TotalDays;
			return (int) daysFromStart/(Duration*DaysInWeek);
		}
	}
}