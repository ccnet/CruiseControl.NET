using System;

namespace tw.ccnet.core
{
	public interface ITask
	{
		void Run(IntegrationResult result);
	}
}
