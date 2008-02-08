using System;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class SelectIconController
	{
		private TextBox entryBox;
		private readonly Button browseButton;
		private OpenFileDialog openFileDialog;
		private CheckBox enabledCheckBox;

		public SelectIconController(
			CheckBox enabledCheckBox,
			TextBox entryBox,
			Button browseButton,
			OpenFileDialog openFileDialog,
			string initialValue)
		{
			this.entryBox = entryBox;
			this.browseButton = browseButton;
			this.openFileDialog = openFileDialog;
			this.enabledCheckBox = enabledCheckBox;

			entryBox.Text = initialValue;
			enabledCheckBox.Checked = entryBox.Text.Length > 0;

			browseButton.Click += new EventHandler(BrowseButton_Click);
			enabledCheckBox.CheckedChanged += new EventHandler(Enabled_Changed);

			UpdateEnabledState(enabledCheckBox.Checked);
		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			FindIconFile(entryBox);
		}

		private void FindIconFile(TextBox textBox)
		{
			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return;

			string fileName = openFileDialog.FileName;

			// make relative path
			if (fileName.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
				fileName = fileName.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);

			textBox.Text = fileName;
		}

		private void Enabled_Changed(object sender, EventArgs e)
		{
			UpdateEnabledState(enabledCheckBox.Checked);
		}

		private void UpdateEnabledState(bool enabled)
		{
			entryBox.Enabled = browseButton.Enabled = enabled;
		}

		public string Value
		{
			get
			{
				if (!enabledCheckBox.Checked)
					return null;

				return entryBox.Text;
			}
		}
	}
}
