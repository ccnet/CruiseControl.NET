using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.config
{
	public interface IPluginSpecification
	{
		string TypeName { get; }
		Type Type { get; }
	}
}
