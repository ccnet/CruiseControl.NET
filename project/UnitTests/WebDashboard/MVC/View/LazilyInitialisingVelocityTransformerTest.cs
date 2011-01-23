using System.Collections;
using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;

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

			DynamicMock pathMapperMock = new DynamicMock(typeof(IPhysicalApplicationPathProvider));
			pathMapperMock.SetupResult("GetFullPathFor", Path.Combine(".", "templates"), typeof (string));

            DynamicMock pluginsMock = new DynamicMock(typeof(IPluginConfiguration));
            pluginsMock.SetupResult("TemplateLocation", null);

            DynamicMock configurationMock = new DynamicMock(typeof(IDashboardConfiguration));
            configurationMock.SetupResult("PluginConfiguration", pluginsMock.MockInstance);

            viewTransformer = new LazilyInitialisingVelocityTransformer((IPhysicalApplicationPathProvider)pathMapperMock.MockInstance,
                (IDashboardConfiguration)configurationMock.MockInstance);

			Assert.AreEqual("foo is bar", viewTransformer.Transform("testTransform.vm", contextContents));
		}
	}
}
