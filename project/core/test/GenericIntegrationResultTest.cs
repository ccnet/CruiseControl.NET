using NUnit.Framework;
using System;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class GenericIntegrationResultTest : Assertion
	{
		[Test]
		public void AppendOutput()
		{
			GenericIntegrationResult result = new GenericIntegrationResult();
			result.Output = "foo";
			result.Output = "bar";
			AssertEquals("foobar", result.Output);
		}

		[Test]
		public void AppendModifications()
		{
			GenericIntegrationResult result = new GenericIntegrationResult();
			Modification mod1 = new Modification();
			Modification mod2 = new Modification();
			result.Modifications = new Modification[] { mod1 };
			result.Modifications = new Modification[] { mod2 };

			AssertEquals(2, result.Modifications.Length);
			AssertEquals(mod1, result.Modifications[0]);
			AssertEquals(mod2, result.Modifications[1]);
		}
	}
}
