using System;

namespace tw.ccnet.core
{
	public interface IStateManager
	{
		bool Exists();
		IntegrationResult Load();
		void Save(IntegrationResult result);
	}
}
