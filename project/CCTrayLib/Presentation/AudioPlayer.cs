using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class AudioPlayer : IAudioPlayer
	{
		public void Play(string filename)
		{
			// probably makes sense to move the code from the static "audio" class in there in due course
			Debug.WriteLine("Playing: " + filename);
			Audio.PlaySound(filename, false, true);
		}
	}
}