using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class ProcessTaskResultTest
	{
		private StringWriter writer;

		[Test]
		public void SucceededIfProcessResultSucceeded()
		{
			ProcessTaskResult result = new ProcessTaskResult(ProcessResultFixture.CreateSuccessfulResult());
			Assert.IsTrue(result.Succeeded());
			Assert.IsFalse(result.Failed());
		}

		[Test]
		public void FailedIfProcessResultFailed()
		{
			ProcessTaskResult result = new ProcessTaskResult(ProcessResultFixture.CreateNonZeroExitCodeResult());
			Assert.IsFalse(result.Succeeded());
			Assert.IsTrue(result.Failed());
		}

		[Test]
		public void FailedIfProcessResultTimedout()
		{
			ProcessTaskResult result = new ProcessTaskResult(ProcessResultFixture.CreateTimedOutResult());
			Assert.IsFalse(result.Succeeded());
			Assert.IsTrue(result.Failed());
		}

		[Test]
		public void DataShouldBeStdOutIfNoStdErr()
		{
			ProcessResult processResult = new ProcessResult("stdout", null, 5, false);
			ProcessTaskResult result = new ProcessTaskResult(processResult);
			Assert.AreEqual("stdout", result.Data);
		}

		[Test]
		public void DataShouldBeStdOutAndStdErrIfStdErrExists()
		{
			ProcessResult processResult = new ProcessResult("stdout", "error", 5, false);
			ProcessTaskResult result = new ProcessTaskResult(processResult);
			Assert.AreEqual(string.Format("stdout{0}error", Environment.NewLine), result.Data);
		}

		[Test]
		public void WriteProcessResultToXml()
		{
			Assert.AreEqual("<task><standardOutput>foo</standardOutput><standardError>bar</standardError></task>", 
				WriteToXml("foo", "bar", ProcessResult.SUCCESSFUL_EXIT_CODE, false));
		}

		[Test]
		public void WriteFailedProcessResultToXml()
		{
			Assert.AreEqual(@"<task failed=""True""><standardOutput /><standardError>bar</standardError></task>", 
				WriteToXml(null, "bar", -3, false));
		}

		[Test]
		public void WriteTimedOutProcessResultToXml()
		{
			Assert.AreEqual(@"<task failed=""True"" timedout=""True""><standardOutput /><standardError>bar</standardError></task>", 
				WriteToXml(null, "bar", ProcessResult.TIMED_OUT_EXIT_CODE, true));
		}

		private string WriteToXml(string output, string error, int errorCode, bool timedOut)
		{
			TaskResult(output, error, errorCode, timedOut).WriteTo(XmlWriter());
			return writer.ToString();
		}

		private ProcessTaskResult TaskResult(string output, string error, int errorCode, bool timedOut)
		{
			return new ProcessTaskResult(new ProcessResult(output, error, errorCode, timedOut));
		}

		private XmlWriter XmlWriter()
		{
			writer = new StringWriter();
			return new XmlTextWriter(writer);
		}
	}
}
