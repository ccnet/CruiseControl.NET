using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public class VelocityViewGeneratorWithTransformer : IVelocityViewGenerator
	{
		private readonly IVelocityTransformer velocityTransformer;

		public VelocityViewGeneratorWithTransformer(IVelocityTransformer velocityTransformer)
		{
			this.velocityTransformer = velocityTransformer;
		}

		public IResponse GenerateView(string templateName, Hashtable velocityContext)
		{
			return new HtmlFragmentResponse(velocityTransformer.Transform(templateName, velocityContext));
		}
	}
}
