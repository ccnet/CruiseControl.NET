using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Label;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class DefaultLabellerTest : IntegrationFixture
	{
		private DefaultLabeller labeller;

		[SetUp]
		public void SetUp()
		{
			labeller = new DefaultLabeller();
		}

		[Test]
		public void GenerateIncrementedLabel()
		{
			Assert.AreEqual("36", labeller.Generate(SuccessfulResult("35")));
		}

		[Test]
		public void GenerateInitialLabel()
		{
			Assert.AreEqual(DefaultLabeller.INITIAL_LABEL, labeller.Generate(InitialIntegrationResult()));
		}

		[Test]
		public void GenerateLabelWhenLastBuildFailed()
		{
			Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
		}

		[Test]
		public void GenerateInitialPrefixedLabel()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample" + DefaultLabeller.INITIAL_LABEL, labeller.Generate(InitialIntegrationResult()));
		}

		[Test]
		public void GeneratePrefixedLabelWhenLastBuildSucceeded()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample36", labeller.Generate(SuccessfulResult("35")));
		}

		[Test]
		public void GeneratePrefixedLabelWhenLastBuildFailed()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
		}

		[Test]
		public void GeneratePrefixedLabelWhenLastBuildSucceededAndHasLabelWithPrefix()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample24", labeller.Generate(SuccessfulResult("Sample23")));
		}

		[Test]
		public void GeneratePrefixedLabelWhenPrefixAndLastIntegrationLabelDontMatch()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample24", labeller.Generate(SuccessfulResult("SomethingElse23")));
		}

		[Test]
		public void GeneratePrefixedLabelWhenPrefixIsNumeric()
		{
			labeller.LabelPrefix = "R3SX";
			Assert.AreEqual("R3SX24", labeller.Generate(SuccessfulResult("R3SX23")));
		}

		[Test]
		public void IncrementLabelOnFailedBuildIfIncrementConditionIsAlways()
		{
			labeller.IncrementOnFailed = true;
			Assert.AreEqual("24", labeller.Generate(FailedResult("23")));
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = @"<defaultLabeller prefix=""foo"" incrementOnFailure=""true"" />";
			NetReflector.Read(xml, labeller);
			Assert.AreEqual("foo", labeller.LabelPrefix);
			Assert.AreEqual(true, labeller.IncrementOnFailed);
		}

		[Test]
		public void DefaultValues()
		{
			Assert.AreEqual(string.Empty, labeller.LabelPrefix);
			Assert.AreEqual(false, labeller.IncrementOnFailed);
		}
	}
}