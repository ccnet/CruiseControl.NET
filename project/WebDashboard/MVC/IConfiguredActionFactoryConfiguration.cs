
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	// NOT CURRENTLY USED - Just a possible implementation of genericising the MVC - likely to be deleted
	public interface IConfiguredActionFactoryConfiguration
	{
		ITypeSpecification GetDefaultActionTypeSpecification();
		ITypeSpecification GetTypeSpecification(string actionName);
	}
}
