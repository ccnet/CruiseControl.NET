using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class LogFileTraceListenerTest : CustomAssertion
	{
		private const string TEMP_DIR = "LogFileTraceListenerTest";
		private LogFileTraceListener listener;

		[SetUp]
		protected void AddTraceListener()
		{			
			listener = new LogFileTraceListener(TempFileUtil.CreateTempFile(TEMP_DIR, "ccnet.log"));
			Trace.Listeners.Add(listener);
		}

		[TearDown]
		protected void RemoveTraceListenerAndCleanUp()
		{
			Trace.Listeners.Remove(listener);
			listener.Close();
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}

		[Test]
		public void LoggingAnEntryShouldFlushLogFileIfAutoFlushIsEnabled()
		{
			Trace.AutoFlush = true;
			Trace.WriteLine("doh!");

			using (TextReader stream = new StreamReader(new FileStream(TempFileUtil.GetTempFilePath(TEMP_DIR, "ccnet.log"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
			{
				AssertContains("doh!", stream.ReadToEnd());
			}
		}
	}
}
