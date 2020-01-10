using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class DefaultActionTest
	{
		private DefaultAction action;
		private Mock<ILinkFactory> linkFactoryMock;

		[SetUp]
		public void Setup()
		{
			linkFactoryMock = new Mock<ILinkFactory>();
			action = new DefaultAction((ILinkFactory) linkFactoryMock.Object);
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldReturnRedirectToFarmReport()
		{
			IAbsoluteLink link = new GeneralAbsoluteLink("", "http://here");
			linkFactoryMock.Setup(factory => factory.CreateFarmLink("", FarmReportFarmPlugin.ACTION_NAME)).Returns(link).Verifiable();

			IResponse response = action.Execute(null);

			Assert.AreEqual("http://here", ((RedirectResponse) response).Url);
			VerifyAll();
		}
	}
}
