using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	[TestFixture]
	public class DefaultStatusIconLoaderTest : Assertion
	{
		private DefaultStatusIconLoader _loader;
		private ProjectStatus _status;
		[SetUp]
		public void Init()
		{
			_status = new ProjectStatus(CruiseControlStatus.Unknown,IntegrationStatus.Unknown,ProjectActivity.Unknown,"foo","http://foo.com",DateTime.Now,"");
			_loader = new DefaultStatusIconLoader();    
		}
		
		[Test]
		public void ShouldShowNowBuildingIconWhenBuilding()
		{
			_status.Activity = ProjectActivity.Building;
		    StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.NOW_BUILDING, icon);
		}
		
		[Test]
		public void ShouldShowSuccessIconWhenBuiltSuccessfuly()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Success;
			StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.SUCCESS, icon);
		}
		
		[Test]
		public void ShouldShowFailureIconWhenBuildFailed()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Failure;
			StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.FAILURE, icon);
		}
		
		[Test]
		public void ShouldShowExceptionIconWhenBuildCCIsInErrorState()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Exception;
			StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.EXCEPTION, icon);
		}
		[Test]
		
		public void ShouldShowUnknownIconWhenBuildCCIsInUnknownState()
		{
			StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.UNKNOWN, icon);
		}
		[Test]
		public void ShouldShowFailureIconWhenBuildProjectActivityIsUnknownAndLastBuildWasFailure()
		{
			_status.Activity = ProjectActivity.Unknown;
			_status.BuildStatus = IntegrationStatus.Failure;
			StatusIcon icon = _loader.LoadIcon(_status);
			AssertSame(StatusIcon.FAILURE, icon);
		}
	}

	
}
