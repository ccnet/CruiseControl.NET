using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public interface IHtmlBuilder
	{
		HtmlTable CreateTable(params HtmlTableRow[] rows);
		HtmlTableRow CreateRow(params HtmlTableCell[] cells);
		HtmlTableCell CreateCell(string content);
		HtmlTableCell CreateCell(Control control);
		HtmlTableCell CreateCell();
		DropDownList CreateDropDownList(string id, string[] entries, string selectedEntry);
		TextBox CreateTextBox(string id, string text);
		CheckBox CreateBooleanCheckBox(string id, bool isChecked);
		Button CreateButton(string id, string text);
		TextBox CreateMultiLineTextBox(string id, string text);
		HtmlAnchor CreateAnchor(string text, string url);
	}
}
