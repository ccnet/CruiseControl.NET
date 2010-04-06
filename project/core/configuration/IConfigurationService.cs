using System;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationService
	{

		IConfiguration Load();
		void Save(IConfiguration configuration);
		void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler);
        void AddConfigurationSubfileLoadedHandler(ConfigurationSubfileLoadedHandler handler);
	}

	public delegate void ConfigurationUpdateHandler();
    public delegate void ConfigurationSubfileLoadedHandler (Uri subfile_uri);
}
