using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("schedule")]
	public class Schedule : ISchedule
	{
		public const int Infinite = -1;
		public const int DefaultTimeOut = 60;

		private int _iterationsSoFar = 0;
		private DateTime _lastIntegrationCompleteTime = DateTime.MinValue;
		private DateTimeProvider _dtProvider;

		public Schedule() : this(new DateTimeProvider()) { }
		public Schedule(DateTimeProvider dtProvider) : this(dtProvider, DefaultTimeOut, Infinite) { }
		public Schedule(DateTimeProvider dtProvider, int timeout, int totalIterations)
		{
			_dtProvider = dtProvider;
			SleepSeconds = timeout;
			TotalIterations = totalIterations;
		}

		/// <summary>
		/// Gets and sets the number of seconds between builds for which
		/// the project integrator should sleep.
		/// </summary>
		[ReflectorProperty("sleepSeconds")]
		public double SleepSeconds = DefaultTimeOut;

		/// <summary>
		/// Gets and sets the total number of iterations this schedule will allow
		/// before stopping integration of the project.  If this value is not
		/// specified, the number of iterations is not capped.
		/// </summary>
		[ReflectorProperty("iterations", Required=false)]
		public int TotalIterations = Infinite;

		public void IntegrationCompleted()
		{
			_iterationsSoFar++;
			_lastIntegrationCompleteTime = _dtProvider.Now;
		}

		public BuildCondition ShouldRunIntegration()
		{
			if (ShouldStopIntegration())
				return BuildCondition.NoBuild;

			TimeSpan timeSinceLastBuild = _dtProvider.Now - _lastIntegrationCompleteTime;
			if (timeSinceLastBuild.TotalSeconds < SleepSeconds)
				return BuildCondition.NoBuild;

			return BuildCondition.IfModificationExists;
		}

		public bool ShouldStopIntegration()
		{
			return (TotalIterations != Infinite && _iterationsSoFar >= TotalIterations);
		}

		public int IterationsSoFar
		{
			get { return _iterationsSoFar; }
		}
	}
}
