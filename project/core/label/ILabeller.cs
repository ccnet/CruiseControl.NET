using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface ILabeller : ITask
	{
		string Generate(IntegrationResult previousLabel);
	}
}
