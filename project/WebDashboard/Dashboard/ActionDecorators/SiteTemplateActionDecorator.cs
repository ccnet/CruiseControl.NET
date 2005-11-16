using System.Collections;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
	// ToDo - test - I think doing so will change the design a bit - will probably get more in on the constructor - should do this after 1.0
	public class SiteTemplateActionDecorator : IAction
	{
		private readonly IAction decoratedAction;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ObjectGiver objectGiver;
		private readonly IRequest request;

		public SiteTemplateActionDecorator(IAction decoratedAction, IVelocityViewGenerator velocityViewGenerator, ObjectGiver objectGiver, IRequest request)
		{
			this.decoratedAction = decoratedAction;
			this.velocityViewGenerator = velocityViewGenerator;
			this.objectGiver = objectGiver;
			this.request = request;
		}

		public IResponse Execute(IRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IResponse decoratedActionResponse = decoratedAction.Execute(cruiseRequest);
			if (decoratedActionResponse is HtmlFragmentResponse)
			{
				velocityContext["breadcrumbs"] = (((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute()).ResponseFragment;
				velocityContext["sidebar"] = (((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute()).ResponseFragment;
				velocityContext["mainContent"] = ((HtmlFragmentResponse) decoratedActionResponse).ResponseFragment;
				velocityContext["dashboardversion"] = GetVersion();
				velocityContext["applicationPath"] = request.ApplicationPath;

				return velocityViewGenerator.GenerateView("SiteTemplate.vm", velocityContext);
			}
			else
			{
				return decoratedActionResponse;
			}
		}

		private string GetVersion()
		{
			System.Reflection.Assembly assembly;
			
			assembly = System.Reflection.Assembly.GetExecutingAssembly();
			return assembly.GetName().Version.ToString();
		}
	}
}
