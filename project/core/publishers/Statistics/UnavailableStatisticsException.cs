using System;

namespace ThoughtWorks.CruiseControl.Core.publishers.Statistics
{
	public class UnavailableStatisticsException : ApplicationException
	{
		public UnavailableStatisticsException(string message) : base(message)
		{
		}
	}
}