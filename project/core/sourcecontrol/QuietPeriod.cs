using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public interface IQuietPeriod
	{
		Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to);
	}

	public class QuietPeriod : IQuietPeriod
	{
		public const int TurnOffQuietPeriod = 0;
		public double ModificationDelaySeconds = TurnOffQuietPeriod;
		private readonly DateTimeProvider dtProvider;

		public QuietPeriod(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		public Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = GetMods(sourceControl, from, to);
			DateTime nextBuildTime = to.StartTime;
			while (ModificationsAreDetectedInQuietPeriod(modifications, nextBuildTime))
			{
				int secondsUntilNextBuild = SecondsUntilNextBuild(modifications, nextBuildTime);
				nextBuildTime = nextBuildTime.AddSeconds(secondsUntilNextBuild);

				Log.Info(string.Format("Modifications have been detected in the quiet period.  Sleeping for {0} seconds until {1}.", secondsUntilNextBuild, nextBuildTime));
				dtProvider.Sleep(secondsUntilNextBuild*1000);
				to.StartTime = nextBuildTime;
				modifications = GetMods(sourceControl, from, to);
			}
			return modifications;
		}

		private Modification[] GetMods(ISourceControl sc, IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = sc.GetModifications(from, to);
			if (modifications == null) modifications = new Modification[0];
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
					return string.Format("{0} modifications detected.", modifications.Length);
			}
		}

		private bool ModificationsAreDetectedInQuietPeriod(Modification[] modifications, DateTime to)
		{
//			return SecondsUntilNextBuild(modifications, to) > 0;
			return false;
		}

		private int SecondsUntilNextBuild(Modification[] modifications, DateTime to)
		{
			return (int) (ModificationDelaySeconds - SecondsSinceLastModification(modifications, to));
		}

		private double SecondsSinceLastModification(Modification[] modifications, DateTime to)
		{
			return (to - GetMostRecentModificationDate(modifications)).TotalSeconds;
		}

		private DateTime GetMostRecentModificationDate(Modification[] modifications)
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