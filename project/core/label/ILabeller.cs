using System;

namespace tw.ccnet.core
{
	public interface ILabeller
	{
		string Generate(IntegrationResult previousLabel);
	}
}
