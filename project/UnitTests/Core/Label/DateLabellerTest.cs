using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class DateLabellerTest : IntegrationFixture
	{
		private DateLabeller labeller;

		[SetUp]
		protected void SetUp()
		{
			IMock mockDateTimeProvider = new DynamicMock(typeof (DateTimeProvider));
			mockDateTimeProvider.SetupResult("Now", new DateTime(2005, 1, 1));
			labeller = new DateLabeller((DateTimeProvider) mockDateTimeProvider.MockInstance);
		}

		[Test]
		public void GenerateInitialLabel()
		{
			Assert.AreEqual("2005.01.01.001", labeller.Generate(IntegrationResult.CreateInitialIntegrationResult("foo", @"c:\", @"c:\")));
		}

		[Test]
		public void IncrementLabelOnSuccessfulBuild()
		{
			Assert.AreEqual("2005.01.01.015", labeller.Generate(SuccessfulResult("2005.1.1.14")));
		}

		[Test]
		public void IncrementLastSuccessfulLabelOnFailedBuild()
		{
			Assert.AreEqual("2005.01.01.014", labeller.Generate(FailedResult("2005.1.1.14", "2005.1.1.13")));
		}

		[Test]
		public void HandleInvalidLabel()
		{
			Assert.AreEqual("2005.01.01.001", labeller.Generate(SuccessfulResult("13")));
		}

        [Test]
        public void GenerateInitialLabel_CustomLayout01()
        {
            labeller.DayFormat = "0";
            labeller.MonthFormat = "0";
            labeller.RevisionFormat = "0";
            Assert.AreEqual("2005.1.1.1", labeller.Generate(IntegrationResult.CreateInitialIntegrationResult("foo", @"c:\", @"c:\")));
        }


        [Test]
        public void GenerateInitialLabel_CustomLayout02()
        {
            labeller.DayFormat = "00000";
            labeller.MonthFormat = "00000";
            labeller.RevisionFormat = "00000";

            Assert.AreEqual("2005.00001.00001.00001", labeller.Generate(IntegrationResult.CreateInitialIntegrationResult("foo", @"c:\", @"c:\")));
        }

	}
}