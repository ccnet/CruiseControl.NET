using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationLoader
	{
		IConfiguration Load();
		void Save(IConfiguration config);
	}
}