namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseConfiguredActionFactoryConfiguration : IConfiguredActionFactoryConfiguration
	{
		public ITypeSpecification GetDefaultActionTypeSpecification()
		{
			return new TypeSpecificationWithType(typeof(DisplayAddProjectPageAction));
		}

		public ITypeSpecification GetTypeSpecification(string actionName)
		{
			if (actionName == "_action_ViewAllBuilds")
			{
				return new TypeSpecificationWithType(typeof(ViewAllBuildsAction));
			}
			else
			{
				return new TypeSpecificationWithType(typeof(SaveNewProjectAction));	
			}
		}
	}
}
