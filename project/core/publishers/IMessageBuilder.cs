using System;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public interface IMessageBuilder
	{
		string BuildMessage(IntegrationResult result, string project);
	}
}
