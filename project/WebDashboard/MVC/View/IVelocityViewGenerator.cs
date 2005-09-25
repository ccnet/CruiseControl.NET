using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public interface IVelocityViewGenerator
	{
		HtmlFragmentResponse GenerateView(string templateName, Hashtable velocityContext);
	}
}
