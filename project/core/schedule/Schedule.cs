using System;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core.schedule
{
	[ReflectorType("schedule")]
	public class Schedule : ISchedule
	{
		public const int Infinite = -1;
		public const int DefaultTimeOut = 60000;

		private int _timeOut = DefaultTimeOut;
		private int _totalIterations = Infinite;
		private int _iterations = 0;
		private bool _forceBuild = false;

		public Schedule() { }

		public Schedule(int timeout, int totalIterations)
		{
			_timeOut = timeout;
			_totalIterations = totalIterations;
		}

		[ReflectorProperty("timeout")]
		public int TimeOut
		{
			get { return _timeOut; }
			set { _timeOut = value ; }
		}

		[ReflectorProperty("iterations", Required=false)]
		public int TotalIterations
		{
			get { return _totalIterations; }
			set { _totalIterations = value; }
		}

		[ReflectorProperty("forcebuild", Required=false)]
		public bool ForceBuild
		{
			get { return _forceBuild; }
			set { _forceBuild = value; }
		}

		public int Iterations
		{
			get { return _iterations; }
		}

		public bool ShouldRun()
		{
			return (_iterations < _totalIterations || _totalIterations == Infinite);
		}

		public TimeSpan CalculateTimeToNextIntegration()
		{
			return new TimeSpan(0, 0, 0, 0, _timeOut);
		}

		public void Update()
		{
			_iterations++;
		}
	}
}
