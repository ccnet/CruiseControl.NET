using System;
using System.Globalization;
using System.IO;
using NMock;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
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
			_vss = new Vss((IHistoryParser) _historyParser.MockInstance, (ProcessExecutor) _executor.MockInstance, null);
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
			ProcessResult result = new ProcessResult("", "", 0, false);
			Modification[] dummyArray = new Modification[1] { new Modification() };
			_historyParser.SetupResult("Parse", dummyArray, typeof(TextReader), typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));

			_vss.GetModifications(DateTime.Now, DateTime.Now);
		}

		[Test]
		public void GetModificationsDoesNotCreateLabelWhenThereAreNoModifications()
		{
			ProcessResult result = new ProcessResult("", "", 0, false);
			Modification[] emptyArray = new Modification[0];
			_historyParser.SetupResult("Parse", emptyArray, typeof(TextReader), typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));

			_vss.GetModifications(DateTime.Now, DateTime.Now);
		}
	}
}