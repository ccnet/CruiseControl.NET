/*
 * Created by SharpDevelop.
 * User: sdonie
 * Date: 2/27/2009
 * Time: 1:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Speech
{
	/// <summary>
	/// Static method designed for use with the SpeakingProjectMonitor
	/// </summary>
	public class SpeechUtil
	{

		public static String makeProjectNameMoreSpeechFriendly(String projectName) {
			String fixedUpName = addSpacesToCamelCase(projectName);
			fixedUpName = fixedUpName.Replace("_"," ");
			fixedUpName = fixedUpName.Replace("-"," ");
			fixedUpName = fixedUpName.Replace("  "," ");
			return fixedUpName;
		}
		
		private static String addSpacesToCamelCase(String text)
		{
	        if (String.IsNullOrEmpty(text))
	           return string.Empty;
	        StringBuilder newText = new StringBuilder(text.Length * 2);
	        newText.Append(text[0]);
	        for (int i = 1; i < text.Length; i++)
	        {
	            if (char.IsUpper(text[i]))
	                newText.Append(' ');
	            newText.Append(text[i]);
	        }
	        return newText.ToString();
		}
		
		public static bool shouldSpeak (BuildTransition t, bool successSetting, bool failureSetting) {
			bool shouldSpeakSuccess = successSetting && buildIsGood(t);
			bool shouldSpeakFailure = failureSetting && buildIsBad(t);
			return shouldSpeakSuccess || shouldSpeakFailure;
		}
		
		private static bool buildIsGood (BuildTransition t){
			return t.Equals(BuildTransition.Fixed) || t.Equals(BuildTransition.StillSuccessful);
		}
		
		private static bool buildIsBad (BuildTransition t){
			return t.Equals(BuildTransition.Broken) || t.Equals(BuildTransition.StillFailing);
		}
		
	}
}
