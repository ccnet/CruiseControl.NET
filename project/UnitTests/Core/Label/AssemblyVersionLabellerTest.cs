using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class AssemblyVersionLabellerTest : IntegrationFixture
	{
		private AssemblyVersionLabeller labeller;

		private static IntegrationResult CreateIntegrationResult()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			return result;
		}

		private static void AddModifications(IntegrationResult result)
		{
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

		[SetUp]
		public void SetUp()
		{
			labeller = new AssemblyVersionLabeller();
		}

		[Test]
		public void GenerateLabel()
		{
			IntegrationResult result = CreateIntegrationResult();
			AddModifications(result);
			Assert.AreEqual(new Version(0, 0, 1, 30).ToString(), labeller.Generate(result));
		}

		[Test]
		public void GenerateLabelFromNoMods()
		{
			IntegrationResult result = CreateIntegrationResult();
			Assert.AreEqual(new Version(0, 0, 0, 0).ToString(), labeller.Generate(result));
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = @"
				<labeller type='assemblyVersionLabeller'>
					<major>1</major>
					<minor>2</minor>
					<build>1234</build>
					<revision>123456</revision>
					<incrementOnFailure>false</incrementOnFailure>
				</labeller>";
			
			NetReflector.Read(xml, labeller);
			Assert.AreEqual(1, labeller.Major);
			Assert.AreEqual(2, labeller.Minor);
			Assert.AreEqual(1234, labeller.Build);
			Assert.AreEqual(123456, labeller.Revision);
			Assert.AreEqual(false, labeller.IncrementOnFailure);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			string xml = @"<labeller type='assemblyVersionLabeller' />";

			NetReflector.Read(xml, labeller);
			Assert.AreEqual(0, labeller.Major);
			Assert.AreEqual(0, labeller.Minor);
			Assert.AreEqual(-1, labeller.Build);
			Assert.AreEqual(-1, labeller.Revision);
			Assert.AreEqual(false, labeller.IncrementOnFailure);
		}

		[Test]
		public void GenerateLabelIterative()
		{
			Assert.AreEqual(new Version(0, 0, 0, 0).ToString(), labeller.Generate(SuccessfulResult("unknown")));

			IntegrationResult result = SuccessfulResult(new Version(0, 0, 1, 30).ToString());
			AddModifications(result);
			Assert.AreEqual(new Version(0, 0, 1, 30).ToString(), labeller.Generate(result));

			result.BuildCondition = BuildCondition.ForceBuild;
			Assert.AreEqual(new Version(0, 0, 2, 30).ToString(), labeller.Generate(result));

			result = SuccessfulResult(new Version(0, 0, 2, 30).ToString());
			AddModifications(result);
			labeller.Major++;
			Assert.AreEqual(new Version(1, 0, 3, 30).ToString(), labeller.Generate(result));

			result = SuccessfulResult(new Version(0, 0, 3, 30).ToString());
			AddModifications(result);
			labeller.Minor++;
			Assert.AreEqual(new Version(1, 1, 4, 30).ToString(), labeller.Generate(result));

			result = SuccessfulResult(new Version(0, 0, 4, 30).ToString());
			labeller.Revision = 40;
			Assert.AreEqual(new Version(1, 1, 5, 40).ToString(), labeller.Generate(result));

			result = SuccessfulResult(new Version(0, 0, 1, 30).ToString());
			AddModifications(result);
			labeller.Major = 5;
			labeller.Minor = 3;
			labeller.Revision = 5467;
			Assert.AreEqual(new Version(5, 3, 2, 5467).ToString(), labeller.Generate(result));

			result = SuccessfulResult(new Version(5, 0, 1, 30).ToString());
			AddModifications(result);
			labeller.Major = 5;
			labeller.Minor = 3;
			labeller.Build = 1234;
			labeller.Revision = 5467;
			Assert.AreEqual(new Version(5, 3, 1234, 5467).ToString(), labeller.Generate(result));
		}
	}
}
