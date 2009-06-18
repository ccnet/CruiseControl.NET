using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	/// <summary>
	/// Summary description for Audio.
	/// </summary>
	public class Audio
	{
		///<summary>
		///Plays a sound from a byte array. 
		///Note: If distortion or corruption of audio playback occurs, 
		///try using synchronous playback, or save to a temp file and
		///use the file-based option.
		///</summary>
		public static int PlaySound (byte[] audio, bool bSynchronous, bool bIgnoreErrors)
		{
			return PlaySound (audio, bSynchronous, bIgnoreErrors, false, false, false);
		}

		///<summary>
		///Plays a sound from a byte array. 
		///Note: If distortion or corruption of audio playback occurs, 
		///try using synchronous playback, or save to a temp file and
		///use the file-based option.
		///</summary>
		public static int PlaySound (byte[] audio, bool bSynchronous, bool bIgnoreErrors,
		                             bool bNoDefault, bool bLoop, bool bNoStop)
		{
			const int SND_ASYNC = 1;
			const int SND_NODEFAULT = 2;
			const int SND_MEMORY = 4;
			const int SND_LOOP = 8;
			const int SND_NOSTOP = 16;
			int Snd_Options = SND_MEMORY;
			if (!bSynchronous)
			{
				Snd_Options += SND_ASYNC;
			}
			if (bNoDefault)
				Snd_Options += SND_NODEFAULT;
			if (bLoop)
				Snd_Options += SND_LOOP;
			if (bNoStop)
				Snd_Options += SND_NOSTOP;
			try
			{
				return PlaySound (audio, 0, Snd_Options);
			}
			catch (Exception)
			{
				if (!bIgnoreErrors)
				{
					throw;
				}
				else
				{
					return 0;
				}
			}
		}

		public static int PlaySound (string sSoundFile, bool bSynchronous, bool bIgnoreErrors)
		{
			return PlaySound (sSoundFile, bSynchronous, bIgnoreErrors, false, false, false);
		}

		public static int PlaySound (string sSoundFile, bool bSynchronous, bool bIgnoreErrors,
		                             bool bNoDefault, bool bLoop, bool bNoStop)
		{
			const int SND_ASYNC = 1;
			const int SND_NODEFAULT = 2;
			const int SND_LOOP = 8;
			const int SND_NOSTOP = 16;
			if (!System.IO.File.Exists (sSoundFile))
			{
				string WinDir = System.IO.Directory.GetParent (System.Environment.SystemDirectory).FullName;
				if (WinDir.Substring (WinDir.Length - 1, 1) != "\\")
					WinDir += "\\";
				if (System.IO.File.Exists (Application.StartupPath + "\\" + sSoundFile))
				{
					sSoundFile = Application.StartupPath + "\\" + sSoundFile;
				}
				else if (System.IO.File.Exists (Application.StartupPath + "\\" + sSoundFile + ".wav"))
				{
					sSoundFile = Application.StartupPath + "\\" + sSoundFile + ".wav";
				}
				else if (System.IO.File.Exists (WinDir + "media\\" + sSoundFile))
				{
					sSoundFile = WinDir + "media\\" + sSoundFile;
				}
				else if (System.IO.File.Exists (WinDir + "media\\" + sSoundFile + ".wav"))
				{
					sSoundFile = WinDir + "media\\" + sSoundFile + ".wav";
				}
				else if (!bIgnoreErrors)
				{
					throw new System.IO.FileNotFoundException ("Sound file doesn't exist.");
				}
			}
			int Snd_Options = 0;
			if (!bSynchronous)
			{
				Snd_Options = SND_ASYNC;
			}
			if (bNoDefault)
				Snd_Options += SND_NODEFAULT;
			if (bLoop)
				Snd_Options += SND_LOOP;
			if (bNoStop)
				Snd_Options += SND_NOSTOP;
			try
			{
				return sndPlaySoundA (sSoundFile, Snd_Options);
			}
			catch (Exception)
			{
				if (!bIgnoreErrors)
				{
					throw;
				}
				else
				{
					return 0;
				}
			}
		}

		[DllImport ("winmm.dll")]
		private static extern int sndPlaySoundA (string lpszSoundName, int uFlags);

		[DllImport ("winmm.dll")]
		private static extern int PlaySound (byte[] pszSound, Int16 hMod, long fdwSound);
	}
}