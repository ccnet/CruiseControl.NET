using System;
using System.Collections;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
	public class SiteTemplateActionDecorator : ICruiseAction
	{
		private readonly ICruiseAction decoratedAction;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ObjectGiver objectGiver;

		public SiteTemplateActionDecorator(ICruiseAction decoratedAction, IVelocityViewGenerator velocityViewGenerator, ObjectGiver objectGiver)
		{
			this.decoratedAction = decoratedAction;
			this.velocityViewGenerator = velocityViewGenerator;
			this.objectGiver = objectGiver;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["breadcrumbs"] = (((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute()).ResponseFragment;
			velocityContext["sidebar"] = (((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute()).ResponseFragment;
			velocityContext["mainContent"] = decoratedAction.Execute(cruiseRequest).ResponseFragment;

			return velocityViewGenerator.GenerateView("SiteTemplate.vm", velocityContext);
		}
	}
}
