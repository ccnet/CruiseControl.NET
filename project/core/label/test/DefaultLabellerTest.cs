using System;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.core.label.test
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

		public void TestGenerate()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "35";
			result.Status = IntegrationStatus.Success;

			AssertEquals("36", _labeller.Generate(result));
		}

		public void TestGenerate_NullLabel()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = null;

			AssertEquals(DefaultLabeller.INITIAL_LABEL, _labeller.Generate(result));
		}

		public void TestGenerate_LastBuildFailed()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.Label = "23";
			result.Status = IntegrationStatus.Failure;

			DefaultLabeller _labeller = new DefaultLabeller();
			string label = _labeller.Generate(result);
			AssertEquals("23", label);
		}
	}
}
