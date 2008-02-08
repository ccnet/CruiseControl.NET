using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class IconSettingsControl : UserControl
	{
		private SelectIconController successIcon;
		private SelectIconController brokenIcon;
		private SelectIconController brokenAndBuildingIcon;
		private SelectIconController buildingIcon;
		private SelectIconController notConnectedIcon;
		
		public IconSettingsControl()
		{
			InitializeComponent();
		}

		public void BindIconControls(ICCTrayMultiConfiguration configuration)
		{
			Icons iconConfig = configuration.Icons;

			successIcon = new SelectIconController(
				chkIconSuccess, txtIconSuccess, btnSuccessBrowse, dlgOpenFile, iconConfig.SuccessIcon);
			brokenIcon = new SelectIconController(
				chkIconBroken, txtIconBroken, btnBrokenBrowse, dlgOpenFile, iconConfig.BrokenIcon);
			brokenAndBuildingIcon = new SelectIconController(
				chkIconBrokenAndBuilding, txtIconBrokenAndBuilding, btnBrokenAndBuildingBrowse, dlgOpenFile,
				iconConfig.BrokenAndBuildingIcon);
			buildingIcon = new SelectIconController(
				chkIconBuilding, txtIconBuilding, btnBuildingBrowse, dlgOpenFile,
				iconConfig.BuildingIcon);
			notConnectedIcon = new SelectIconController(
				chkIconNotConnected, txtIconNotConnected, btnNotConnectedBrowse, dlgOpenFile,
				iconConfig.NotConnectedIcon);
		}

		public void PersistIconTabSettings(ICCTrayMultiConfiguration configuration)
		{
			configuration.Icons.SuccessIcon = successIcon.Value;
			configuration.Icons.BrokenIcon = brokenIcon.Value;
			configuration.Icons.BrokenAndBuildingIcon = brokenAndBuildingIcon.Value;
			configuration.Icons.BuildingIcon = buildingIcon.Value;
			configuration.Icons.NotConnectedIcon = notConnectedIcon.Value;
		}
	}
}
