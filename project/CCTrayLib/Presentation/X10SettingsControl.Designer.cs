using System.Windows.Forms;
using System;


namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class X10SettingsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBoxX10Enabled = new System.Windows.Forms.CheckBox();
			this.listBoxComPort = new System.Windows.Forms.ListBox();
			this.labelComPort = new System.Windows.Forms.Label();
			this.listBoxHouseCode = new System.Windows.Forms.ListBox();
			this.labelHouseCode = new System.Windows.Forms.Label();
			this.listBoxSuccessUnitCode = new System.Windows.Forms.ListBox();
			this.labelSuccessUnit = new System.Windows.Forms.Label();
			this.listBoxFailureUnitCode = new System.Windows.Forms.ListBox();
			this.labelFailureUnitCode = new System.Windows.Forms.Label();
			this.checkBoxActiveMonday = new System.Windows.Forms.CheckBox();
			this.groupBoxActiveDays = new System.Windows.Forms.GroupBox();
			this.checkBoxActiveSunday = new System.Windows.Forms.CheckBox();
			this.checkBoxActiveSaturday = new System.Windows.Forms.CheckBox();
			this.checkBoxActiveFriday = new System.Windows.Forms.CheckBox();
			this.checkBoxActiveThursday = new System.Windows.Forms.CheckBox();
			this.checkBoxActiveWednesday = new System.Windows.Forms.CheckBox();
			this.checkBoxActiveTuesday = new System.Windows.Forms.CheckBox();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.labelStopTime = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.comboBoxControllerType = new System.Windows.Forms.ComboBox();
			this.labelControllerType = new System.Windows.Forms.Label();
			this.dateTimePickerStartTime = new System.Windows.Forms.DateTimePicker();
			this.dateTimePickerEndTime = new System.Windows.Forms.DateTimePicker();
			this.groupBoxActiveDays.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxX10Enabled
			// 
			this.checkBoxX10Enabled.AutoSize = true;
			this.checkBoxX10Enabled.Location = new System.Drawing.Point(8, 8);
			this.checkBoxX10Enabled.Name = "checkBoxX10Enabled";
			this.checkBoxX10Enabled.Size = new System.Drawing.Size(87, 17);
			this.checkBoxX10Enabled.TabIndex = 0;
			this.checkBoxX10Enabled.Text = "X10 &Enabled";
			this.checkBoxX10Enabled.UseVisualStyleBackColor = true;
			this.checkBoxX10Enabled.CheckedChanged += new System.EventHandler(this.checkBoxX10Enabled_CheckedChanged);
			// 
			// listBoxComPort
			// 
			this.listBoxComPort.FormattingEnabled = true;
			this.listBoxComPort.Location = new System.Drawing.Point(8, 54);
			this.listBoxComPort.Name = "listBoxComPort";
			this.listBoxComPort.Size = new System.Drawing.Size(53, 212);
			this.listBoxComPort.TabIndex = 4;
			// 
			// labelComPort
			// 
			this.labelComPort.AutoSize = true;
			this.labelComPort.Location = new System.Drawing.Point(8, 32);
			this.labelComPort.Name = "labelComPort";
			this.labelComPort.Size = new System.Drawing.Size(53, 13);
			this.labelComPort.TabIndex = 3;
			this.labelComPort.Text = "COM &Port";
			// 
			// listBoxHouseCode
			// 
			this.listBoxHouseCode.FormattingEnabled = true;
			this.listBoxHouseCode.Location = new System.Drawing.Point(81, 54);
			this.listBoxHouseCode.Name = "listBoxHouseCode";
			this.listBoxHouseCode.Size = new System.Drawing.Size(63, 212);
			this.listBoxHouseCode.TabIndex = 6;
			// 
			// labelHouseCode
			// 
			this.labelHouseCode.AutoSize = true;
			this.labelHouseCode.Location = new System.Drawing.Point(78, 32);
			this.labelHouseCode.Name = "labelHouseCode";
			this.labelHouseCode.Size = new System.Drawing.Size(66, 13);
			this.labelHouseCode.TabIndex = 5;
			this.labelHouseCode.Text = "&House Code";
			// 
			// listBoxSuccessUnitCode
			// 
			this.listBoxSuccessUnitCode.FormattingEnabled = true;
			this.listBoxSuccessUnitCode.Items.AddRange(new object[] {
									"1",
									"2",
									"3",
									"4",
									"5",
									"6",
									"7",
									"8",
									"9",
									"10",
									"11",
									"12",
									"13",
									"14",
									"15",
									"16"});
			this.listBoxSuccessUnitCode.Location = new System.Drawing.Point(166, 54);
			this.listBoxSuccessUnitCode.Name = "listBoxSuccessUnitCode";
			this.listBoxSuccessUnitCode.Size = new System.Drawing.Size(63, 212);
			this.listBoxSuccessUnitCode.TabIndex = 8;
			// 
			// labelSuccessUnit
			// 
			this.labelSuccessUnit.AutoSize = true;
			this.labelSuccessUnit.Location = new System.Drawing.Point(163, 32);
			this.labelSuccessUnit.Name = "labelSuccessUnit";
			this.labelSuccessUnit.Size = new System.Drawing.Size(98, 13);
			this.labelSuccessUnit.TabIndex = 7;
			this.labelSuccessUnit.Text = "Success Unit Code";
			// 
			// listBoxFailureUnitCode
			// 
			this.listBoxFailureUnitCode.FormattingEnabled = true;
			this.listBoxFailureUnitCode.Items.AddRange(new object[] {
									"1",
									"2",
									"3",
									"4",
									"5",
									"6",
									"7",
									"8",
									"9",
									"10",
									"11",
									"12",
									"13",
									"14",
									"15",
									"16"});
			this.listBoxFailureUnitCode.Location = new System.Drawing.Point(267, 54);
			this.listBoxFailureUnitCode.Name = "listBoxFailureUnitCode";
			this.listBoxFailureUnitCode.Size = new System.Drawing.Size(63, 212);
			this.listBoxFailureUnitCode.TabIndex = 10;
			// 
			// labelFailureUnitCode
			// 
			this.labelFailureUnitCode.AutoSize = true;
			this.labelFailureUnitCode.Location = new System.Drawing.Point(264, 32);
			this.labelFailureUnitCode.Name = "labelFailureUnitCode";
			this.labelFailureUnitCode.Size = new System.Drawing.Size(88, 13);
			this.labelFailureUnitCode.TabIndex = 9;
			this.labelFailureUnitCode.Text = "Failure Unit Code";
			// 
			// checkBoxActiveMonday
			// 
			this.checkBoxActiveMonday.AutoSize = true;
			this.checkBoxActiveMonday.Location = new System.Drawing.Point(15, 19);
			this.checkBoxActiveMonday.Name = "checkBoxActiveMonday";
			this.checkBoxActiveMonday.Size = new System.Drawing.Size(47, 17);
			this.checkBoxActiveMonday.TabIndex = 0;
			this.checkBoxActiveMonday.Text = "&Mon";
			this.checkBoxActiveMonday.UseVisualStyleBackColor = true;
			// 
			// groupBoxActiveDays
			// 
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveSunday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveSaturday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveFriday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveThursday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveWednesday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveTuesday);
			this.groupBoxActiveDays.Controls.Add(this.checkBoxActiveMonday);
			this.groupBoxActiveDays.Location = new System.Drawing.Point(361, 54);
			this.groupBoxActiveDays.Name = "groupBoxActiveDays";
			this.groupBoxActiveDays.Size = new System.Drawing.Size(284, 72);
			this.groupBoxActiveDays.TabIndex = 11;
			this.groupBoxActiveDays.TabStop = false;
			this.groupBoxActiveDays.Text = "Active on these days";
			// 
			// checkBoxActiveSunday
			// 
			this.checkBoxActiveSunday.AutoSize = true;
			this.checkBoxActiveSunday.Location = new System.Drawing.Point(69, 44);
			this.checkBoxActiveSunday.Name = "checkBoxActiveSunday";
			this.checkBoxActiveSunday.Size = new System.Drawing.Size(45, 17);
			this.checkBoxActiveSunday.TabIndex = 6;
			this.checkBoxActiveSunday.Text = "S&un";
			this.checkBoxActiveSunday.UseVisualStyleBackColor = true;
			// 
			// checkBoxActiveSaturday
			// 
			this.checkBoxActiveSaturday.AutoSize = true;
			this.checkBoxActiveSaturday.Location = new System.Drawing.Point(16, 43);
			this.checkBoxActiveSaturday.Name = "checkBoxActiveSaturday";
			this.checkBoxActiveSaturday.Size = new System.Drawing.Size(42, 17);
			this.checkBoxActiveSaturday.TabIndex = 5;
			this.checkBoxActiveSaturday.Text = "&Sat";
			this.checkBoxActiveSaturday.UseVisualStyleBackColor = true;
			// 
			// checkBoxActiveFriday
			// 
			this.checkBoxActiveFriday.AutoSize = true;
			this.checkBoxActiveFriday.Location = new System.Drawing.Point(232, 20);
			this.checkBoxActiveFriday.Name = "checkBoxActiveFriday";
			this.checkBoxActiveFriday.Size = new System.Drawing.Size(37, 17);
			this.checkBoxActiveFriday.TabIndex = 4;
			this.checkBoxActiveFriday.Text = "&Fri";
			this.checkBoxActiveFriday.UseVisualStyleBackColor = true;
			// 
			// checkBoxActiveThursday
			// 
			this.checkBoxActiveThursday.AutoSize = true;
			this.checkBoxActiveThursday.Location = new System.Drawing.Point(177, 20);
			this.checkBoxActiveThursday.Name = "checkBoxActiveThursday";
			this.checkBoxActiveThursday.Size = new System.Drawing.Size(48, 17);
			this.checkBoxActiveThursday.TabIndex = 3;
			this.checkBoxActiveThursday.Text = "Thu&r";
			this.checkBoxActiveThursday.UseVisualStyleBackColor = true;
			// 
			// checkBoxActiveWednesday
			// 
			this.checkBoxActiveWednesday.AutoSize = true;
			this.checkBoxActiveWednesday.Location = new System.Drawing.Point(121, 20);
			this.checkBoxActiveWednesday.Name = "checkBoxActiveWednesday";
			this.checkBoxActiveWednesday.Size = new System.Drawing.Size(49, 17);
			this.checkBoxActiveWednesday.TabIndex = 2;
			this.checkBoxActiveWednesday.Text = "&Wed";
			this.checkBoxActiveWednesday.UseVisualStyleBackColor = true;
			// 
			// checkBoxActiveTuesday
			// 
			this.checkBoxActiveTuesday.AutoSize = true;
			this.checkBoxActiveTuesday.Location = new System.Drawing.Point(69, 20);
			this.checkBoxActiveTuesday.Name = "checkBoxActiveTuesday";
			this.checkBoxActiveTuesday.Size = new System.Drawing.Size(45, 17);
			this.checkBoxActiveTuesday.TabIndex = 1;
			this.checkBoxActiveTuesday.Text = "&Tue";
			this.checkBoxActiveTuesday.UseVisualStyleBackColor = true;
			// 
			// labelStartTime
			// 
			this.labelStartTime.AutoSize = true;
			this.labelStartTime.Location = new System.Drawing.Point(358, 141);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(55, 13);
			this.labelStartTime.TabIndex = 12;
			this.labelStartTime.Text = "Start Time";
			// 
			// labelStopTime
			// 
			this.labelStopTime.AutoSize = true;
			this.labelStopTime.Location = new System.Drawing.Point(358, 167);
			this.labelStopTime.Name = "labelStopTime";
			this.labelStopTime.Size = new System.Drawing.Size(55, 13);
			this.labelStopTime.TabIndex = 14;
			this.labelStopTime.Text = "Stop Time";
			// 
			// labelStatus
			// 
			this.labelStatus.BackColor = System.Drawing.SystemColors.Info;
			this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.ForeColor = System.Drawing.SystemColors.InfoText;
			this.labelStatus.Location = new System.Drawing.Point(330, 8);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(315, 17);
			this.labelStatus.TabIndex = 18;
			this.labelStatus.Text = "this is invisible most of the time";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelStatus.Visible = false;
			// 
			// comboBoxControllerType
			// 
			this.comboBoxControllerType.FormattingEnabled = true;
			this.comboBoxControllerType.Location = new System.Drawing.Point(185, 6);
			this.comboBoxControllerType.Name = "comboBoxControllerType";
			this.comboBoxControllerType.Size = new System.Drawing.Size(139, 21);
			this.comboBoxControllerType.TabIndex = 2;
			// 
			// labelControllerType
			// 
			this.labelControllerType.AutoSize = true;
			this.labelControllerType.Location = new System.Drawing.Point(101, 10);
			this.labelControllerType.Name = "labelControllerType";
			this.labelControllerType.Size = new System.Drawing.Size(78, 13);
			this.labelControllerType.TabIndex = 1;
			this.labelControllerType.Text = "Controller Type";
			// 
			// dateTimePickerStartTime
			// 
			this.dateTimePickerStartTime.CustomFormat = "hh:mm tt";
			this.dateTimePickerStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePickerStartTime.Location = new System.Drawing.Point(419, 137);
			this.dateTimePickerStartTime.Name = "dateTimePickerStartTime";
			this.dateTimePickerStartTime.ShowUpDown = true;
			this.dateTimePickerStartTime.Size = new System.Drawing.Size(86, 20);
			this.dateTimePickerStartTime.TabIndex = 13;
			this.dateTimePickerStartTime.Value = new System.DateTime(2001, 1, 1, 8, 0, 0, 0);
			// 
			// dateTimePickerEndTime
			// 
			this.dateTimePickerEndTime.CustomFormat = "hh:mm tt";
			this.dateTimePickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePickerEndTime.Location = new System.Drawing.Point(419, 163);
			this.dateTimePickerEndTime.Name = "dateTimePickerEndTime";
			this.dateTimePickerEndTime.Size = new System.Drawing.Size(86, 20);
			this.dateTimePickerEndTime.TabIndex = 15;
			this.dateTimePickerEndTime.Value = new System.DateTime(2001, 1, 1, 17, 0, 0, 0);
			// 
			// X10SettingsControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.dateTimePickerEndTime);
			this.Controls.Add(this.dateTimePickerStartTime);
			this.Controls.Add(this.labelControllerType);
			this.Controls.Add(this.comboBoxControllerType);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.labelStopTime);
			this.Controls.Add(this.labelStartTime);
			this.Controls.Add(this.groupBoxActiveDays);
			this.Controls.Add(this.labelFailureUnitCode);
			this.Controls.Add(this.listBoxFailureUnitCode);
			this.Controls.Add(this.labelSuccessUnit);
			this.Controls.Add(this.listBoxSuccessUnitCode);
			this.Controls.Add(this.labelHouseCode);
			this.Controls.Add(this.listBoxHouseCode);
			this.Controls.Add(this.labelComPort);
			this.Controls.Add(this.listBoxComPort);
			this.Controls.Add(this.checkBoxX10Enabled);
			this.Name = "X10SettingsControl";
			this.Size = new System.Drawing.Size(667, 289);
			this.groupBoxActiveDays.ResumeLayout(false);
			this.groupBoxActiveDays.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion

        private CheckBox checkBoxX10Enabled;
        private ListBox listBoxComPort;
        private Label labelComPort;
        private ListBox listBoxHouseCode;
        private Label labelHouseCode;
        private ListBox listBoxSuccessUnitCode;
        private Label labelSuccessUnit;
        private ListBox listBoxFailureUnitCode;
        private Label labelFailureUnitCode;
        private CheckBox checkBoxActiveMonday;
        private GroupBox groupBoxActiveDays;
        private CheckBox checkBoxActiveWednesday;
        private CheckBox checkBoxActiveTuesday;
        private CheckBox checkBoxActiveSunday;
        private CheckBox checkBoxActiveSaturday;
        private CheckBox checkBoxActiveFriday;
        private CheckBox checkBoxActiveThursday;
        private Label labelStartTime;
        private Label labelStopTime;
        private Label labelStatus;
        private ComboBox comboBoxControllerType;
        private Label labelControllerType;
		private System.Windows.Forms.DateTimePicker dateTimePickerEndTime;
		private System.Windows.Forms.DateTimePicker dateTimePickerStartTime;

	}
}
