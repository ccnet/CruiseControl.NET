using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultLinkListFactoryTest
	{
		private DynamicMock linkFactoryMock;
		private DefaultLinkListFactory linkListFactory;

		[SetUp]
		public void Setup()
		{
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			linkListFactory = new DefaultLinkListFactory((ILinkFactory) linkFactoryMock.MockInstance);
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldGenerateBuildLinks()
		{
			string action = "my action";
			IBuildSpecifier buildSpecifier1 = (IBuildSpecifier) new DynamicMock(typeof(IBuildSpecifier)).MockInstance;
			IBuildSpecifier buildSpecifier2 = (IBuildSpecifier) new DynamicMock(typeof(IBuildSpecifier)).MockInstance;
			IAbsoluteLink link1 = new GeneralAbsoluteLink("link 1");
			IAbsoluteLink link2 = new GeneralAbsoluteLink("link 2");

			linkFactoryMock.ExpectAndReturn("CreateStyledBuildLink", link1, buildSpecifier1, action);
			linkFactoryMock.ExpectAndReturn("CreateStyledBuildLink", link2, buildSpecifier2, action);

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateStyledBuildLinkList(new IBuildSpecifier[] { buildSpecifier1, buildSpecifier2 }, action);

			Assert.AreEqual(2, returnedLinks.Length);
			Assert.AreEqual(link1, returnedLinks[0]);
			Assert.AreEqual(link2, returnedLinks[1]);

			VerifyAll();
		}

		[Test]
		public void ShouldGenerateServerLinks()
		{
			string action = "ViewServerReport";
			IServerSpecifier serverSpecifier1 = new DefaultServerSpecifier("myserver");
			IServerSpecifier serverSpecifier2 = new DefaultServerSpecifier("myotherserver");

			IAbsoluteLink link1 = new GeneralAbsoluteLink("link 1");
			IAbsoluteLink link2 = new GeneralAbsoluteLink("link 2");

			linkFactoryMock.ExpectAndReturn("CreateServerLink", link1, serverSpecifier1, action);
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier2, action);

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateServerLinkList(new IServerSpecifier[] { serverSpecifier1, serverSpecifier2 }, action);

			Assert.AreEqual(2, returnedLinks.Length);
			Assert.AreEqual(link1, returnedLinks[0]);
			Assert.AreEqual(link2, returnedLinks[1]);

			VerifyAll();
		}
	}
}
