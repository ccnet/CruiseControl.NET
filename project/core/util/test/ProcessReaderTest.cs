using NMock;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessReaderTest
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
	}
}
