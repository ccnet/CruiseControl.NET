using System;
using System.Globalization;
using System.IO;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class VssApplyLabelTest : CustomAssertion
	{
		private IMock _executor;
		private IMock _historyParser;
		private Vss _vss;

		[SetUp]
		protected void SetUp()
		{
			_executor = new DynamicMock(typeof(ProcessExecutor));
			_historyParser = new DynamicMock(typeof(IHistoryParser));
			_vss = new Vss(new VssLocale(CultureInfo.InvariantCulture), (IHistoryParser) _historyParser.MockInstance, (ProcessExecutor) _executor.MockInstance, null);
			_vss.Executable = "ss.exe";
		}

		[TearDown]
		protected void TearDown()
		{
			_executor.Verify();
		}

		[Test]
		public void ApplyLabelIsDisabledByDefault()
		{
			AssertFalse(_vss.ApplyLabel);
		}

		[Test]
		public void GetModificationsWhenApplyLabelIsDisabledDoesNotCreateLabels()
		{
			ProcessResult result = new ProcessResult("",string.Empty, 0, false);
			Modification[] dummyArray = new Modification[1] { new Modification() };
			_historyParser.SetupResult("Parse", dummyArray, typeof(TextReader), typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));

			_vss.GetModifications(IntegrationResultMother.CreateSuccessful(DateTime.Now), IntegrationResultMother.CreateSuccessful(DateTime.Now));
		}

		[Test]
		public void GetModificationsDoesNotCreateLabelWhenThereAreNoModifications()
		{
			ProcessResult result = new ProcessResult("",string.Empty, 0, false);
			Modification[] emptyArray = new Modification[0];
			_historyParser.SetupResult("Parse", emptyArray, typeof(TextReader), typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));

			_vss.GetModifications(IntegrationResultMother.CreateSuccessful(DateTime.Now), IntegrationResultMother.CreateSuccessful(DateTime.Now));
		}
	}
}