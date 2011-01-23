using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IConfigurationFileLoader
	{
        /// <summary>
        /// Loads the specified file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		IConfiguration Load(FileInfo file);
        /// <summary>
        /// Adds the subfile loaded handler.	
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks></remarks>
	    void AddSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler);
	}
}