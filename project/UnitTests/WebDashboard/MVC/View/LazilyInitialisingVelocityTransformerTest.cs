using System.Collections;
using System.IO;
using Moq;
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

			var pathMapperMock = new Mock<IPhysicalApplicationPathProvider>();
			pathMapperMock.Setup(provider => provider.GetFullPathFor(It.IsAny<string>())).Returns(Path.Combine(".", "templates"));

            var pluginsMock = new Mock<IPluginConfiguration>();
            pluginsMock.SetupGet(_configuration => _configuration.TemplateLocation).Returns(() => null);

            var configurationMock = new Mock<IDashboardConfiguration>();
            configurationMock.SetupGet(_configuration => _configuration.PluginConfiguration).Returns(pluginsMock.Object);

            viewTransformer = new LazilyInitialisingVelocityTransformer((IPhysicalApplicationPathProvider)pathMapperMock.Object,
                (IDashboardConfiguration)configurationMock.Object);

			Assert.AreEqual("foo is bar", viewTransformer.Transform("TestTransform.vm", contextContents));
		}
	}
}
