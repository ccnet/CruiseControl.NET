using System.Web;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CruiseObjectGiverInitializerTest
	{
		[Test]
		public void ShouldReturnAnObjectGiverThatCanGiveRequiredTypes()
		{
//			HttpRequest request = new HttpRequest("foo", "http://foo/bar", "foo");
			HttpContext context = new HttpContext(null);

			ObjectGiverAndRegistrar objectGiver = new ObjectGiverAndRegistrar();
			objectGiver.IgnoreNMockImplementations = true;

			new CruiseObjectGiverInitializer(objectGiver).InitializeGiverForRequest(context);

			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(HttpPathMapper)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ServerQueryingBuildRetriever)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(CruiseManagerBuildNameRetriever)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ConfigurationSettingsConfigGetter)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(PathMapperUsingHostName)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(DefaultBuildNameFormatter)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ServerAggregatingCruiseManagerWrapper)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(SideBarViewBuilder)));
			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder)));

			// ToDo - test Plugins registered correctly
		}
	}

	public class StubBuildPlugin : IPlugin
	{
		public TypedAction[] Actions
		{
			get { return new TypedAction[] { new TypedAction("MyPlugin", this.GetType()) }; }
		}
	}
}
