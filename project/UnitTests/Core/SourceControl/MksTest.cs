using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class MksTest : CustomAssertion
	{
		private string sandboxRoot;

		[SetUp]
		public void SetUp()
		{
			sandboxRoot = TempFileUtil.CreateTempDir("MksSandBox");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir("MksSandBox");
		}

		private string CreateSourceControlXml()
		{
			return string.Format(
				@"    <sourceControl type=""mks"">
						  <executable>..\bin\si.exe</executable>
						  <port>8722</port>
						  <hostname>hostname</hostname>
						  <user>CCNetUser</user>
						  <password>CCNetPassword</password>
						  <sandboxroot>{0}</sandboxroot>
						  <sandboxfile>myproject.pj</sandboxfile>
						  <autoGetSource>true</autoGetSource>
					  </sourceControl>
				 ", sandboxRoot);
		}

		[Test]
		public void CheckDefaults()
		{
			Mks mks = new Mks();
			Assert.AreEqual(@"si.exe", mks.Executable);
			Assert.AreEqual(8722, mks.Port);
			Assert.AreEqual(false, mks.AutoGetSource);
		}

		[Test]
		public void ValuePopulation()
		{
			Mks mks = CreateMks(CreateSourceControlXml(), null, null);
			Assert.AreEqual(@"..\bin\si.exe", mks.Executable);
			Assert.AreEqual(@"hostname", mks.Hostname);
			Assert.AreEqual(8722, mks.Port);
			Assert.AreEqual(@"CCNetUser", mks.User);
			Assert.AreEqual(@"CCNetPassword", mks.Password);
			Assert.AreEqual(sandboxRoot, mks.SandboxRoot);
			Assert.AreEqual(@"myproject.pj", mks.SandboxFile);
			Assert.AreEqual(true, mks.AutoGetSource);
		}

		[Test, Ignore("The Add label command needs to be replaced by check pointing")]
		public void LabelSourceControl()
		{
			IHistoryParser mockHistoryParser = (IHistoryParser) new DynamicMock(typeof (IHistoryParser)).MockInstance;
			DynamicMock mockExecutorWrapper = new DynamicMock(typeof (ProcessExecutor));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), new IsTypeOf(typeof (ProcessInfo)));
			ProcessExecutor mockProcessExecutor = (ProcessExecutor) mockExecutorWrapper.MockInstance;

			Mks mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.LabelSourceControl("dummy Label", IntegrationResultMother.CreateSuccessful());
			mockExecutorWrapper.Verify();
		}

		[Test]
		public void GetSource()
		{
			IHistoryParser mockHistoryParser = (IHistoryParser) new DynamicMock(typeof (IHistoryParser)).MockInstance;
			DynamicMock mockExecutorWrapper = new DynamicMock(typeof (ProcessExecutor));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), new IsTypeOf(typeof (ProcessInfo)));
			ProcessExecutor mockProcessExecutor = (ProcessExecutor) mockExecutorWrapper.MockInstance;

			Mks mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.GetSource(null);
			mockExecutorWrapper.Verify();
		}

		[Test]
		public void GetModifications()
		{
			DateTime from = DateTime.Now.Subtract(new TimeSpan(15000000));
			DateTime to = DateTime.Now;
			DynamicMock mockHistoryParserWrapper = new DynamicMock(typeof (IHistoryParser));
			mockHistoryParserWrapper.Expect("Parse", new IsTypeOf(typeof (TextReader)), from, to);
			IHistoryParser mockHistoryParser = (IHistoryParser) mockHistoryParserWrapper.MockInstance;
			DynamicMock mockExecutorWrapper = new DynamicMock(typeof (ProcessExecutor));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), new IsTypeOf(typeof (ProcessInfo)));
			ProcessExecutor mockProcessExecutor = (ProcessExecutor) mockExecutorWrapper.MockInstance;

			Mks mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.GetModifications(from, to);
			mockHistoryParserWrapper.Verify();
			mockExecutorWrapper.Verify();
		}

		private Mks CreateMks(string xml, IHistoryParser mockHistoryParser, ProcessExecutor mockExecutor)
		{
			Mks mks = new Mks(mockHistoryParser, mockExecutor);
			NetReflector.Read(xml, mks);
			return mks;
		}
	}
}