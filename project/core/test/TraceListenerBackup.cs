using System.Collections;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	public class TraceListenerBackup
	{
		private ArrayList backupListenerCollection;

		public TraceListenerBackup()
		{
			backupListenerCollection = new ArrayList();
			backupListenerCollection.AddRange(Trace.Listeners);
			Trace.Listeners.Clear();			
		}

		public void Reset()
		{
			Trace.Listeners.Clear();
			foreach (TraceListener listener in backupListenerCollection)
			{
				Trace.Listeners.Add(listener);				
			}			
		}

		public TestTraceListener AddTestTraceListener()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			return listener;
		}

		public TraceListener AddTraceListener(TraceListener listener)
		{
			Trace.Listeners.Add(listener);
			return listener;
		}
	}
}
