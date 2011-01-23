using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;


namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public class LowLevelDriverFactory
    {
        private X10Configuration configuration;
        private static IX10LowLevelDriver driver = null; // Can only have one active driver at the time
        private static Object padLock = new object();

        public LowLevelDriverFactory(X10Configuration configuration)
        {
            this.configuration = configuration;
        }

        public IX10LowLevelDriver getDriver()
        {
            // Make sure we shut down the old driver before creating a new
            // or, they will compete for resources.
            lock (padLock)
            {
                if (driver != null)
                {
                    driver.CloseDriver();
                }
                
                ControllerType type = (ControllerType)Enum.Parse(typeof(ControllerType), configuration.DeviceType);
                switch (type)
                {
                    case ControllerType.CM11:
                        driver = new Cm11LowLevelDriver(configuration.HouseCode, configuration.ComPort);
                        break;

                    case ControllerType.CM17A:
                        driver = new Cm17LowLevelDriver(configuration.HouseCode, configuration.ComPort);
                        break;
                }
               return driver;
            }
        }
    }
}
