using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class AddProjectsTest
	{
		// This isn't really a test, just a quick way to invoke and display the
		// dialog for interactive testing
		[Test, Explicit]
		public void ShowDialogForInteractiveTesting()
		{
			AddProjects addProjects = new AddProjects(null, new Project[0]);
			addProjects.GetListOfNewProjects(null);
		}

		[Test]
		public void TheServerListBoxIsPopulatedWithAListOfAllServersCurrentlyConfigured()
		{
			Project[] projects = {
			                     	new Project("tcp://localhost:123/blah", "proj1"),
			                     	new Project("tcp://otherserver:456/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(2, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[0]);
			Assert.AreEqual(projects[1].BuildServer, addProjects.lbServer.Items[1]);
		}

		[Test]
		public void TheServerListBoxIsPopulatedInAlphabeticalOrder()
		{
			Project[] projects = {
			                     	new Project("tcp://b:123/blah", "proj1"),
			                     	new Project("tcp://a:123/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(2, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[1].BuildServer, addProjects.lbServer.Items[0]);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[1]);
		}

		[Test]
		public void DuplicateServersAreIgnoredWhenAddingToTheServerList()
		{
			Project[] projects = {
			                     	new Project("tcp://localhost:123/blah", "proj1"),
			                     	new Project("tcp://localhost:123/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(1, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[0]);
		}

		[Test]
		public void CurrentlyAddedProjectsAreIgnoredWhenServerIsSelected()
		{
			Project[] allProjects = {
			                        	new Project("tcp://localhost:123/blah", "proj1"),
			                        	new Project("tcp://localhost:123/blah", "proj2"),
			                        	new Project("tcp://localhost:123/blah", "proj3"),
			                        };

			Project[] selectedProjects = {
			                             	new Project("tcp://localhost:123/blah", "proj1"),
			                             	new Project("tcp://localhost:123/blah", "proj2"),
			                             };

			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseProjectManagerFactory));
			mockCruiseManagerFactory.ExpectAndReturn("GetProjectList", allProjects, allProjects[0].BuildServer);
			AddProjects addProjects = new AddProjects((ICruiseProjectManagerFactory) mockCruiseManagerFactory.MockInstance, selectedProjects);
			addProjects.lbServer.SelectedIndex = 0;
			Assert.AreEqual(1, addProjects.lbProject.Items.Count);
			Assert.AreEqual("proj3", addProjects.lbProject.Items[0].ToString());
		}
	}
}