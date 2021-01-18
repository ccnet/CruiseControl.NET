using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
    public class X10SettingsControlTest
    {
        [Test]
        [Platform(Exclude = "Mono", Reason = "No X display available")]
        public void CanBindToDefaultConfiguration()
        {
            X10SettingsControl control = new X10SettingsControl();

            X10Configuration configuration = new X10Configuration();
            control.BindX10Controls(configuration);
        }


    }
}
