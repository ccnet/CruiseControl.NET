using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class DefaultHtmlBuilderTest : Assertion
	{
		[Test]
		public void ShoudCreateTablesWithAllRequestedRows()
		{
			// Setup
			HtmlTableRow row1 = new HtmlTableRow();
			HtmlTableCell cell1 = new HtmlTableCell();
			row1.Cells.Add(cell1);
			HtmlTableRow row2 = new HtmlTableRow();

			// Execute
			HtmlTable table = new DefaultHtmlBuilder().CreateTable(row1, row2);

			// Verify
			AssertEquals(2, table.Rows.Count);
			AssertEquals(row1, table.Rows[0]);
			AssertEquals(row2, table.Rows[1]);
		}

		[Test]
		public void ShoudCreateRowsWithAllRequestedCells()
		{
			// Setup
			HtmlTableCell cell1 = new HtmlTableCell();
			cell1.InnerHtml = "Boo";
			HtmlTableCell cell2 = new HtmlTableCell();

			// Execute
			HtmlTableRow row = new DefaultHtmlBuilder().CreateRow(cell1, cell2);

			// Verify
			AssertEquals(2, row.Cells.Count);
			AssertEquals(cell1, row.Cells[0]);
			AssertEquals(cell2, row.Cells[1]);
		}

		[Test]
		public void ShouldCreateCellWithGivenContentAsInnerHtml()
		{
			// Setup
			string content = "<b>Hello</b>";

			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell(content);
			
			// Verify
			AssertEquals(content, cell.InnerHtml);
		}

		[Test]
		public void ShouldCreateCellWithGivenSubControl()
		{
			// Setup
			HtmlTable subControl = new HtmlTable();

			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell(subControl);

			// Verify
			AssertEquals(subControl, cell.Controls[0]);
		}

		[Test]
		public void ShouldCreateAnEmptyCell()
		{
			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell();

			// Verify
			AssertEquals(0, cell.Controls.Count);
			AssertEquals("", cell.InnerHtml);
		}

		[Test]
		public void ShouldCreateTextBoxWithGivenIdAndContent()
		{
			// Execute
			TextBox textBox = new DefaultHtmlBuilder().CreateTextBox("myTextBox", "Hello World");

			// Verify
			AssertEquals("myTextBox", textBox.ID);
			AssertEquals("Hello World", textBox.Text);
		}

		[Test]
		public void ShouldCreateBooleanCheckBoxWithGivenIdAndSetAppropriately()
		{
			// Execute
			CheckBox checkBox = new DefaultHtmlBuilder().CreateBooleanCheckBox("myCheckBox", true);

			// Verify
			AssertEquals("myCheckBox", checkBox.ID);
			AssertEquals(true, checkBox.Checked);

			// Execute
			checkBox = new DefaultHtmlBuilder().CreateBooleanCheckBox("myOtherCheckBox", false);

			// Verify
			AssertEquals("myOtherCheckBox", checkBox.ID);
			AssertEquals(false, checkBox.Checked);
		}

		[Test]
		public void ShouldCreateButtonWithPrefixedIdAndText()
		{
			// Execute
			Button button = new DefaultHtmlBuilder().CreateButton("myButton", "Hello");

			// Verify
			AssertEquals("_action_myButton", button.ID);
			AssertEquals("Hello", button.Text);
		}

		[Test]
		public void ShouldCreateDropDownListWithGivenEntriesAndCorrectOneSelected()
		{
			// Setup
			string[] entries = new string[] {"entry1", "entry2", "entry3"};

			// Execute
			DropDownList dropDownList = new DefaultHtmlBuilder().CreateDropDownList("myDropDown", entries, "entry2");

			// Verify
			AssertEquals("myDropDown", dropDownList.ID);
			AssertEquals(3, dropDownList.Items.Count);
			AssertEquals("entry1", dropDownList.Items[0].Text);
			AssertEquals(false, dropDownList.Items[0].Selected);
			AssertEquals("entry2", dropDownList.Items[1].Text);
			AssertEquals(true, dropDownList.Items[1].Selected);
		}
	}
}
