using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public abstract class HtmlBuilderViewBuilder
	{
		private readonly IHtmlBuilder htmlBuilder;

		public HtmlBuilderViewBuilder(IHtmlBuilder htmlBuilder)
		{
			this.htmlBuilder = htmlBuilder;
		}

		public HtmlTable Table(params HtmlTableRow[] rows)
		{
			return htmlBuilder.CreateTable(rows);
		}

		public HtmlTableRow TR(params HtmlTableCell[] cells)
		{
			return htmlBuilder.CreateRow(cells);
		}

		public HtmlTableCell TD(string content)
		{
			return htmlBuilder.CreateCell(content);
		}

		public HtmlTableCell TD(Control control)
		{
			return htmlBuilder.CreateCell(control);
		}

		public HtmlTableCell TD()
		{
			return htmlBuilder.CreateCell();
		}

		public DropDownList DropDown(string id, string[] entries, string selectedEntry)
		{
			return htmlBuilder.CreateDropDownList(id, entries, selectedEntry);
		}

		public TextBox TextBox(string id, string text)
		{
			return htmlBuilder.CreateTextBox(id, text);
		}

		public CheckBox BooleanCheckBox(string id, bool isChecked)
		{
			return htmlBuilder.CreateBooleanCheckBox(id, isChecked);
		}

		public Button Button(string id, string text)
		{
			return htmlBuilder.CreateButton(id, text);
		}
	}
}
