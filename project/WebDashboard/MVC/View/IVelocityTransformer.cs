using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public interface IVelocityTransformer
	{
		string Transform(string templateName, Hashtable velocityContext);
	}
}
