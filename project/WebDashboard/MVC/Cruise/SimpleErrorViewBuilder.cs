using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SimpleErrorViewBuilder : HtmlBuilderViewBuilder, IErrorViewBuilder
	{
		public SimpleErrorViewBuilder(IHtmlBuilder htmlBuilder) : base(htmlBuilder) { }

		public Control BuildView(string errorMessage)
		{
			HtmlGenericControl control = new HtmlGenericControl("div");
			control.InnerText = errorMessage;
			return control;
		}
	}
}
