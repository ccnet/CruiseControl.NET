using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class SpeechSettingsControl : UserControl
	{
		private SpeechConfiguration currentConfiguration;
		
		public SpeechSettingsControl()
		{
			InitializeComponent();
            UpdateEnabledState();
		}
		
		public void BindSpeechControls(SpeechConfiguration configuration)
		{
            this.currentConfiguration = configuration;
            this.checkBoxSpeechEnabled.Checked = configuration.Enabled;
            this.checkBoxSpeakBuildStartedEvents.Checked = configuration.SpeakBuildStarted;
            this.checkBoxSpeakBuildResultEvents.Checked = configuration.SpeakBuildResults;
		}
		
		public void PersistSpeechTabSettings(SpeechConfiguration configuration)
		{
            configuration.Enabled = this.checkBoxSpeechEnabled.Checked;
            configuration.SpeakBuildStarted = this.checkBoxSpeakBuildStartedEvents.Checked;
            configuration.SpeakBuildResults = this.checkBoxSpeakBuildResultEvents.Checked;

            this.currentConfiguration = configuration;
		}
		
        private void UpdateEnabledState()
        {
            bool controlsEnabled = this.checkBoxSpeechEnabled.Checked;
            this.checkBoxSpeakBuildStartedEvents.Enabled = controlsEnabled;
            this.checkBoxSpeakBuildResultEvents.Enabled = controlsEnabled;
        }
		
		
		void CheckBoxSpeechEnabledCheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
