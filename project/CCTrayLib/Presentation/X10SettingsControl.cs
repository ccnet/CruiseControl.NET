using System;
using System.IO.Ports;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class X10SettingsControl : UserControl
	{
		public X10SettingsControl()
		{
			InitializeComponent();
            UpdateEnabledState();
		}

		public void BindX10Controls(X10Configuration configuration)
		{
            checkBoxX10Enabled.Checked = configuration.Enabled;

            String[] houseCodes = Enum.GetNames(typeof(HouseCode));
            Array.Sort(houseCodes);
            listBoxHouseCode.Items.AddRange(houseCodes);
            listBoxHouseCode.SelectedItem = configuration.HouseCode;

            String[] deviceTypes = Enum.GetNames(typeof(ControllerType));
            Array.Sort(deviceTypes);
            comboBoxControllerType.Items.AddRange(deviceTypes);
            comboBoxControllerType.SelectedItem = configuration.DeviceType;

            String[] availableComPorts = SerialPort.GetPortNames();
            listBoxComPort.Items.AddRange(availableComPorts);
            if (availableComPorts.Length == 0)
            {
                labelStatus.Text = "Problem: no COM ports found!";
                labelStatus.Visible = true;
                checkBoxX10Enabled.Checked = false;
                checkBoxX10Enabled.Enabled = false;
                UpdateEnabledState();
                return;
            }
            comboBoxControllerType.SelectedItem = configuration.DeviceType;
            listBoxComPort.SelectedItem = configuration.ComPort;

            listBoxFailureUnitCode.SelectedIndex = configuration.FailureUnitCode - 1;
            listBoxBuildingUnitCode.SelectedIndex = configuration.BuildingUnitCode -1;
            listBoxSuccessUnitCode.SelectedIndex = configuration.SuccessUnitCode - 1;

            checkBoxActiveSunday.Checked = configuration.ActiveDays[(int)DayOfWeek.Sunday];
            checkBoxActiveMonday.Checked = configuration.ActiveDays[(int)DayOfWeek.Monday];
            checkBoxActiveTuesday.Checked = configuration.ActiveDays[(int)DayOfWeek.Tuesday];
            checkBoxActiveWednesday.Checked = configuration.ActiveDays[(int)DayOfWeek.Wednesday];
            checkBoxActiveThursday.Checked = configuration.ActiveDays[(int)DayOfWeek.Thursday];
            checkBoxActiveFriday.Checked = configuration.ActiveDays[(int)DayOfWeek.Friday];
            checkBoxActiveSaturday.Checked = configuration.ActiveDays[(int)DayOfWeek.Saturday];

            dateTimePickerStartTime.Value = timeWithGoodDate(configuration.StartTime);
            dateTimePickerEndTime.Value = timeWithGoodDate(configuration.EndTime);
        }

		public void PersistX10TabSettings(X10Configuration configuration)
		{
            configuration.Enabled = checkBoxX10Enabled.Checked;
			configuration.ComPort = (listBoxComPort.SelectedItem ?? "COM0").ToString();
            configuration.DeviceType = comboBoxControllerType.SelectedItem.ToString();
            configuration.HouseCode = listBoxHouseCode.SelectedItem.ToString();
            configuration.StartTime = dateTimePickerStartTime.Value;
            configuration.EndTime = dateTimePickerEndTime.Value;
            configuration.FailureUnitCode = listBoxFailureUnitCode.SelectedIndex+1;
            configuration.BuildingUnitCode = listBoxBuildingUnitCode.SelectedIndex+1;
            configuration.SuccessUnitCode = listBoxSuccessUnitCode.SelectedIndex+1;
            configuration.ActiveDays[(int)DayOfWeek.Sunday] = checkBoxActiveSunday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Monday] = checkBoxActiveMonday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Tuesday] = checkBoxActiveTuesday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Wednesday] = checkBoxActiveWednesday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Thursday] = checkBoxActiveThursday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Friday] = checkBoxActiveFriday.Checked;
            configuration.ActiveDays[(int)DayOfWeek.Saturday] = checkBoxActiveSaturday.Checked;
        }

        private void UpdateEnabledState()
        {
            bool controlsEnabled = checkBoxX10Enabled.Checked;
            listBoxComPort.Enabled = controlsEnabled;
            listBoxFailureUnitCode.Enabled = controlsEnabled;
            listBoxBuildingUnitCode.Enabled = controlsEnabled;
            listBoxSuccessUnitCode.Enabled = controlsEnabled;
            listBoxHouseCode.Enabled = controlsEnabled;
            checkBoxActiveSunday.Enabled = controlsEnabled;
            checkBoxActiveMonday.Enabled = controlsEnabled;
            checkBoxActiveTuesday.Enabled = controlsEnabled;
            checkBoxActiveWednesday.Enabled = controlsEnabled;
            checkBoxActiveThursday.Enabled = controlsEnabled;
            checkBoxActiveFriday.Enabled = controlsEnabled;
            checkBoxActiveSaturday.Enabled = controlsEnabled;
            labelComPort.Enabled = controlsEnabled;
            labelHouseCode.Enabled = controlsEnabled;
            labelSuccessUnit.Enabled = controlsEnabled;
            labelBuildingUnit.Enabled = controlsEnabled;
            labelFailureUnit.Enabled = controlsEnabled;
            groupBoxActiveDays.Enabled = controlsEnabled;
            labelControllerType.Enabled = controlsEnabled;
            comboBoxControllerType.Enabled = controlsEnabled;
            labelStartTime.Enabled = controlsEnabled;
            labelStopTime.Enabled = controlsEnabled;
            dateTimePickerEndTime.Enabled = controlsEnabled;
            dateTimePickerStartTime.Enabled = controlsEnabled;

        }

        private void checkBoxX10Enabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        
        private DateTime timeWithGoodDate(DateTime timeValue)
        {
        	return new DateTime(2001,1,1,timeValue.Hour,timeValue.Minute,timeValue.Second);
        }

	}
}
