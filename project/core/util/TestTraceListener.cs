using System;
using System.Diagnostics;
using System.Collections;

namespace tw.ccnet.core.util
{
	public class TestTraceListener : TraceListener
	{
		private IList _traces = new ArrayList();

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
	}
}
