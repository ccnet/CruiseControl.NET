using System;
using System.Web;
using NUnit.Framework;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CruiseObjectGiverInitializerTest
	{
		// See ToDo in CruiseObjectGiver Initializer - needs sorting out
//		[Test]
//		public void ShouldReturnAnObjectGiverThatCanGiveRequiredTypes()
//		{
////			HttpRequest request = new HttpRequest("foo", "http://foo/bar", "foo");
//			HttpContext context = new HttpContext(null);
//
//			ObjectGiverAndRegistrar objectGiver = new ObjectGiverAndRegistrar();
//			objectGiver.IgnoreNMockImplementations = true;
//
//			new CruiseObjectGiverInitializer(objectGiver).SetupObjectGiverForRequest(context);
//
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(HttpPathMapper)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ServerQueryingBuildRetriever)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(CruiseManagerBuildNameRetriever)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ConfigurationSettingsConfigGetter)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(PathMapperUsingHostName)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(DefaultBuildNameFormatter)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(ServerAggregatingCruiseManagerWrapper)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(SideBarViewBuilder)));
//			Assert.IsNotNull(objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder)));
//
//			// ToDo - test Plugins registered correctly
//		}
	}

	public class StubBuildPlugin : IPlugin
	{
		public INamedAction[] NamedActions
		{
			get { return new INamedAction[] { new ImmutableNamedAction("MyPlugin", null) }; }
		}

		public string LinkDescription
		{
			get { throw new NotImplementedException(); }
		}

		public string LinkActionName
		{
			get { throw new NotImplementedException(); }
		}
	}
}
