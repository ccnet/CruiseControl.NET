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
			TestReader stream = new TestReader("string to read");
			using (ProcessReader reader = new ProcessReader(stream))
			{
				reader.WaitForExit();
				Assert.AreEqual("string to read", reader.Output);				
			}
			Assert.IsTrue(stream.IsClosed);
		}

		class TestReader : StringReader
		{
			private bool isClosed = false;

			public TestReader(string s) : base(s)
			{
			}

			public override void Close()
			{
				base.Close();
				isClosed = true;
			}

			public bool IsClosed
			{
				get { return isClosed; }
			}
		}
	}
}
