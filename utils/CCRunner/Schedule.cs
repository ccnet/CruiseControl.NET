using System;
using tw.ccnet.remote;

namespace CCNet.CCRunner
{
	[Serializable]
	public class Schedule : ISchedule
	{
		private int runs = 0; 

		public Schedule()
		{
		}

		public bool ForceBuild 
		{ 
			get { return true; }
		}
		
		public bool ShouldRun()
		{
			return runs == 0;
		}

		public TimeSpan CalculateTimeToNextIntegration()
		{
			return new TimeSpan(0);		// run immediately
		}

		public void Update()
		{
			runs++;
		}

		public override bool Equals(object obj)
		{
			return (obj is Schedule) && runs == ((Schedule)obj).runs;
		}

		public override int GetHashCode()
		{
			return runs;
		}
	}
}
