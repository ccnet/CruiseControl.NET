using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	/// <remarks>
	/// This code is based on code\label\DefaultLabeller.cs.
	/// </remarks> 
	[TestFixture]
	public class LastChangeLabellerTest : CustomAssertion
	{
		private LastChangeLabeller labeller;

		[SetUp]
		public void SetUp()
		{
			labeller = new LastChangeLabeller();
		}

		[Test]
		public void GenerateLabel()
		{
			IntegrationResult result = CreateIntegrationResult();
			AddModifications(result);
			Assert.AreEqual("30", labeller.Generate(result));
		}

		[Test]
		public void GenerateLabelFromNoMods()
		{
			IntegrationResult result = CreateIntegrationResult();
			Assert.AreEqual("unknown", labeller.Generate(result));
		}

		[Test]
		public void GeneratePrefixedLabel()
		{
			IntegrationResult result = CreateIntegrationResult();
			AddModifications(result);
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample30", labeller.Generate(result));
		}

		[Test]
		public void GeneratePrefixedLabelWhenPrefixAndLastIntegrationLabelDontMatch()
		{
			IntegrationResult result = CreateIntegrationResult();
			AddModifications(result);
			labeller.LabelPrefix = "Sample";
			result.LastSuccessfulIntegrationLabel = "SomethingElse23";
			Assert.AreEqual("Sample30", labeller.Generate(result));
		}

		[Test]
		public void GeneratePrefixedLabelWhenPrefixIsNumeric()
		{
			IntegrationResult result = CreateIntegrationResult();
			AddModifications(result);
			labeller.LabelPrefix = "R3SX";
			result.LastSuccessfulIntegrationLabel = "R3SX23";
			Assert.AreEqual("R3SX30", labeller.Generate(result));
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = "<LastChangeLabeller prefix=\"foo\"/>";
			NetReflector.Read(xml, labeller);
			Assert.AreEqual("foo", labeller.LabelPrefix);
		}

		[Test]
		public void DefaultValues()
		{
			Assert.AreEqual(string.Empty, labeller.LabelPrefix);
		}

		private IntegrationResult CreateIntegrationResult()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			return result;
		}
		
		private void AddModifications(IntegrationResult result) {
			result.Modifications = new Modification[3];
			result.Modifications[0] = ModificationMother.CreateModification("fileName", "folderName",
																			new DateTime(), "userName",
																			"comment", 10, "email@address",
																			"http://url");
			result.Modifications[1] = ModificationMother.CreateModification("fileName", "folderName",
																			new DateTime(), "userName",
																			"comment", 30, "email@address",
																			"http://url");
			result.Modifications[2] = ModificationMother.CreateModification("fileName", "folderName",
			                                                                new DateTime(), "userName",
																			"comment", 20, "email@address",
																			"http://url");
		}
	}
}