using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public class DefaultVelocityViewGenerator : IVelocityViewGenerator
	{
		private readonly IVelocityTransformer velocityTransformer;

		public DefaultVelocityViewGenerator(IVelocityTransformer velocityTransformer)
		{
			this.velocityTransformer = velocityTransformer;
		}

		public IView GenerateView(string templateName, Hashtable velocityContext)
		{
			return new HtmlView(velocityTransformer.Transform(templateName, velocityContext));
		}
	}
}
