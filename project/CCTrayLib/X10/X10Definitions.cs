using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public enum ControllerType { CM11, CM17A }

    public enum HouseCode { A=1, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P }

    [Flags]
    public enum Function
    {
        AllUnitsOff = 0x00,
        AllLightsOn = 0x01,
        On = 0x02,
        Off = 0x03,
        Dim = 0x04,
        Bright = 0x05,
        AllLightsOff = 0x06,
        ExtendedCode = 0x07,
        HailRequest = 0x08,
        HailAcknowledge = 0x09,
        PresetDim1 = 0x0A,
        PresetDim2 = 0x0B,
        ExtededDataTransfer = 0x0C,
        StatusOn = 0X0D,
        StatusOff = 0x0E,
        StatusRequest = 0x0F
    }

    public class X10Definitions
    {

    }
}
