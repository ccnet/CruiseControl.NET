using System;
using System.Windows.Forms;
using ActiveHomeScriptLib;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public class Cm19LowLevelDriver : IX10LowLevelDriver
    {
        ActiveHomeClass ActiveHome = new ActiveHomeClass();
        private string _housecode;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="houseCode">x10 house code</param>
        public Cm19LowLevelDriver(string houseCode)
        {
            _housecode = houseCode;
        }

        /// <summary>
        /// Call into the active home SDK for the USB connectivity. 
        /// </summary>
        public void ControlDevice(int deviceCode, Function deviceCommand, int lightLevel)
        {
            try
            {
                switch (deviceCommand)
                {
                    case Function.AllUnitsOff:
                        break;
                    case Function.AllLightsOn:
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " On", null, null);
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " On", null, null);
                        break;
                    case Function.On:
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " On", null, null);
                        break;
                    case Function.Off:
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " Off", null, null);
                        break;
                    case Function.Dim:
                        break;
                    case Function.Bright:
                        break;
                    case Function.AllLightsOff:
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " Off", null, null);
                        ActiveHome.SendAction("SendPLC", _housecode + deviceCode + " Off", null, null);
                        break;
                    case Function.ExtendedCode:
                        break;
                    case Function.HailRequest:
                        break;
                    case Function.HailAcknowledge:
                        break;
                    case Function.PresetDim1:
                        break;
                    case Function.PresetDim2:
                        break;
                    case Function.ExtededDataTransfer:
                        break;
                    case Function.StatusOn:
                        break;
                    case Function.StatusOff:
                        break;
                    case Function.StatusRequest:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Problem with the CM19 X10 'active home API' ",exception);
            }
        }

        /// <summary>
        /// Not implemented 
        /// </summary>
        public void ResetStatus(Label statusLabel){}

        /// <summary>
        /// Not implemented 
        /// </summary>
        public void CloseDriver(){}
    }
}
