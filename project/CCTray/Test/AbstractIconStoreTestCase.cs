using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray.Test
{
	public abstract class AbstractIconStoreTestCase : Assertion
	{

		private ProjectStatus _status;
		private IIconStore _iconStore;

		[SetUp]
		public virtual void Init()
		{
			_status = new ProjectStatus(ProjectIntegratorState.Stopped, IntegrationStatus.Unknown,ProjectActivity.Unknown,"foo","http://foo.com",DateTime.Now,"");
			_iconStore = CreateIconStore();    
		}

	    protected abstract IIconStore CreateIconStore();
		protected abstract void Validate(StatusIcon expectedIcon, StatusIcon actualIcon);

		[Test]
		public void ShouldShowNowBuildingIconWhenBuilding()
		{
			_status.Activity = ProjectActivity.Building;
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.NOW_BUILDING, icon);
		}
		
		[Test]
		public void ShouldShowSuccessIconWhenBuiltSuccessfuly()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Success;
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.SUCCESS, icon);
		}
		
		[Test]
		public void ShouldShowFailureIconWhenBuildFailed()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Failure;
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.FAILURE, icon);
		}
		
		[Test]
		public void ShouldShowExceptionIconWhenBuildCCIsInErrorState()
		{
			_status.Activity = ProjectActivity.Sleeping;
			_status.BuildStatus = IntegrationStatus.Exception;
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.EXCEPTION, icon);
		}
		[Test]
		
		public void ShouldShowUnknownIconWhenBuildCCIsInUnknownState()
		{
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.UNKNOWN, icon);
		}
		[Test]
		public void ShouldShowFailureIconWhenBuildProjectActivityIsUnknownAndLastBuildWasFailure()
		{
			_status.Activity = ProjectActivity.Unknown;
			_status.BuildStatus = IntegrationStatus.Failure;
			StatusIcon icon = _iconStore[_status];
			Validate(ResourceIconStore.FAILURE, icon);
		}
	}
}
