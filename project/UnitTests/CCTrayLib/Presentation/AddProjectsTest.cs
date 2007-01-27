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
			AddProjects addProjects = new AddProjects(null, new CCTrayProject[0]);
			addProjects.GetListOfNewProjects(null);
		}

		[Test]
		public void TheServerListBoxIsPopulatedWithAListOfAllServersCurrentlyConfigured()
		{
			CCTrayProject[] projects = {
			                     	new CCTrayProject("tcp://localhost:123/blah", "proj1"),
			                     	new CCTrayProject("tcp://otherserver:456/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(2, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[0]);
			Assert.AreEqual(projects[1].BuildServer, addProjects.lbServer.Items[1]);
		}

		[Test]
		public void TheServerListBoxIsPopulatedInAlphabeticalOrder()
		{
			CCTrayProject[] projects = {
			                     	new CCTrayProject("tcp://b:123/blah", "proj1"),
			                     	new CCTrayProject("tcp://a:123/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(2, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[1].BuildServer, addProjects.lbServer.Items[0]);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[1]);
		}

		[Test]
		public void DuplicateServersAreIgnoredWhenAddingToTheServerList()
		{
			CCTrayProject[] projects = {
			                     	new CCTrayProject("tcp://localhost:123/blah", "proj1"),
			                     	new CCTrayProject("tcp://localhost:123/blah", "proj2"),
			                     };

			AddProjects addProjects = new AddProjects(null, projects);
			Assert.AreEqual(1, addProjects.lbServer.Items.Count);
			Assert.AreEqual(projects[0].BuildServer, addProjects.lbServer.Items[0]);
		}

		[Test]
		public void CurrentlyAddedProjectsAreIgnoredWhenServerIsSelected()
		{
			CCTrayProject[] allProjects = {
			                        	new CCTrayProject("tcp://localhost:123/blah", "proj1"),
			                        	new CCTrayProject("tcp://localhost:123/blah", "proj2"),
			                        	new CCTrayProject("tcp://localhost:123/blah", "proj3"),
			                        };

			CCTrayProject[] selectedProjects = {
			                             	new CCTrayProject("tcp://localhost:123/blah", "proj1"),
			                             	new CCTrayProject("tcp://localhost:123/blah", "proj2"),
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