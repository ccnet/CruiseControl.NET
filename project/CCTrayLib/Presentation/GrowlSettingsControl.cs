using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class GrowlSettingsControl : UserControl
	{
		public GrowlSettingsControl()
		{
			InitializeComponent();
		}

		public void BindGrowlControls(GrowlConfiguration configuration)
		{
			checkBoxEnabled.Checked = configuration.Enabled;
			textBoxPassword.Text = configuration.Password;

			if (!string.IsNullOrEmpty(configuration.Hostname))
			{
				checkBoxRemoteGrowl.Checked = true;
				textBoxHostname.Text = configuration.Hostname;
				textBoxPort.Text = configuration.Port.ToString();
			}
			else
			{
				checkBoxRemoteGrowl.Checked = false;
			}
			BindNotificationLevelCombo(configuration);
			EnableControls();
		}

		public void PersistGrowlTabSettings(GrowlConfiguration configuration)
		{
			configuration.Enabled = checkBoxEnabled.Checked;
			configuration.Password = textBoxPassword.Text;
			if (comboMinNotificationLevel.SelectedIndex > -1 && comboMinNotificationLevel.SelectedIndex < comboMinNotificationLevel.Items.Count)
			{
                configuration.MinimumNotificationLevel = (ToolTipIcon)comboMinNotificationLevel.Items[comboMinNotificationLevel.SelectedIndex];
			}
			else
			{
                configuration.MinimumNotificationLevel = ToolTipIcon.None;
			}

			if (checkBoxRemoteGrowl.Checked)
			{
				configuration.Hostname = textBoxHostname.Text;
				configuration.Port = int.Parse(textBoxPort.Text);
			}
			else
			{
				configuration.Hostname = string.Empty;
			}
		}

		private void BindNotificationLevelCombo(GrowlConfiguration configuration)
		{
			comboMinNotificationLevel.Items.AddRange(
				new object[] { 
                    ErrorLevel.Error.NotifyInfo, 
                    ErrorLevel.Warning.NotifyInfo, 
                    ErrorLevel.Info.NotifyInfo 
                }
			);

			for (int i = 0; i < comboMinNotificationLevel.Items.Count; ++i)
			{
                if (((ToolTipIcon)comboMinNotificationLevel.Items[i]) == configuration.MinimumNotificationLevel)
				{
					comboMinNotificationLevel.SelectedIndex = i;
				}
			}
		}

		private void checkBoxRemoteGrowl_CheckedChanged(object sender, EventArgs e)
		{
			groupBoxRemoteInstance.Enabled = checkBoxRemoteGrowl.Checked;
		}

		private void textBoxPort_Leave(object sender, EventArgs e)
		{
			int tempInt;
			if (!int.TryParse(textBoxPort.Text,out tempInt))
			{
				MessageBox.Show("Port must be a number.", "Invalid input");
				textBoxPort.Focus();
			}
		}

		private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
		{
			EnableControls();
		}

		private void EnableControls()
		{
			bool enabledStatus = checkBoxEnabled.Checked;
			panel1.Enabled = enabledStatus;
		}
	}
}
