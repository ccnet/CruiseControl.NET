using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class IterationLabellerTest : CustomAssertion
	{
		private IterationLabeller labeller;
		private DateTime releaseStartDate = new DateTime(2005, 01, 01, 00, 00, 00, 00);

		[SetUp]
		public void SetUp()
		{
			labeller = new IterationLabeller();
			labeller.ReleaseStartDate = releaseStartDate;
		}

		[Test]
		public void GenerateIncrementedLabel()
		{
			Assert.AreEqual("14.36", labeller.Generate(IntegrationResultMother.CreateSuccessful("14.35")));
		}

		[Test]
		public void GenerateWithNullLabel()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label = null;
			Assert.AreEqual("14.1", labeller.Generate(result));
		}

		[Test]
		public void GenerateAfterLastBuildFailed()
		{
			Assert.AreEqual("14.23", labeller.Generate(IntegrationResultMother.CreateFailed("14.23")));
		}

		[Test]
		public void GeneratePrefixedLabelWithNullResultLabel()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.Label = null;
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample.14.1", labeller.Generate(result));
		}

		[Test]
		public void GeneratePrefixedLabelOnSuccessAndPreviousLabel()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample.14.24", labeller.Generate(IntegrationResultMother.CreateSuccessful("Sample.14.23")));
		}

		[Test]
		public void GeneratePrefixedLabelOnFailureAndPreviousLabel()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample.14.23", labeller.Generate(IntegrationResultMother.CreateFailed("Sample.14.23")));
		}

		[Test]
		public void GeneratePrefixedLabelOnSuccessAndPreviousLabelWithDifferentPrefix()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample.14.24", labeller.Generate(IntegrationResultMother.CreateSuccessful("SomethingElse.14.23")));
		}

		[Test]
		public void IncrementPrefixedLabelWithNumericPrefix()
		{
			labeller.LabelPrefix = "R3SX";
			Assert.AreEqual("R3SX.14.24", labeller.Generate(IntegrationResultMother.CreateSuccessful("R3SX.14.23")));
		}

		[Test]
		public void IncrementPrefixedLabelWithNumericSeperatorSeperatedPrefix()
		{
			labeller.LabelPrefix = "1.0";
			Assert.AreEqual("1.0.14.24", labeller.Generate(IntegrationResultMother.CreateSuccessful("1.0.14.23")));
		}
	}
}