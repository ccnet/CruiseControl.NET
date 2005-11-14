using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl
{
	[TestFixture]
	public class ActionFilterTest
	{
		private ActionFilter filter;

		[SetUp]
		protected void CreateFilter()
		{
			filter = new ActionFilter();
		}

		[Test]
		public void ShouldNotAcceptIfNoActionIsSpecified()
		{
			Assert.IsFalse(filter.Accept(new Modification()));
		}

		[Test]
		public void ShouldFilterSpecifiedAction()
		{
			Modification mod = new Modification();
			mod.Type = "Created";

			filter.Actions = new string[] {"Created"};
			Assert.IsTrue(filter.Accept(mod), "Action not filtered");
		}

		[Test]
		public void LoadFromConfiguration()
		{
			filter = (ActionFilter) NetReflector.Read(@"<actionFilter><actions><action>Created</action><action>Checked in</action></actions></actionFilter>");
			Assert.AreEqual(2, filter.Actions.Length);
			Assert.AreEqual("Created", filter.Actions[0]);
			Assert.AreEqual("Checked in", filter.Actions[1]);
		}
	}
}