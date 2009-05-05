using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class GeneralSettingsControl : UserControl
	{
		public GeneralSettingsControl()
		{
			InitializeComponent();
		}

		public void BindGeneralTabControls(ICCTrayMultiConfiguration configuration)
		{
            chkShowBalloons.DataBindings.Clear();
			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");
            chkAlwaysOnTop.DataBindings.Clear();
			chkAlwaysOnTop.DataBindings.Add("Checked", configuration, "AlwaysOnTop");
            chkShowInTaskbar.DataBindings.Clear();
			chkShowInTaskbar.DataBindings.Add("Checked", configuration, "ShowInTaskbar");

			rdoStatusWindow.Checked = (configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.ShowStatusWindow);
			rdoWebPage.Checked =
				(configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);

			if (configuration.PollPeriodSeconds <= numPollPeriod.Minimum)
            {
                configuration.PollPeriodSeconds = (int)numPollPeriod.Minimum; 
            }
            numPollPeriod.Value = configuration.PollPeriodSeconds;

            txtFixUserName.DataBindings.Clear();
			txtFixUserName.DataBindings.Add("Text", configuration, "FixUserName");
            BindNotificationLevelCombo(configuration);
		}

		public void PersistGeneralTabSettings(ICCTrayMultiConfiguration configuration)
		{
			configuration.ShouldShowBalloonOnBuildTransition = chkShowBalloons.Checked;
            configuration.MinimumNotificationLevel = (NotifyInfoFlags)comboBalloonMinNotificationLevel.Items[comboBalloonMinNotificationLevel.SelectedIndex];

			configuration.AlwaysOnTop = chkAlwaysOnTop.Checked;
			configuration.PollPeriodSeconds = (int)numPollPeriod.Value;
			configuration.TrayIconDoubleClickAction =
				(rdoStatusWindow.Checked
					? TrayIconDoubleClickAction.ShowStatusWindow
					: TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);
			configuration.FixUserName = txtFixUserName.Text;
		}

        private void chkShowBalloons_CheckedChanged(object sender, EventArgs e)
        {
            labelBalloonMinNotificationLevel.Enabled = chkShowBalloons.Checked;
            comboBalloonMinNotificationLevel.Enabled = chkShowBalloons.Checked;
        }

        private void BindNotificationLevelCombo(ICCTrayMultiConfiguration configuration)
        {
            comboBalloonMinNotificationLevel.Items.AddRange(
                new object[] { 
                    ErrorLevel.Error.NotifyInfo, 
                    ErrorLevel.Warning.NotifyInfo, 
                    ErrorLevel.Info.NotifyInfo 
                }
            );

            for (int i = 0; i < comboBalloonMinNotificationLevel.Items.Count; ++i)
            {
                if (((NotifyInfoFlags)comboBalloonMinNotificationLevel.Items[i]) == configuration.MinimumNotificationLevel)
                {
                    comboBalloonMinNotificationLevel.SelectedIndex = i;
                }
            }
        }

	}
}
