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
			return new TypeSpecificationWithType(typeof(SaveNewProjectAction));
		}
	}
}
