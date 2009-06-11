/*
 * Created by SharpDevelop.
 * User: sdonie
 * Date: 2/27/2009
 * Time: 1:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Speech;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Speech
{
	[TestFixture]
	public class SpeechUtilTest
	{
		[Test]
		public void TestMakeProjectNameSpeechFriendly()
		{
			Assert.AreEqual("expected",SpeechUtil.makeProjectNameMoreSpeechFriendly("expected"));
			Assert.AreEqual("the One True Project",SpeechUtil.makeProjectNameMoreSpeechFriendly("theOneTrueProject"));
			Assert.AreEqual("Project 1",SpeechUtil.makeProjectNameMoreSpeechFriendly("Project_1"));
			Assert.AreEqual("A Project With First Word A Single Letter",SpeechUtil.makeProjectNameMoreSpeechFriendly("AProjectWithFirstWordASingleLetter"));
			Assert.AreEqual("a Project With First Word A Single Letter",SpeechUtil.makeProjectNameMoreSpeechFriendly("aProjectWithFirstWordASingleLetter"));
			Assert.AreEqual("A Project With Some Underscores",SpeechUtil.makeProjectNameMoreSpeechFriendly("AProjectWith_Some_Underscores"));
			Assert.AreEqual("A Project With some dashes",SpeechUtil.makeProjectNameMoreSpeechFriendly("AProjectWith-some-dashes"));
			Assert.AreEqual("",SpeechUtil.makeProjectNameMoreSpeechFriendly(""));
		}
		
		[Test]
		public void TestWhetherWeShouldSpeak(){
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.StillSuccessful,true,false));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.Fixed,true,false));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.StillSuccessful,true,true));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.Fixed,true,true));

			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.Broken,false,true));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.StillFailing,false,true));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.Broken,true,true));
			Assert.IsTrue(SpeechUtil.shouldSpeak(BuildTransition.StillFailing,true,true));

			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.Broken,true,false));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.StillFailing,true,false));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.Broken,false,false));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.StillFailing,false,false));
			
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.StillSuccessful,false,true));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.Fixed,false,true));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.StillSuccessful,false,false));
			Assert.IsFalse(SpeechUtil.shouldSpeak(BuildTransition.Fixed,false,false));
		}
	}
}
