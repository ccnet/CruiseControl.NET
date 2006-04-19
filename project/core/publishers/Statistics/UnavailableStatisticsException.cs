using System;

namespace ThoughtWorks.CruiseControl.Core.publishers.Statistics
{
	public class UnavailableStatisticsException : ApplicationException
	{
		const string format = "Unavailable statistics {0}. Check your statistics publisher configuration";
		public UnavailableStatisticsException(string message) : base(string.Format(format ,message))
		{
		}
	}
}