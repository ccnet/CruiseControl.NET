using ThoughtWorks.CruiseControl.Core.Tasks;
namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public interface IMessageBuilder
	{

        /// <summary>
        /// the xslFiles that could be used for the transformations done by BuildMessage
        /// </summary>
        System.Collections.IList xslFiles { get; set; }


        string BuildMessage(IIntegrationResult result, TaskContext context);
	}
}
