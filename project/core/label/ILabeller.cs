using System;

namespace tw.ccnet.core
{
	public interface ILabeller : ITask
	{
		string Generate(IntegrationResult previousLabel);
	}
}
