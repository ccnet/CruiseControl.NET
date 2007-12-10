using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class AudioSettingsControl : UserControl
	{
		private SelectAudioFileController brokenAudio;
		private SelectAudioFileController fixedAudio;
		private SelectAudioFileController stillFailingAudio;
		private SelectAudioFileController successfulAudio;

		public AudioSettingsControl()
		{
			InitializeComponent();
		}

		public void BindAudioControls(ICCTrayMultiConfiguration configuration)
		{
			AudioFiles audioConfig = configuration.Audio;

			brokenAudio = new SelectAudioFileController(
				chkAudioBroken, txtAudioFileBroken, btnBrokenBrowse, btnBrokenPlay, dlgOpenFile, audioConfig.BrokenBuildSound);
			fixedAudio = new SelectAudioFileController(
				chkAudioFixed, txtAudioFileFixed, btnFixedBrowse, btnFixedPlay, dlgOpenFile, audioConfig.FixedBuildSound);
			stillFailingAudio = new SelectAudioFileController(
				chkAudioStillFailing, txtAudioFileFailing, btnStillFailingBrowse, btnStillFailingPlay, dlgOpenFile,
				audioConfig.StillFailingBuildSound);
			successfulAudio = new SelectAudioFileController(
				chkAudioSuccessful, txtAudioFileSuccess, btnSuccessfulBrowse, btnSuccessfulPlay, dlgOpenFile,
				audioConfig.StillSuccessfulBuildSound);
		}

		public void PersistAudioTabSettings(ICCTrayMultiConfiguration configuration)
		{
			configuration.Audio.BrokenBuildSound = brokenAudio.Value;
			configuration.Audio.FixedBuildSound = fixedAudio.Value;
			configuration.Audio.StillFailingBuildSound = stillFailingAudio.Value;
			configuration.Audio.StillSuccessfulBuildSound = successfulAudio.Value;
            configuration.Persist();
		}
	}
}
