using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class SpeechSettingsControl : UserControl
	{
		public SpeechSettingsControl()
		{
			InitializeComponent();
            UpdateEnabledState();
		}
		
		public void BindSpeechControls(SpeechConfiguration configuration)
		{
            this.checkBoxSpeechEnabled.Checked = configuration.Enabled;
            this.checkBoxSpeakBuildStartedEvents.Checked = configuration.SpeakBuildStarted;
            this.checkBoxSpeakBuildSuccededEvents.Checked = configuration.SpeakBuildSucceded;
            this.checkBoxSpeakBuildFailedEvents.Checked = configuration.SpeakBuildFailed;
		}
		
		public void PersistSpeechTabSettings(SpeechConfiguration configuration)
		{
            configuration.Enabled = this.checkBoxSpeechEnabled.Checked;
            configuration.SpeakBuildStarted = this.checkBoxSpeakBuildStartedEvents.Checked;
            configuration.SpeakBuildSucceded = this.checkBoxSpeakBuildSuccededEvents.Checked;
            configuration.SpeakBuildFailed = this.checkBoxSpeakBuildFailedEvents.Checked;
		}
		
        private void UpdateEnabledState()
        {
            bool controlsEnabled = this.checkBoxSpeechEnabled.Checked;
            this.checkBoxSpeakBuildStartedEvents.Enabled = controlsEnabled;
            this.checkBoxSpeakBuildSuccededEvents.Enabled = controlsEnabled;
            this.checkBoxSpeakBuildFailedEvents.Enabled = controlsEnabled;
        }
		
		
		void CheckBoxSpeechEnabledCheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
