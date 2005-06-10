using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl
{
	[TestFixture]
	public class UserFilterTest
	{
		private UserFilter filter;

		[SetUp]
		protected void CreateFilter()
		{
			filter = new UserFilter();			
		}

		[Test]
		public void ShouldNotAcceptIfNoUserIsSpecified()
		{
			Assert.IsFalse(filter.Accept(new Modification()));
		}

		[Test]
		public void ShouldFilterSpecifiedUser()
		{
			Modification mod = new Modification();
			mod.UserName = "bob";

			filter.UserNames = new string[] { "bob" };
			Assert.IsTrue(filter.Accept(mod));
		}

		[Test]
		public void LoadFromConfiguration()
		{
			filter = (UserFilter) NetReflector.Read(@"<userFilter><names><name>bob</name><name>perry</name></names></userFilter>");
			Assert.AreEqual(2, filter.UserNames.Length);
			Assert.AreEqual("bob", filter.UserNames[0]);
			Assert.AreEqual("perry", filter.UserNames[1]);
		}
	}
}