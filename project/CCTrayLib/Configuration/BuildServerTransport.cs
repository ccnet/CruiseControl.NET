namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public enum BuildServerTransport
	{
		Remoting,
		HTTP,
        /// <summary>
        /// The transport will use an extension module.
        /// </summary>
        Extension,
	}
}