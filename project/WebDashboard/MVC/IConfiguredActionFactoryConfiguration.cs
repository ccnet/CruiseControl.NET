
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IConfiguredActionFactoryConfiguration
	{
		ITypeSpecification GetDefaultActionTypeSpecification();
		ITypeSpecification GetTypeSpecification(string actionName);
	}
}
