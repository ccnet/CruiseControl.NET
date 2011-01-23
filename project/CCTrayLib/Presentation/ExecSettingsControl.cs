using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class ExecSettingsControl : UserControl
	{
		public ExecSettingsControl()
		{
			InitializeComponent();
		}

		public void BindExecControls(ICCTrayMultiConfiguration configuration)
		{
			ExecCommands config = configuration.Execs;

			txtExecSuccess.Text = config.SuccessCommand;
			txtExecBroken.Text = config.BrokenCommand;
			txtExecBrokenAndBuilding.Text = config.BrokenAndBuildingCommand;
			txtExecBuilding.Text = config.BuildingCommand;
			txtExecNotConnected.Text = config.NotConnectedCommand;
		}

		public void PersistExecTabSettings(ICCTrayMultiConfiguration configuration)
		{
			configuration.Execs.SuccessCommand = txtExecSuccess.Text;
			configuration.Execs.BrokenCommand = txtExecBroken.Text;
			configuration.Execs.BrokenAndBuildingCommand = txtExecBrokenAndBuilding.Text;
			configuration.Execs.BuildingCommand = txtExecBuilding.Text;
			configuration.Execs.NotConnectedCommand = txtExecNotConnected.Text;
		}
	}
}
