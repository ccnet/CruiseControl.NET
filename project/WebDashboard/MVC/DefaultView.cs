using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class DefaultView : IView
	{
		private Control control;

		public DefaultView(string innerHtml)
		{
			control = new HtmlGenericControl("div");
			((HtmlGenericControl) control).InnerHtml = innerHtml;
		}

		public DefaultView(Control control)
		{
			this.control = control;
		}

		public Control Control
		{
			get
			{
				return control;
			}
		}
	}
}
