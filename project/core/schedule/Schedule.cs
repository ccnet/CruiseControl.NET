using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("schedule")]
	public class Schedule : ISchedule
	{
		public const int Infinite = -1;
		public const int DefaultTimeOut = 60;

		private double _timeOut = DefaultTimeOut;
		private int _totalIterations = Infinite;
		private int _iterationsSoFar = 0;
		private bool _forceNextBuild = false;
		private DateTime _lastIntegrationCompleteTime = DateTime.MinValue;

		public Schedule() { }

		public Schedule(int timeout, int totalIterations)
		{
			_timeOut = timeout;
			_totalIterations = totalIterations;
		}

		/// <summary>
		/// Gets and sets the number of seconds between builds for which
		/// the project integrator should sleep.
		/// </summary>
		[ReflectorProperty("sleepSeconds")]
		public double SleepSeconds
		{
			get { return _timeOut; }
			set { _timeOut = value ; }
		}

		/// <summary>
		/// Gets and sets the total number of iterations this schedule will allow
		/// before stopping integration of the project.  If this value is not
		/// specified, the number of iterations is not capped.
		/// </summary>
		[ReflectorProperty("iterations", Required=false)]
		public int TotalIterations
		{
			get { return _totalIterations; }
			set { _totalIterations = value; }
		}

		public void IntegrationCompleted()
		{
			_iterationsSoFar++;
			_lastIntegrationCompleteTime = DateTime.Now;
		}

		public BuildCondition ShouldRunIntegration()
		{
			if (ShouldStopIntegration())
				return BuildCondition.NoBuild;

			if (_forceNextBuild)
			{
				_forceNextBuild = false;
				return BuildCondition.ForceBuild;
			}

			TimeSpan timeSinceLastBuild = DateTime.Now - _lastIntegrationCompleteTime;
			if (timeSinceLastBuild.TotalSeconds < SleepSeconds)
				return BuildCondition.NoBuild;

			return BuildCondition.IfModificationExists;
		}

		public bool ShouldStopIntegration()
		{
			return (_totalIterations != Infinite && _iterationsSoFar >= _totalIterations);
		}

		/// <summary>
		/// Forces <see cref="ShouldRunIntegration"/> to return true on its next
		/// invocation, regardless of the time since the previous build, etc...
		/// </summary>
		public void ForceBuild()
		{
			_forceNextBuild = true;
		}

		public int IterationsSoFar
		{
			get { return _iterationsSoFar; }
		}
	}
}
