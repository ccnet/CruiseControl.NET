using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Speech;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class SpeechSettingsControlTest
	{

        [Test]
        public void CanBindToDefaultConfiguration()
        {
        	SpeechSettingsControl control = new SpeechSettingsControl();

        	SpeechConfiguration configuration = new SpeechConfiguration();
            control.BindSpeechControls(configuration);
        }
	}
}
