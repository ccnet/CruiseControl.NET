using System;
using tw.ccnet.remote;

namespace CCRunner
{
	public class Schedule : MarshalByRefObject, ISchedule
	{
		private int runs = 0; 

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
	}
}
