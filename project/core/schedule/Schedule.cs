using System;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core.schedule
{
	[Serializable]
	[ReflectorType("schedule")]
	public class Schedule : ISchedule
	{
		public const int Infinite = -1;
		public const int DefaultTimeOut = 60000;

		double _timeOut = DefaultTimeOut;
		int _totalIterations = Infinite;
		int _iterationsSoFar = 0;
		bool _forceNextBuild = false;
		DateTime _lastIntegrationCompleteTime = DateTime.MinValue;

		#region Constructors

		public Schedule() { }

		public Schedule(int timeout, int totalIterations)
		{
			_timeOut = timeout;
			_totalIterations = totalIterations;
		}

		#endregion

		#region Xml configuration file properties

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

		#endregion

		#region Interface implementation

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
			if (timeSinceLastBuild.TotalSeconds<SleepSeconds)
				return BuildCondition.NoBuild;

			return BuildCondition.IfModificationExists;
		}

		public bool ShouldStopIntegration()
		{
			return (_totalIterations!=Infinite && _iterationsSoFar>=_totalIterations);
		}


		#endregion

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
