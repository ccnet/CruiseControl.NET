using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class WorkflowResultTest 
	{
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
