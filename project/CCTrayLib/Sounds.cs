using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	/// <summary>
	/// Summary description for Sounds.
	/// </summary>
	public class Sounds
	{
		public Sound AnotherSuccessfulBuildSound;
		public Sound AnotherFailedBuildSound;
		public Sound BrokenBuildSound;
		public Sound FixedBuildSound;

		public static Sounds CreateDefaultSettings()
		{
			Sounds defaults = new Sounds();
			defaults.AnotherSuccessfulBuildSound = new Sound("still-successful.wav");
			defaults.AnotherFailedBuildSound = new Sound("still-failing.wav");
			defaults.BrokenBuildSound = new Sound("broken.wav");
			defaults.FixedBuildSound = new Sound("fixed.wav");
			return defaults;
		}

		//TODO Sreekanth Find a better way for mapping... maybe compile time????

		private Sound For(BuildTransition transition)
		{
			if (transition == BuildTransition.Broken)
				return BrokenBuildSound;

			if (transition == BuildTransition.Fixed)
				return FixedBuildSound;

			if (transition == BuildTransition.StillFailing)
				return AnotherFailedBuildSound;

			if (transition == BuildTransition.StillSuccessful)
				return AnotherSuccessfulBuildSound;

			throw new Exception("Unsupported build transition.");
		}

		public void PlayFor(BuildTransition transition)
		{
			if (For(transition).Play)
			{
				Stream stream = new FileStream(For(transition).FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				Audio.PlaySound(bytes, true, true);
			}
		}
	}
}