
namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectIntegratorListFactory
	{
		IProjectIntegratorList CreateProjectIntegrators(IProjectList projects);
	}
}
