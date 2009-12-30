#pragma warning disable 1591
using System;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class CruiseControlConfigSectionHandler : IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return null;
		}
	}
}
