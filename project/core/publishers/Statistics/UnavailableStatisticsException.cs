using System;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// 	
    /// </summary>
	public class UnavailableStatisticsException : ApplicationException
	{
		const string format = "Unavailable statistics {0}. Check your statistics publisher configuration";
        /// <summary>
        /// Initializes a new instance of the <see cref="UnavailableStatisticsException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public UnavailableStatisticsException(string message) : base(string.Format(format ,message))
		{
		}
	}
}