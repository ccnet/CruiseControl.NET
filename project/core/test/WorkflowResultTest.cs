using NUnit.Framework;
using System;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class WorkflowResultTest 
	{
		[Test]
		public void AppendOutput()
		{
			WorkflowResult result = new WorkflowResult();
			result.Output = "foo";
			result.Output = "bar";
			Assert.AreEqual("foobar", result.Output);
		}

		[Test]
		public void AppendModifications()
		{
			WorkflowResult result = new WorkflowResult();
			Modification mod1 = new Modification();
			Modification mod2 = new Modification();
			result.Modifications = new Modification[] { mod1 };
			result.Modifications = new Modification[] { mod2 };

			Assert.AreEqual(2, result.Modifications.Length);
			Assert.AreEqual(mod1, result.Modifications[0]);
			Assert.AreEqual(mod2, result.Modifications[1]);
		}
	}
}
