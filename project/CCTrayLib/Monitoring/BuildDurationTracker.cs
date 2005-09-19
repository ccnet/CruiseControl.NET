using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class BuildDurationTracker
	{
		private bool isBuildInProgress;
		private DateTimeProvider currentTimeProvider;
		private DateTime timeOfCurrentBuildStart;
		private TimeSpan lastBuildDuration = TimeSpan.MaxValue;


		public BuildDurationTracker()
			: this(new DateTimeProvider())
		{
		}

		public BuildDurationTracker(DateTimeProvider currentTimeProvider)
		{
			this.currentTimeProvider = currentTimeProvider;
		}

		public TimeSpan LastBuildDuration
		{
			get { return lastBuildDuration; }
		}

		public TimeSpan EstimatedTimeRemainingOnCurrentBuild
		{
			get
			{
				if (!isBuildInProgress || lastBuildDuration == TimeSpan.MaxValue)
					return TimeSpan.MaxValue;

				return (timeOfCurrentBuildStart + lastBuildDuration) - currentTimeProvider.Now;
			}
		}

		public bool IsBuildInProgress
		{
			get { return isBuildInProgress; }
		}

		public void OnBuildStart()
		{
			isBuildInProgress = true;
			timeOfCurrentBuildStart = currentTimeProvider.Now;
		}

		public void OnSuccessfulBuild()
		{
			if (isBuildInProgress)
			{
				lastBuildDuration = currentTimeProvider.Now - timeOfCurrentBuildStart;
				isBuildInProgress = false;
			}
		}

	}
}