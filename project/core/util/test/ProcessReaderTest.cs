using NMock;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessReaderTest : Assertion
	{
		[Test]
		public void EnsureThatStreamIsClosedOnceReadingIsComplete()
		{
			IMock mockStream = new DynamicMock(typeof(TextReader));
			mockStream.ExpectAndReturn("ReadToEnd", "string to read");
			mockStream.Expect("Close");

			ProcessReader reader = new ProcessReader((TextReader) mockStream.MockInstance);
			reader.Start();
			reader.WaitForExit();

			mockStream.Verify();	
		}

		[Test]
		public void EnsureThatStreamIsClosedIfReadingIsAborted()
		{
			ThreadedMock mockStream = new ThreadedMock(typeof(TextReader));
			mockStream.ExpectAndReturnAndSignal("ReadToEnd", null);
			mockStream.Expect("Close");

			ProcessReader reader = new ProcessReader((TextReader) mockStream.MockInstance);
			reader.Start();
			mockStream.WaitForCall();
			reader.Abort();

			mockStream.Verify();	
		}
	}

	class ThreadedMock : DynamicMock
	{
		private ManualResetEvent latch = new ManualResetEvent(false);
		private string methodName;

 		public ThreadedMock(Type type) : base(type)
		{
		}

		public void ExpectAndReturnAndSignal(string methodName, object result, params object[] args)
		{
			this.methodName = methodName;
			base.ExpectAndReturn(methodName, result, args);
		}

		public override object Invoke(string methodName, params object[] args)
		{
			object result = base.Invoke(methodName, args);
			if (this.methodName == methodName)
			{
				latch.Set();
			}
			return result;
		}

		public void WaitForCall()
		{
			latch.WaitOne();
		}
	}
}
