using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class DefaultHtmlBuilderTest
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
			Assert.AreEqual(2, table.Rows.Count);
			Assert.AreEqual(row1, table.Rows[0]);
			Assert.AreEqual(row2, table.Rows[1]);
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
			Assert.AreEqual(2, row.Cells.Count);
			Assert.AreEqual(cell1, row.Cells[0]);
			Assert.AreEqual(cell2, row.Cells[1]);
		}

		[Test]
		public void ShouldCreateCellWithGivenContentAsInnerHtml()
		{
			// Setup
			string content = "<b>Hello</b>";

			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell(content);
			
			// Verify
			Assert.AreEqual(content, cell.InnerHtml);
		}

		[Test]
		public void ShouldCreateCellWithGivenSubControl()
		{
			// Setup
			HtmlTable subControl = new HtmlTable();

			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell(subControl);

			// Verify
			Assert.AreEqual(subControl, cell.Controls[0]);
		}

		[Test]
		public void ShouldCreateAnEmptyCell()
		{
			// Execute
			HtmlTableCell cell = new DefaultHtmlBuilder().CreateCell();

			// Verify
			Assert.AreEqual(0, cell.Controls.Count);
			Assert.AreEqual("", cell.InnerHtml);
		}

		[Test]
		public void ShouldCreateSimpleTextBoxWithGivenIdAndContent()
		{
			// Execute
			TextBox textBox = new DefaultHtmlBuilder().CreateTextBox("myTextBox", "Hello World");

			// Verify
			Assert.AreEqual("myTextBox", textBox.ID);
			Assert.AreEqual("Hello World", textBox.Text);
			Assert.AreEqual(TextBoxMode.SingleLine , textBox.TextMode);
		}

		[Test]
		public void ShouldCreateMultiLineTextBoxWithGivenIdAndContent()
		{
			// Execute
			TextBox textBox = new DefaultHtmlBuilder().CreateMultiLineTextBox("myTextBox", "Hello World");

			// Verify
			Assert.AreEqual("myTextBox", textBox.ID);
			Assert.AreEqual("Hello World", textBox.Text);
			Assert.AreEqual(TextBoxMode.MultiLine , textBox.TextMode);
		}

		[Test]
		public void ShouldCreateBooleanCheckBoxWithGivenIdAndSetAppropriately()
		{
			// Execute
			CheckBox checkBox = new DefaultHtmlBuilder().CreateBooleanCheckBox("myCheckBox", true);

			// Verify
			Assert.AreEqual("myCheckBox", checkBox.ID);
			Assert.AreEqual(true, checkBox.Checked);

			// Execute
			checkBox = new DefaultHtmlBuilder().CreateBooleanCheckBox("myOtherCheckBox", false);

			// Verify
			Assert.AreEqual("myOtherCheckBox", checkBox.ID);
			Assert.AreEqual(false, checkBox.Checked);
		}

		[Test]
		public void ShouldCreateButtonWithPrefixedIdAndText()
		{
			// Execute
			Button button = new DefaultHtmlBuilder().CreateButton("myButton", "Hello");

			// Verify
			Assert.AreEqual("_action_myButton", button.ID);
			Assert.AreEqual("Hello", button.Text);
		}

		[Test]
		public void ShouldCreateDropDownListWithGivenEntriesAndCorrectOneSelected()
		{
			// Setup
			string[] entries = new string[] {"entry1", "entry2", "entry3"};

			// Execute
			DropDownList dropDownList = new DefaultHtmlBuilder().CreateDropDownList("myDropDown", entries, "entry2");

			// Verify
			Assert.AreEqual("myDropDown", dropDownList.ID);
			Assert.AreEqual(3, dropDownList.Items.Count);
			Assert.AreEqual("entry1", dropDownList.Items[0].Text);
			Assert.AreEqual(false, dropDownList.Items[0].Selected);
			Assert.AreEqual("entry2", dropDownList.Items[1].Text);
			Assert.AreEqual(true, dropDownList.Items[1].Selected);
		}

		[Test]
		public void ShouldCreateAnchorWithGivenTextAndUrl()
		{
			// Execute
			HtmlAnchor anchor = new DefaultHtmlBuilder().CreateAnchor("hello world", "helloworld.htm;");

			// Verify
			Assert.AreEqual("hello world", anchor.InnerHtml);
			Assert.AreEqual("helloworld.htm;", anchor.HRef);
		}
	}
}
