using System;
using System.Diagnostics;
using System.Collections;

namespace tw.ccnet.core.util
{
	public class TestTraceListener : TraceListener
	{
		private ArrayList _traces = new ArrayList();

		public override void Write(string trace)
		{
			_traces.Add(trace);
		}

		public override void WriteLine(string trace)
		{
			_traces.Add(trace);
		}

		public IList Traces
		{
			get { return _traces; }
		}

		public override string ToString()
		{
			return string.Join("\n", (string[])_traces.ToArray(typeof(string)));
		}
	}
}
