using System;
using System.IO;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class SelectAudioFileController
	{
		private TextBox entryBox;
		private readonly Button browseButton;
		private readonly Button playButton;
		private OpenFileDialog openFileDialog;
		private CheckBox enabledCheckBox;

		public SelectAudioFileController(
			CheckBox enabledCheckBox,
			TextBox entryBox,
			Button browseButton,
			Button playButton,
			OpenFileDialog openFileDialog,
			string initialValue)
		{
			this.entryBox = entryBox;
			this.browseButton = browseButton;
			this.playButton = playButton;
			this.openFileDialog = openFileDialog;
			this.enabledCheckBox = enabledCheckBox;

			entryBox.Text = initialValue;
			enabledCheckBox.Checked = entryBox.Text.Length > 0;

			browseButton.Click += new EventHandler(BrowseButton_Click);
			playButton.Click += new EventHandler(PlayButton_Click);
			enabledCheckBox.CheckedChanged += new EventHandler(Enabled_Changed);

			UpdateEnabledState(enabledCheckBox.Checked);
		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			FindAudioFile(entryBox);
		}

		private void FindAudioFile(TextBox textBox)
		{
			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return;

			string fileName = openFileDialog.FileName;

			// make relative path
			if (fileName.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
				fileName = fileName.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);

			textBox.Text = fileName;
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			PlayAudioFile(entryBox.Text);
		}

		private void PlayAudioFile(string fileName)
		{
			if (fileName == null || fileName.Trim().Length == 0)
				return;

			if (!File.Exists(fileName))
			{
				MessageBox.Show("The specified audio file was not found.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Audio.PlaySound(fileName, false, true);
		}

		private void Enabled_Changed(object sender, EventArgs e)
		{
			UpdateEnabledState(enabledCheckBox.Checked);
		}

		private void UpdateEnabledState(bool enabled)
		{
			entryBox.Enabled = browseButton.Enabled = playButton.Enabled = enabled;
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