using System;
using NMock;
using NUnit.Framework;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ActionInstantiatorWithObjectGiverTest
	{
		[Test]
		public void ShouldUseObjectGiverToInstantiateActions()
		{
			DynamicMock objectGiverMock = new DynamicMock(typeof(ObjectGiver));
			Type typeToInstantiate = typeof(XslReportBuildAction);
			ICruiseAction instantiated = new XslReportBuildAction(null);
			objectGiverMock.ExpectAndReturn("GiveObjectByType", instantiated, typeToInstantiate);

			ActionInstantiatorWithObjectGiver instantiator = new ActionInstantiatorWithObjectGiver((ObjectGiver) objectGiverMock.MockInstance);
			Assert.AreEqual(instantiated, instantiator.InstantiateAction(typeToInstantiate));
			objectGiverMock.Verify();
		}
	}
}
