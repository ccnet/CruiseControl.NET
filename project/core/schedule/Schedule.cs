using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.schedule
{
	[ReflectorType("schedule")]
	public class Schedule : ISchedule
	{
		public const int Infinite = -1;
		public const long DefaultTimeOut = 60000;

		private long _timeOut = DefaultTimeOut;
		private int _totalIterations = Infinite;
		private int _iterations = 0;

		[ReflectorProperty("timeout")]
		public long TimeOut
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
			return new TimeSpan(_timeOut);
		}

		public void Update()
		{
			_iterations++;
		}
	}
}
