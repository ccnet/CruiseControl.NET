using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public interface IVelocityViewGenerator
	{
		IResponse GenerateView(string templateName, Hashtable velocityContext);
	}
}
