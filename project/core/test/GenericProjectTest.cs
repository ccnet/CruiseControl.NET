using System;
using NUnit.Framework;
using NMock;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class GenericProjectTest : CustomAssertion
	{
		[Test]
		public void LoalConfiguration()
		{
			string xml = @"
<generic>
	<tasks>
		<mock/>
		<mock/>
	</tasks>
</generic>";
			XmlPopulator populator = new XmlPopulator();
			object obj = populator.Populate(XmlUtil.CreateDocumentElement(xml));
			AssertNotNull(obj);
			AssertEquals(typeof(GenericProject), obj.GetType());
			GenericProject project = (GenericProject)obj;
			AssertEquals(2, project.Tasks.Count);
			Assert(project.Tasks[0] is ITask);
			Assert(project.Tasks[1] is ITask);
			//TODO: need NetReflector to enforce type of collection elements
		}

		[Test]
		public void Run()
		{
			IMock taskMock1 = new DynamicMock(typeof(ITask));
			taskMock1.Expect("Run", new NMock.Constraints.NotNull());

		}
	}
}
