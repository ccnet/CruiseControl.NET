using NMock;
using NUnit.Framework;
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
			IActionSpecifier actionSpecifier = new ActionSpecifierWithName("Bob");
			IBuildSpecifier buildSpecifier1 = (IBuildSpecifier) new DynamicMock(typeof(IBuildSpecifier)).MockInstance;
			IBuildSpecifier buildSpecifier2 = (IBuildSpecifier) new DynamicMock(typeof(IBuildSpecifier)).MockInstance;
			IAbsoluteLink link1 = new GeneralAbsoluteLink("link 1");
			IAbsoluteLink link2 = new GeneralAbsoluteLink("link 2");

			linkFactoryMock.ExpectAndReturn("CreateStyledBuildLink", link1, buildSpecifier1, actionSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateStyledBuildLink", link2, buildSpecifier2, actionSpecifier);

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateStyledBuildLinkList(new IBuildSpecifier[] { buildSpecifier1, buildSpecifier2 }, actionSpecifier);

			Assert.AreEqual(2, returnedLinks.Length);
			Assert.AreEqual(link1, returnedLinks[0]);
			Assert.AreEqual(link2, returnedLinks[1]);

			VerifyAll();
		}
	}
}
