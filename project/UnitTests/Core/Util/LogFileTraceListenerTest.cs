using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class LogFileTraceListenerTest : CustomAssertion
	{
		private const string TEMP_DIR = "LogFileTraceListenerTest";
		private LogFileTraceListener listener;
		private TraceListenerBackup backup;

		[SetUp]
		protected void AddTraceListener()
		{			
			listener = new LogFileTraceListener(TempFileUtil.CreateTempFile(TEMP_DIR, "ccnet.log"));
			backup = new TraceListenerBackup();
			backup.AddTraceListener(listener);
		}

		[TearDown]
		protected void RemoveTraceListenerAndCleanUp()
		{
			backup.Reset();
			listener.Close();
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}

		[Test, Ignore("temp")]
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
