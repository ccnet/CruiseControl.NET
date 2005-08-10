using System.Collections;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
	public class SiteTemplateActionDecorator : IAction
	{
		private readonly IAction decoratedAction;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ObjectGiver objectGiver;

		public SiteTemplateActionDecorator(IAction decoratedAction, IVelocityViewGenerator velocityViewGenerator, ObjectGiver objectGiver)
		{
			this.decoratedAction = decoratedAction;
			this.velocityViewGenerator = velocityViewGenerator;
			this.objectGiver = objectGiver;
		}

		public IResponse Execute(IRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["breadcrumbs"] = (((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute()).ResponseFragment;
			velocityContext["sidebar"] = (((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute()).ResponseFragment;
			velocityContext["mainContent"] = decoratedAction.Execute(cruiseRequest).ResponseFragment;

			velocityContext["dashboardversion"] = GetVersion();

			return velocityViewGenerator.GenerateView("SiteTemplate.vm", velocityContext);
		}

		private string GetVersion()
		{
			System.Reflection.Assembly assembly;
			
			assembly = System.Reflection.Assembly.GetExecutingAssembly();
			return assembly.GetName().Version.ToString();
		}

	}
}
