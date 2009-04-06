using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;


namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public class LowLevelDriverFactory
    {
        private X10Configuration configuration;
        private IX10LowLevelDriver driver = null;

        public LowLevelDriverFactory(X10Configuration configuration)
        {
            this.configuration = configuration;
        }

        public IX10LowLevelDriver getDriver()
        {
            if (driver == null)
            {
                ControllerType type = (ControllerType)Enum.Parse(typeof(ControllerType), configuration.DeviceType);
                switch (type){
                    case ControllerType.CM11:
                        driver = new Cm11LowLevelDriver(configuration.HouseCode, configuration.ComPort);
                        break;

                    case ControllerType.CM17A:
                        driver = new Cm17LowLevelDriver(configuration.HouseCode,configuration.ComPort);
                        break;
                }
            }
            return driver;
        }
    }
}
