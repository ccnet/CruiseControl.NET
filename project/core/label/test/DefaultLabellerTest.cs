using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label.Test
{
	[TestFixture]
	public class DefaultLabellerTest : CustomAssertion
	{
		DefaultLabeller _labeller;

		[SetUp]
		public void SetUp()
		{
			_labeller = new DefaultLabeller();
		}

		[Test]
		public void Generate()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "35";
			result.Status = IntegrationStatus.Success;

			AssertEquals("36", _labeller.Generate(result));
		}

		[Test]
		public void Generate_NullLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = null;

			AssertEquals(DefaultLabeller.INITIAL_LABEL, _labeller.Generate(result));
		}

		[Test]
		public void Generate_LastBuildFailed()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "23";
			result.Status = IntegrationStatus.Failure;

			DefaultLabeller _labeller = new DefaultLabeller();
			string label = _labeller.Generate(result);
			AssertEquals("23", label);
		}

		[Test]
		public void Generate_PrefixedLabel_NullResultLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = "Sample";
			result.Label = null;
			Assertion.AssertEquals("Sample" + DefaultLabeller.INITIAL_LABEL, _labeller.Generate(result));
		}

		[Test]
		public void Generate_PrefixedLabel_SuccessAndPreviousLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = "Sample";
			result.Label = "23";
			Assertion.AssertEquals("Sample24", _labeller.Generate(result));
		}

		[Test]
		public void Generate_PrefixedLabel_FailureAndPreviousLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = "Sample";
			result.Label = "23";
			result.Status = IntegrationStatus.Failure;
			Assertion.AssertEquals("23", _labeller.Generate(result));
		}

		[Test]
		public void Generate_PrefixedLabel_SuccessAndPreviousLabelWithPrefix()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = "Sample";
			result.Label = "Sample23";
			result.Status = IntegrationStatus.Success;
			Assertion.AssertEquals("Sample24", _labeller.Generate(result));
		}

		[Test]
		public void Generate_PrefixedLabel_SuccessAndPreviousLabelWithDifferentPrefix()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = "Sample";
			result.Label = "SomethingElse23";
			result.Status = IntegrationStatus.Success;
			Assertion.AssertEquals("Sample24", _labeller.Generate(result));
		}

		[Test]
		public void IncrementPrefixedLabel()
		{
			_labeller.LabelPrefix = "Sample";
			AssertEquals("24", _labeller.IncrementLabel("Sample23"));
		}

		[Test]
		public void IncrementPrefixedLabelDifferentPrefix()
		{
			_labeller.LabelPrefix = "Sample";
			AssertEquals("24", _labeller.IncrementLabel("SomethingElse23"));
		}

		[Test]
		public void IncrementPrefixedLabelNumericPrefix()
		{
			string prefix = "R3SX";
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			_labeller.LabelPrefix = prefix;
			result.Label = prefix + "23";
			result.Status = IntegrationStatus.Success;

			AssertEquals(prefix + "24", _labeller.Generate(result));
		}

		[Test]
		public void ShouldRun()
		{
			IProject project = (IProject) new DynamicMock(typeof(IProject)).MockInstance;
			Assert(_labeller.ShouldRun(new IntegrationResult(), project));
			Assert(_labeller.ShouldRun(IntegrationResultMother.CreateSuccessful(), project));
			AssertFalse(_labeller.ShouldRun(IntegrationResultMother.CreateFailed(), project));
			AssertFalse(_labeller.ShouldRun(IntegrationResultMother.CreateExceptioned(), project));
		}
	}
}
