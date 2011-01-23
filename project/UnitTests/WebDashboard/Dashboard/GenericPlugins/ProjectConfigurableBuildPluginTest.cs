using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.GenericPlugins
{
	[TestFixture]
	public class ProjectConfigurableBuildPluginTest
	{
		private ProjectConfigurableBuildPlugin plugin;
		private IProjectSpecifier project1 = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "project1");
		private IProjectSpecifier project2 = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "project2");

		[SetUp]
		public void Setup()
		{
			plugin = new StubbedProjectConfigurableBuildPlugin();
		}

		[Test]
		public void ByDefaultWillAlwaysBeAvailableForAnyProject()
		{
			Assert.IsTrue(plugin.IsDisplayedForProject( project2 ));
		}

		[Test]
		public void ShouldNotBeAvailableForANonIncludedProjectIfIncludedProjectsAreSpecified()
		{
			plugin.IncludedProjects = new string[] { project1.ProjectName };
			Assert.IsFalse(plugin.IsDisplayedForProject( project2 ));
		}

		[Test]
		public void ShouldBeAvailableForAnIncludedProjectIfIncludedProjectsAreSpecified()
		{
			plugin.IncludedProjects = new string[] { project1.ProjectName };
			Assert.IsTrue(plugin.IsDisplayedForProject( project1 ));
		}

		[Test]
		public void ShouldNotBeAvailableForAnExcludedProjectIfExcludedProjectsAreSpecified()
		{
			plugin.ExcludedProjects = new string[] { project1.ProjectName };
			Assert.IsFalse(plugin.IsDisplayedForProject( project1 ));
		}

		[Test]
		public void ShouldBeAvailableForANonExcludedProjectIfExcludedProjectsAreSpecified()
		{
			plugin.ExcludedProjects = new string[] { project1.ProjectName };
			Assert.IsTrue(plugin.IsDisplayedForProject( project2 ));
		}

		[Test]
		public void ShouldThrowAnAppropriateExceptionIfBothIncludedAndExcludedProjectsAreSpecified()
		{
			plugin.IncludedProjects = new string[] { project1.ProjectName };
			try
			{
				plugin.ExcludedProjects = new string[] { project2.ProjectName };
				Assert.Fail("Should not be able to set included and excluded projects");
			}
			catch (CruiseControlException e)
			{
				Assert.AreEqual("Invalid configuration - cannot set both Included and Excluded Projects for a Build Plugin", e.Message);
			}

			plugin.IncludedProjects = new string[0];
			plugin.ExcludedProjects = new string[] { project2.ProjectName };
			try
			{
				plugin.IncludedProjects = new string[] { project1.ProjectName };
				Assert.Fail("Should not be able to set included and excluded projects");
			}
			catch (CruiseControlException e)
			{
				Assert.AreEqual("Invalid configuration - cannot set both Included and Excluded Projects for a Build Plugin", e.Message);
			}
		}

		private class StubbedProjectConfigurableBuildPlugin : ProjectConfigurableBuildPlugin
		{
			public override INamedAction[] NamedActions
			{
				get { throw new NotImplementedException("This is a stub class for testing"); }
			}

			public override string LinkDescription
			{
				get { throw new NotImplementedException("This is a stub class for testing"); }
			}
		}
	}
}
