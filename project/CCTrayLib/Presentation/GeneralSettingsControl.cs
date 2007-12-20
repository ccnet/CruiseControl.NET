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
	public partial class GeneralSettingsControl : UserControl
	{
		public GeneralSettingsControl()
		{
			InitializeComponent();
		}

		public void BindGeneralTabControls(ICCTrayMultiConfiguration configuration)
		{
			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");
			chkAlwaysOnTop.DataBindings.Add("Checked", configuration, "AlwaysOnTop");
			chkShowInTaskbar.DataBindings.Add("Checked", configuration, "ShowInTaskbar");

			rdoStatusWindow.Checked = (configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.ShowStatusWindow);
			rdoWebPage.Checked =
				(configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);

			numPollPeriod.Value = configuration.PollPeriodSeconds;

			txtFixUserName.DataBindings.Add("Text", configuration, "FixUserName");
		}

		public void PersistGeneralTabSettings(ICCTrayMultiConfiguration configuration)
		{
			configuration.ShouldShowBalloonOnBuildTransition = chkShowBalloons.Checked;
			configuration.AlwaysOnTop = chkAlwaysOnTop.Checked;
			configuration.PollPeriodSeconds = (int)numPollPeriod.Value;
			configuration.TrayIconDoubleClickAction =
				(rdoStatusWindow.Checked
					? TrayIconDoubleClickAction.ShowStatusWindow
					: TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);
			configuration.FixUserName = txtFixUserName.Text;
		}

	}
}
