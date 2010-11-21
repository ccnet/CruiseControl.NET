namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IMessageBuilder
	{

        /// <summary>
        /// The xslFiles that could be used for the transformations done by BuildMessage
        /// </summary>
        System.Collections.IList xslFiles { get; set; }


        /// <summary>
        /// Builds the message.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildMessage(IIntegrationResult result);
	}
}
