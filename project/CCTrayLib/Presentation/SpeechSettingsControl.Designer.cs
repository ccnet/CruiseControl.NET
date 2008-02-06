/*
 * Created by SharpDevelop.
 * User: sdonie
 * Date: 1/25/2008
 * Time: 9:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class SpeechSettingsControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBoxSpeechEnabled = new System.Windows.Forms.CheckBox();
			this.checkBoxSpeakBuildStartedEvents = new System.Windows.Forms.CheckBox();
			this.checkBoxSpeakBuildResultEvents = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBoxSpeechEnabled
			// 
			this.checkBoxSpeechEnabled.Location = new System.Drawing.Point(8, 8);
			this.checkBoxSpeechEnabled.Name = "checkBoxSpeechEnabled";
			this.checkBoxSpeechEnabled.Size = new System.Drawing.Size(190, 24);
			this.checkBoxSpeechEnabled.TabIndex = 0;
			this.checkBoxSpeechEnabled.Text = "&Speech Enabled";
			this.checkBoxSpeechEnabled.UseVisualStyleBackColor = true;
			this.checkBoxSpeechEnabled.CheckedChanged += new System.EventHandler(this.CheckBoxSpeechEnabledCheckedChanged);
			// 
			// checkBoxSpeakBuildStartedEvents
			// 
			this.checkBoxSpeakBuildStartedEvents.Location = new System.Drawing.Point(27, 39);
			this.checkBoxSpeakBuildStartedEvents.Name = "checkBoxSpeakBuildStartedEvents";
			this.checkBoxSpeakBuildStartedEvents.Size = new System.Drawing.Size(210, 24);
			this.checkBoxSpeakBuildStartedEvents.TabIndex = 1;
			this.checkBoxSpeakBuildStartedEvents.Text = "Speak Build &Started Events";
			this.checkBoxSpeakBuildStartedEvents.UseVisualStyleBackColor = true;
			// 
			// checkBoxSpeakBuildResultEvents
			// 
			this.checkBoxSpeakBuildResultEvents.Location = new System.Drawing.Point(27, 70);
			this.checkBoxSpeakBuildResultEvents.Name = "checkBoxSpeakBuildResultEvents";
			this.checkBoxSpeakBuildResultEvents.Size = new System.Drawing.Size(210, 24);
			this.checkBoxSpeakBuildResultEvents.TabIndex = 2;
			this.checkBoxSpeakBuildResultEvents.Text = "Speak Build &Result Events";
			this.checkBoxSpeakBuildResultEvents.UseVisualStyleBackColor = true;
			// 
			// SpeechSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkBoxSpeakBuildResultEvents);
			this.Controls.Add(this.checkBoxSpeakBuildStartedEvents);
			this.Controls.Add(this.checkBoxSpeechEnabled);
			this.Name = "SpeechSettingsControl";
			this.Size = new System.Drawing.Size(667, 289);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox checkBoxSpeakBuildResultEvents;
		private System.Windows.Forms.CheckBox checkBoxSpeakBuildStartedEvents;
		private System.Windows.Forms.CheckBox checkBoxSpeechEnabled;
	}
}
