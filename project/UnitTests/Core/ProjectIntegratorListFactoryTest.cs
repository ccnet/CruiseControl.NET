using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectIntegratorListFactoryTest : Assertion
	{
		[Test]
		public void CreatesProjectIntegrators()
		{
			// Setup
			Project project1 = new Project();
			project1.Name = "Project 1";
			Project project2 = new Project();
			project2.Name = "Project 2";
			ProjectList projectList = new ProjectList();
			projectList.Add(project1);
			projectList.Add(project2);

			// Execute
			IProjectIntegratorList integrators = new ProjectIntegratorListFactory().CreateProjectIntegrators(projectList);

			// Verify
			AssertEquals(2, integrators.Count);
			AssertEquals(project1, integrators["Project 1"].Project );
			AssertEquals(project2, integrators["Project 2"].Project );
		}
	}
}
