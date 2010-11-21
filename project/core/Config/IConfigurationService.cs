using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IConfigurationService
	{

        /// <summary>
        /// Loads this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		IConfiguration Load();
        /// <summary>
        /// Saves the specified configuration.	
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <remarks></remarks>
		void Save(IConfiguration configuration);
        /// <summary>
        /// Adds the configuration update handler.	
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks></remarks>
		void AddConfigurationUpdateHandler(ConfigurationUpdateHandler handler);
        /// <summary>
        /// Adds the configuration subfile loaded handler.	
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks></remarks>
        void AddConfigurationSubfileLoadedHandler(ConfigurationSubfileLoadedHandler handler);
	}

    /// <summary>
    /// 	
    /// </summary>
	public delegate void ConfigurationUpdateHandler();
    /// <summary>
    /// 	
    /// </summary>
    /// <param name="subfile_uri">The subfile_uri.</param>
    public delegate void ConfigurationSubfileLoadedHandler (Uri subfile_uri);
}
