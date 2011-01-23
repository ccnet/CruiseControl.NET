using System;
namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// 	
    /// </summary>
	public interface ICruiseManagerFactory
	{
        /// <summary>
        /// Gets the cruise manager.	
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [Obsolete("")]
		ICruiseManager GetCruiseManager(string url);

        /// <summary>
        /// Retrieve an instance of a <see cref="ICruiseServerClient"/> for communicating with
        /// the server via .NET remoting.
        /// </summary>
        /// <param name="url">The URL of the server.</param>
        /// <returns>A new <see cref="ICruiseServerClient"/> instance.</returns>
        ICruiseServerClient GetCruiseServerClient(string url);
	}
}