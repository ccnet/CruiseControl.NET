using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ProjectStateIconAdaptorTest
	{
		private Mock<IProjectStateIconProvider> mockIconProvider;
		private StubProjectMonitor monitor;
		private IProjectStateIconProvider iconProvider;

		[SetUp]
		public void SetUp()
		{
			monitor = new StubProjectMonitor( "testProject" );

			mockIconProvider = new Mock<IProjectStateIconProvider>(MockBehavior.Strict);

			iconProvider = (IProjectStateIconProvider) this.mockIconProvider.Object;

			this.monitor.ProjectState = ProjectState.Building;

		}

		[Test]
		public void OnCreationTheCurrentStateOfTheIconIsRead()
		{
			StatusIcon icon = new StatusIcon();
			mockIconProvider.Setup(iconProvider => iconProvider.GetStatusIconForState(ProjectState.Building)).Returns(icon).Verifiable();

			ProjectStateIconAdaptor adaptor = new ProjectStateIconAdaptor( monitor, iconProvider );
			Assert.AreSame( icon, adaptor.StatusIcon );

			mockIconProvider.Verify();
		}

		[Test]
		public void WhenTheMonitorPollsTheIconMayBeUpdated()
		{
			StatusIcon icon = new StatusIcon();
			mockIconProvider.Setup(iconProvider => iconProvider.GetStatusIconForState(ProjectState.Building)).Returns(icon).Verifiable();

			ProjectStateIconAdaptor adaptor = new ProjectStateIconAdaptor( monitor, iconProvider );
			Assert.AreSame( icon, adaptor.StatusIcon );

			monitor.ProjectState = ProjectState.Broken;

			StatusIcon icon2 = new StatusIcon();
			mockIconProvider.Setup(iconProvider => iconProvider.GetStatusIconForState(ProjectState.Broken)).Returns(icon2).Verifiable();

			monitor.Poll();

			Assert.AreSame( icon2, adaptor.StatusIcon );
			mockIconProvider.Verify();
		}

		int iconChangedCount;
		
		[Test]
		public void WhenTheStatusIconIsChangedAnEventIsFired()
		{
			iconChangedCount = 0;

			StatusIcon icon = new StatusIcon();
			mockIconProvider.Setup(iconProvider => iconProvider.GetStatusIconForState(ProjectState.Building)).Returns(icon).Verifiable();

			ProjectStateIconAdaptor adaptor = new ProjectStateIconAdaptor( monitor, iconProvider );
			adaptor.IconChanged += new EventHandler(IconChanged);

			Assert.AreEqual(0,iconChangedCount);

			StatusIcon icon2 = new StatusIcon();
			adaptor.StatusIcon = icon2;
			Assert.AreEqual(1,iconChangedCount);

			adaptor.StatusIcon = icon2;
			Assert.AreEqual(1,iconChangedCount);

		}

		private void IconChanged( object sender, EventArgs e )
		{
			iconChangedCount++;
		}
	}

}