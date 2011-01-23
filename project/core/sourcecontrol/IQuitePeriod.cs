namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
    public interface IQuietPeriod 
    {
        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to);
    }
}
