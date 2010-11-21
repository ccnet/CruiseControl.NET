
using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
	public class QuietPeriod : IQuietPeriod
	{
		private readonly TimeSpan GracePeriodInWhichItIsNotWorthApplyingTheQuietPeriod = TimeSpan.FromMilliseconds(100);
		private readonly TimeSpan AmountOfTimeInTheFutureToWarnAboutFutureModifications = TimeSpan.FromSeconds(10);
		private readonly TimeSpan AmountOfTimeInTheFutureToSkipQuietPeriod = TimeSpan.FromSeconds(60);
		private readonly DateTimeProvider dtProvider;

		private TimeSpan modificationDelay = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the modification delay seconds.	
        /// </summary>
        /// <value>The modification delay seconds.</value>
        /// <remarks></remarks>
		public double ModificationDelaySeconds
		{
			get { return modificationDelay.TotalSeconds; }
			set { modificationDelay = TimeSpan.FromSeconds(value); }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="QuietPeriod" /> class.	
        /// </summary>
        /// <param name="dtProvider">The dt provider.</param>
        /// <remarks></remarks>
		public QuietPeriod(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="lastBuild">The last build.</param>
        /// <param name="thisBuild">The this build.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult lastBuild, IIntegrationResult thisBuild)
		{
			for (;;)
			{
				Modification[] modifications = GetModificationsWithLogging(sourceControl, lastBuild, thisBuild);
				
				DateTime timeOfThisBuild = thisBuild.StartTime;
				DateTime timeOfLatestModification = GetMostRecentModificationDateTime(modifications);

				TimeSpan timeInTheFutureOfLatestModification = timeOfLatestModification - timeOfThisBuild;
				
				if (timeInTheFutureOfLatestModification > AmountOfTimeInTheFutureToWarnAboutFutureModifications)
				{
					Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"The latest modification is {0:n0} seconds in the future; this probably indicates that the clock of your " +
						"build server is out of sync with your source control server.  This can adversely impact the behaviour of CruiseControl.NET",
					            	timeInTheFutureOfLatestModification.TotalSeconds ));					
				}
				
				if (timeInTheFutureOfLatestModification > AmountOfTimeInTheFutureToSkipQuietPeriod)
				{
					Log.Warning(" -> because this is more than a minute in the future, quiet period processing has been skipped");
					return modifications;
				}
				
				DateTime endOfQuietPeriod = timeOfLatestModification + modificationDelay;
				TimeSpan waitRequiredToReachEndOfQuietPeriod = endOfQuietPeriod - timeOfThisBuild;
				
				if (waitRequiredToReachEndOfQuietPeriod < GracePeriodInWhichItIsNotWorthApplyingTheQuietPeriod)
					return modifications;
								
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"The most recent modification at {0} is within in the modification delay.  Waiting for {1:n1} seconds until {2} before checking again.", 
				                       timeOfLatestModification, waitRequiredToReachEndOfQuietPeriod.TotalSeconds, endOfQuietPeriod));
				
				dtProvider.Sleep(waitRequiredToReachEndOfQuietPeriod);
				thisBuild.StartTime = dtProvider.Now;
			}
		}

		private Modification[] GetModificationsWithLogging(ISourceControl sc, IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = sc.GetModifications(from, to);
			if (modifications == null) 
				modifications = new Modification[0];
			Log.Info(GetModificationsDetectedMessage(modifications));
			return modifications;
		}

		private string GetModificationsDetectedMessage(Modification[] modifications)
		{
			switch (modifications.Length)
			{
				case 0:
					return "No modifications detected.";
				case 1:
					return "1 modification detected.";
				default:
					return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} modifications detected.", modifications.Length);
			}
		}

		private DateTime GetMostRecentModificationDateTime(Modification[] modifications)
		{
			DateTime maxDate = DateTime.MinValue;
			foreach (Modification mod in modifications)
			{
				maxDate = DateUtil.MaxDate(mod.ModifiedTime, maxDate);
			}
			return maxDate;
		}
	}
}