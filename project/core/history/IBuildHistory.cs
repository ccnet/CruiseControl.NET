using System;

namespace tw.ccnet.core
{
	public interface IBuildHistory
	{
		bool Exists();
		IntegrationResult Load();
		void Save(IntegrationResult result);
	}
}
