using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class DateLabellerTest
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
			Assert.AreEqual("2005.1.1.1", labeller.Generate(IntegrationResult.CreateInitialIntegrationResult("foo", @"c:\")));
		}

		[Test]
		public void IncrementLabelOnSuccessfulBuild()
		{
			Assert.AreEqual("2005.1.1.15", labeller.Generate(IntegrationResultMother.CreateSuccessful("2005.1.1.14")));
		}

		[Test]
		public void HandleInvalidLabel()
		{
			Assert.AreEqual("2005.1.1.1", labeller.Generate(IntegrationResultMother.CreateSuccessful("13")));			
		}
	}
}