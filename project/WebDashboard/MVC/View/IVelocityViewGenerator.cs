using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public interface IVelocityViewGenerator
	{
		IView GenerateView(string templateName, Hashtable velocityContext);
	}
}
