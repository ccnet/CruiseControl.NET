using System;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.remote;

namespace tw.ccnet.core.label.test
{
	[TestFixture]
	public class DefaultLabellerTest
	{
		DefaultLabeller _labeller;

		[SetUp]
		public void SetUp()
		{
			_labeller = new DefaultLabeller();
		}

		public void TestGenerate()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "35";
			result.Status = IntegrationStatus.Success;

			Assertion.AssertEquals("36", _labeller.Generate(result));
		}

		public void TestGenerate_NullLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = null;

			Assertion.AssertEquals(DefaultLabeller.INITIAL_LABEL, _labeller.Generate(result));
		}

		public void TestGenerate_LastBuildFailed()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "23";
			result.Status = IntegrationStatus.Failure;

			DefaultLabeller _labeller = new DefaultLabeller();
			string label = _labeller.Generate(result);
			Assertion.AssertEquals("23", label);
		}
	}
}
