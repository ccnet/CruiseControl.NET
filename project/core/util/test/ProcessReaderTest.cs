using System.IO;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessReaderTest
	{
		[Test]
		public void EnsureThatStreamIsClosedOnceReadingIsComplete()
		{
			StringReader stream = new StringReader("string to read");
			using (ProcessReader reader = new ProcessReader(stream))
			{
				reader.WaitForExit();
				Assert.AreEqual("string to read", reader.Output);				
			}
		}
	}
}
