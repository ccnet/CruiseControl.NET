using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SimpleErrorViewBuilder : HtmlBuilderViewBuilder, IErrorViewBuilder
	{
		public SimpleErrorViewBuilder(IHtmlBuilder htmlBuilder) : base(htmlBuilder) { }

		public IView BuildView(string errorMessage)
		{
			return new DefaultView(errorMessage);
		}
	}
}
