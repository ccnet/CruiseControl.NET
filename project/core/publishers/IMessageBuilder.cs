namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public interface IMessageBuilder
	{
		string BuildMessage(IIntegrationResult result);
	}
}
