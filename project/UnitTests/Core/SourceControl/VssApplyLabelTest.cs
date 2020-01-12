using System;
using System.Globalization;
using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class VssApplyLabelTest : CustomAssertion
	{
		private Mock<ProcessExecutor> _executor;
		private Mock<IHistoryParser> _historyParser;
		private Vss _vss;

		[SetUp]
		protected void SetUp()
		{
			_executor = new Mock<ProcessExecutor>();
			_historyParser = new Mock<IHistoryParser>();
			_vss = new Vss(new VssLocale(CultureInfo.InvariantCulture), (IHistoryParser) _historyParser.Object, (ProcessExecutor) _executor.Object, null);
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
			_historyParser.Setup(parser => parser.Parse(It.IsAny<TextReader>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(dummyArray).Verifiable();
			_executor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(result).Verifiable();

			_vss.GetModifications(IntegrationResultMother.CreateSuccessful(DateTime.Now), IntegrationResultMother.CreateSuccessful(DateTime.Now));

			_executor.Verify();
			_executor.VerifyNoOtherCalls();
		}

		[Test]
		public void GetModificationsDoesNotCreateLabelWhenThereAreNoModifications()
		{
			ProcessResult result = new ProcessResult("",string.Empty, 0, false);
			Modification[] emptyArray = new Modification[0];
			_historyParser.Setup(parser => parser.Parse(It.IsAny<TextReader>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(emptyArray).Verifiable();
			_executor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(result).Verifiable();

			_vss.GetModifications(IntegrationResultMother.CreateSuccessful(DateTime.Now), IntegrationResultMother.CreateSuccessful(DateTime.Now));

			_executor.Verify();
			_executor.VerifyNoOtherCalls();
		}
	}
}