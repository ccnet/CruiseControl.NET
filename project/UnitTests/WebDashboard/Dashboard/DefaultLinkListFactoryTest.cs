using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultLinkListFactoryTest
	{
		private Mock<ILinkFactory> linkFactoryMock;
		private DefaultLinkListFactory linkListFactory;

		[SetUp]
		public void Setup()
		{
			linkFactoryMock = new Mock<ILinkFactory>();
			linkListFactory = new DefaultLinkListFactory((ILinkFactory) linkFactoryMock.Object);
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldGenerateBuildLinks()
		{
			string action = "my action";
			IBuildSpecifier buildSpecifier1 = (IBuildSpecifier) new Mock<IBuildSpecifier>().Object;
			IBuildSpecifier buildSpecifier2 = (IBuildSpecifier)new Mock<IBuildSpecifier>().Object;
			IAbsoluteLink link1 = new GeneralAbsoluteLink("link 1");
			IAbsoluteLink link2 = new GeneralAbsoluteLink("link 2");

			linkFactoryMock.Setup(factory => factory.CreateStyledBuildLink(buildSpecifier1, action)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateStyledBuildLink(buildSpecifier2, action)).Returns(link2).Verifiable();

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateStyledBuildLinkList(new IBuildSpecifier[] { buildSpecifier1, buildSpecifier2 }, action);

			Assert.AreEqual(2, returnedLinks.Length);
			Assert.AreEqual(link1, returnedLinks[0]);
			Assert.AreEqual(link2, returnedLinks[1]);

			VerifyAll();
		}

		[Test]
		public void ShouldGenerateBuildLinksAndIdentifySelectedLink()
		{
			string action = "my action";
			IBuildSpecifier buildSpecifier1 = (IBuildSpecifier)new Mock<IBuildSpecifier>().Object;
			IBuildSpecifier buildSpecifier2 = (IBuildSpecifier)new Mock<IBuildSpecifier>().Object;
			IBuildSpecifier selectedBuildSpecifier = buildSpecifier1;
			IAbsoluteLink link1 = new GeneralAbsoluteLink("link 1");
			IAbsoluteLink link2 = new GeneralAbsoluteLink("link 2");

			linkFactoryMock.Setup(factory => factory.CreateStyledSelectedBuildLink(buildSpecifier1, action)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateStyledBuildLink(buildSpecifier2, action)).Returns(link2).Verifiable();

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateStyledBuildLinkList(new IBuildSpecifier[] { buildSpecifier1, buildSpecifier2 }, selectedBuildSpecifier, action);

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

			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier1, action)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier2, action)).Returns(link2).Verifiable();

			IAbsoluteLink[] returnedLinks = linkListFactory.CreateServerLinkList(new IServerSpecifier[] { serverSpecifier1, serverSpecifier2 }, action);

			Assert.AreEqual(2, returnedLinks.Length);
			Assert.AreEqual(link1, returnedLinks[0]);
			Assert.AreEqual(link2, returnedLinks[1]);

			VerifyAll();
		}
	}
}
