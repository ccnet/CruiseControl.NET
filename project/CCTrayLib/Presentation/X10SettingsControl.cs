using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class X10SettingsControl : UserControl
	{
        private X10Configuration currentConfiguration;

		public X10SettingsControl()
		{
			InitializeComponent();
            UpdateEnabledState();
		}

		public void BindX10Controls(X10Configuration configuration)
		{
            this.currentConfiguration = configuration;
            this.checkBoxX10Enabled.Checked = configuration.Enabled;

            String[] houseCodes = Enum.GetNames(typeof(ThoughtWorks.CruiseControl.CCTrayLib.X10.HouseCode));
            Array.Sort(houseCodes);
            this.listBoxHouseCode.Items.AddRange(houseCodes);
            this.listBoxHouseCode.SelectedItem = configuration.HouseCode;

            String[] deviceTypes = Enum.GetNames(typeof(ThoughtWorks.CruiseControl.CCTrayLib.X10.ControllerType));
            Array.Sort(deviceTypes);
            this.comboBoxControllerType.Items.AddRange(deviceTypes);
            this.comboBoxControllerType.SelectedItem = configuration.DeviceType;

            String[] availableComPorts = System.IO.Ports.SerialPort.GetPortNames();
            this.listBoxComPort.Items.AddRange(availableComPorts);
            if (availableComPorts.Length == 0)
            {
                labelStatus.Text = "Problem: no COM ports found!";
                labelStatus.Visible = true;
                this.checkBoxX10Enabled.Checked = false;
                this.checkBoxX10Enabled.Enabled = false;
                UpdateEnabledState();
                return;
            }
            this.comboBoxControllerType.SelectedItem = configuration.DeviceType;
            this.listBoxComPort.SelectedItem = configuration.ComPort;

            this.listBoxFailureUnitCode.SelectedIndex = configuration.FailureUnitCode - 1;
            this.listBoxSuccessUnitCode.SelectedIndex = configuration.SuccessUnitCode - 1;

            this.checkBoxActiveSunday.Checked = configuration.ActiveDays[(int)DayOfWeek.Sunday];
            this.checkBoxActiveMonday.Checked = configuration.ActiveDays[(int)DayOfWeek.Monday];
            this.checkBoxActiveTuesday.Checked = configuration.ActiveDays[(int)DayOfWeek.Tuesday];
            this.checkBoxActiveWednesday.Checked = configuration.ActiveDays[(int)DayOfWeek.Wednesday];
            this.checkBoxActiveThursday.Checked = configuration.ActiveDays[(int)DayOfWeek.Thursday];
            this.checkBoxActiveFriday.Checked = configuration.ActiveDays[(int)DayOfWeek.Friday];
            this.checkBoxActiveSaturday.Checked = configuration.ActiveDays[(int)DayOfWeek.Saturday];

            this.dateTimePickerStartTime.Value = timeWithGoodDate(configuration.StartTime);
            this.dateTimePickerEndTime.Value = timeWithGoodDate(configuration.EndTime);
        }

		public void PersistX10TabSettings(X10Configuration configuration)
		{
            configuration.Enabled = this.checkBoxX10Enabled.Checked;
            String comPort = "COM0";
            if (this.listBoxComPort.SelectedItem != null)
            {
                comPort = this.listBoxComPort.SelectedItem.ToString();
            }
            configuration.ComPort = comPort;
            configuration.DeviceType = this.comboBoxControllerType.SelectedItem.ToString();
            configuration.HouseCode = this.listBoxHouseCode.SelectedItem.ToString();
            configuration.StartTime = this.dateTimePickerStartTime.Value;
            configuration.EndTime = this.dateTimePickerEndTime.Value;
            configuration.FailureUnitCode = this.listBoxFailureUnitCode.SelectedIndex+1;
            configuration.SuccessUnitCode = this.listBoxSuccessUnitCode.SelectedIndex+1;
            configuration.ActiveDays[(int)DayOfWeek.Sunday] = this.checkBoxActiveSunday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Monday] = this.checkBoxActiveMonday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Tuesday] = this.checkBoxActiveTuesday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Wednesday] = this.checkBoxActiveWednesday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Thursday] = this.checkBoxActiveThursday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Friday] = this.checkBoxActiveFriday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Saturday] = this.checkBoxActiveSaturday.Checked;
            this.currentConfiguration = configuration;
        }

        private void UpdateEnabledState()
        {
            bool controlsEnabled = this.checkBoxX10Enabled.Checked;
            this.listBoxComPort.Enabled = controlsEnabled;
            this.listBoxFailureUnitCode.Enabled = controlsEnabled;
            this.listBoxSuccessUnitCode.Enabled = controlsEnabled;
            this.listBoxHouseCode.Enabled = controlsEnabled;
            this.checkBoxActiveSunday.Enabled = controlsEnabled;
            this.checkBoxActiveMonday.Enabled = controlsEnabled;
            this.checkBoxActiveTuesday.Enabled = controlsEnabled;
            this.checkBoxActiveWednesday.Enabled = controlsEnabled;
            this.checkBoxActiveThursday.Enabled = controlsEnabled;
            this.checkBoxActiveFriday.Enabled = controlsEnabled;
            this.checkBoxActiveSaturday.Enabled = controlsEnabled;
            this.labelComPort.Enabled = controlsEnabled;
            this.labelFailureUnitCode.Enabled = controlsEnabled;
            this.labelHouseCode.Enabled = controlsEnabled;
            this.labelStartTime.Enabled = controlsEnabled;
            this.labelStopTime.Enabled = controlsEnabled;
            this.labelSuccessUnit.Enabled = controlsEnabled;
            this.groupBoxActiveDays.Enabled = controlsEnabled;
            this.labelControllerType.Enabled = controlsEnabled;
            this.comboBoxControllerType.Enabled = controlsEnabled;

        }

        private void checkBoxX10Enabled_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateEnabledState();
        }

        
        private DateTime timeWithGoodDate(DateTime timeValue)
        {
        	return new DateTime(2001,1,1,timeValue.Hour,timeValue.Minute,timeValue.Second);
        }

	}
}
