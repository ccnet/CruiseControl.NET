using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseActionInstantiator : ITypeInstantiator
	{
		public object GetInstance(ITypeSpecification typeSpecification)
		{
			Type type = ((TypeSpecificationWithType) typeSpecification).Type;
			if (type == typeof(DisplayAddProjectPageAction))
			{
				return new DisplayAddProjectPageAction(
					new AddProjectModelGenerator(
						new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory())), 
					new AddProjectViewBuilder(new DefaultHtmlBuilder()));
			}
			else
			{
				return new SaveNewProjectAction(
					new AddProjectModelGenerator(
						new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory())), 
					new AddProjectViewBuilder(new DefaultHtmlBuilder()),
					new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory()),
					new NetReflectorProjectSerializer());
			}
		}
	}
}
