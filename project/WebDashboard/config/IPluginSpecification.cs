using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public interface IPluginSpecification
	{
		string TypeName { get; }
		Type Type { get; }
	}
}
