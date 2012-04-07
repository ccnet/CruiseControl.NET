using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.X10
{
    [TestFixture]
    public class LowLevelDriverFactoryTest
    {

        [Test]
        public void ShouldCreateTheCm11DriverBasedOnType()
        {
            X10Configuration configuration = new X10Configuration();
            configuration.DeviceType = ControllerType.CM11.ToString();
            configuration.ComPort = "COM1";

            LowLevelDriverFactory factory = new LowLevelDriverFactory(configuration);
            try
            {
                IX10LowLevelDriver driver = factory.getDriver();
                // factory will return null driver if it can't create one - caller needs to check!
                if (driver != null)
                {
                    Assert.That(driver, Is.InstanceOf<Cm11LowLevelDriver>(), "driver should be correct type");
                }
            }
            catch (ApplicationException appEx)
            {
                // this test only works if COM1 is available...fail if the message is anything other than
                // something about the com port not being there. 
                Assert.IsTrue(appEx.InnerException.Message.Contains("The port 'COM1' does not exist."),"threw an exception, but the message was wrong");
            }

        }

        [Test]
        public void ShouldCreateTheCm17aDriverBasedOnType()
        {
            X10Configuration configuration = new X10Configuration();
            configuration.DeviceType = ControllerType.CM17A.ToString();
            configuration.ComPort = "COM1";

            LowLevelDriverFactory factory = new LowLevelDriverFactory(configuration);
            IX10LowLevelDriver driver = factory.getDriver();

            // factory will return null driver if it can't create one - caller needs to check!
            if (driver != null)
            {
                Assert.That(driver, Is.InstanceOf<Cm17LowLevelDriver>());
            }
        }
    }
}
