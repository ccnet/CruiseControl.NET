using System;
using Moq;
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
			var objectSourceMock = new Mock<ObjectSource>();
			Type typeToInstantiate = typeof(XslReportBuildAction);
			ICruiseAction instantiated = new XslReportBuildAction(null, null);
			objectSourceMock.Setup(objectSource => objectSource.GetByType(typeToInstantiate)).Returns(instantiated).Verifiable();

			ActionInstantiatorWithObjectSource instantiator = new ActionInstantiatorWithObjectSource((ObjectSource) objectSourceMock.Object);
			Assert.AreEqual(instantiated, instantiator.InstantiateAction(typeToInstantiate));
			objectSourceMock.Verify();
		}
	}
}
