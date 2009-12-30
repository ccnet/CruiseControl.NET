#pragma warning disable 1591
using System;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class UnavailableStatisticsException : ApplicationException
	{
		const string format = "Unavailable statistics {0}. Check your statistics publisher configuration";
		public UnavailableStatisticsException(string message) : base(string.Format(format ,message))
		{
		}
	}
}