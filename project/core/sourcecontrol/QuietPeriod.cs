using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class QuietPeriod
	{
		public const int TURN_OFF_QUIET_PERIOD = 0;
		public double ModificationDelaySeconds = TURN_OFF_QUIET_PERIOD;
		private readonly DateTimeProvider dtProvider;

		public QuietPeriod(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		public Modification[] GetModifications(ISourceControl sc, DateTime from, DateTime to)
		{
			Modification[] modifications = GetMods(sc, from, to);				
			while (ModificationsAreDetectedInQuietPeriod(modifications, to))
			{
				double secondsUntilNextBuild = ModificationDelaySeconds - SecondsSinceLastBuild(modifications, to);
				to = to.AddSeconds(secondsUntilNextBuild);

				Log.Info("Modifications have been detected in the quiet delay; waiting until " + to);
				dtProvider.Sleep((int)(secondsUntilNextBuild * 1000));

				modifications = GetMods(sc, from, to);
			}
			return modifications;
		}

		private Modification[] GetMods(ISourceControl sc, DateTime from, DateTime to)
		{
			Modification[] modifications = sc.GetModifications(from, to);
			if (modifications == null) modifications = new Modification[0];
			return modifications;
		}

		private bool ModificationsAreDetectedInQuietPeriod(Modification[] modifications, DateTime to)
		{
			return ModificationDelaySeconds != TURN_OFF_QUIET_PERIOD && SecondsSinceLastBuild(modifications, to) < ModificationDelaySeconds;
		}

		private double SecondsSinceLastBuild(Modification[] modifications, DateTime to)
		{
			return (to - GetLastModificationDate(modifications)).TotalSeconds;
		}

		private DateTime GetLastModificationDate(Modification[] modifications)
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
