using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.View
{
	[TestFixture]
	public class LazilyInitialisingVelocityTransformerTest
	{
		private LazilyInitialisingVelocityTransformer viewTransformer;

		[Test]
		public void ShouldUseVelocityToMergeContextContentsWithTemplate()
		{
			Hashtable contextContents = new Hashtable();
			contextContents["foo"] = "bar";

			DynamicMock pathMapperMock = new DynamicMock(typeof(IPathMapper));
			pathMapperMock.SetupResult("PhysicalApplicationPath", ".");

			viewTransformer = new LazilyInitialisingVelocityTransformer((IPathMapper) pathMapperMock.MockInstance);

			Assert.AreEqual("foo is bar", viewTransformer.Transform("testTransform.vm", contextContents));
		}
	}
}
