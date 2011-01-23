using System;
using NMock;
using NUnit.Framework;
using Objection;
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
			DynamicMock objectSourceMock = new DynamicMock(typeof(ObjectSource));
			Type typeToInstantiate = typeof(XslReportBuildAction);
			ICruiseAction instantiated = new XslReportBuildAction(null, null);
			objectSourceMock.ExpectAndReturn("GetByType", instantiated, typeToInstantiate);

			ActionInstantiatorWithObjectSource instantiator = new ActionInstantiatorWithObjectSource((ObjectSource) objectSourceMock.MockInstance);
			Assert.AreEqual(instantiated, instantiator.InstantiateAction(typeToInstantiate));
			objectSourceMock.Verify();
		}
	}
}
