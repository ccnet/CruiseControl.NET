
using System.Collections;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class TestTraceListener : TraceListener
	{
		private ArrayList _traces = new ArrayList();

        /// <summary>
        /// Writes the specified trace.	
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <remarks></remarks>
		public override void Write(string trace)
		{
			_traces.Add(trace);
		}

        /// <summary>
        /// Writes the line.	
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <remarks></remarks>
		public override void WriteLine(string trace)
		{
			_traces.Add(trace);
		}

        /// <summary>
        /// Gets the traces.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public IList Traces
		{
			get { return _traces; }
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return string.Join("\n", (string[])_traces.ToArray(typeof(string)));
		}
	}
}
