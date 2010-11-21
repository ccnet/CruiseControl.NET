using System.Configuration;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public class CruiseControlConfigSectionHandler : IConfigurationSectionHandler
	{
        /// <summary>
        /// Creates the specified parent.	
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The config context.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public virtual object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return null;
		}
	}
}
