using NUnit.Framework;
using System;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class WorkflowResultTest : Assertion
	{
		[Test]
		public void AppendOutput()
		{
			WorkflowResult result = new WorkflowResult();
			result.Output = "foo";
			result.Output = "bar";
			AssertEquals("foobar", result.Output);
		}

		[Test]
		public void AppendModifications()
		{
			WorkflowResult result = new WorkflowResult();
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
