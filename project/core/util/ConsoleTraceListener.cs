#pragma warning disable 1591
using System;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ConsoleTraceListener : TextWriterTraceListener
	{
		public ConsoleTraceListener() : base(Console.Out) { }
	}
}
