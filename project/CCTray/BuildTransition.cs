using System;
using System.Reflection;

namespace tw.ccnet.remote.monitor
{
	public enum BuildTransition
	{
		[BuildTransition("Broken build", "The most recent checkins have broken the build.", ErrorLevel.Error)]
		Broken,

		[BuildTransition("Fixed build", "The most recent checkins have fixed the build.", ErrorLevel.Info)]
		Fixed,

		[BuildTransition("Build successful", "Yet another successful build!", ErrorLevel.Info)]
		StillSuccessful,

		[BuildTransition("Build still failing", "The build is still broken...", ErrorLevel.Warning)]
		StillFailing
	}

	public enum ErrorLevel
	{
		Info,
		Warning,
		Error
	}

	public class BuildTransitionAttribute : Attribute
	{
		public string Caption;
		public string Description;
		public ErrorLevel ErrorLevel;

		public BuildTransitionAttribute(string caption, string description, ErrorLevel messageType)
		{
			Caption = caption;
			Description = description;
			ErrorLevel = messageType;
		}
	}

	internal class BuildTransitionUtil
	{
		public static BuildTransitionAttribute GetBuildTransitionAttribute(BuildTransition transition)
		{
			FieldInfo fieldInfo = transition.GetType().GetField(transition.ToString());
			BuildTransitionAttribute[] attributes = 
				(BuildTransitionAttribute[])fieldInfo.GetCustomAttributes(typeof(BuildTransitionAttribute), false);
			return (attributes.Length>0) ? attributes[0] : null;
		}
	}
}
