using System.IO;
using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class ControlView : IView
	{
		private readonly string htmlFragment;

		public ControlView(Control control)
		{
			using (TextWriter stringWriter = new StringWriter())
			{
				using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
				{
					control.RenderControl(writer);
				}
				this.htmlFragment = stringWriter.ToString();
			}
		}

		public string HtmlFragment
		{
			get { return htmlFragment; }
		}
	}
}
