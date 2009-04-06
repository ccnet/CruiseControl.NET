using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    public class Cm17LowLevelDriver : IX10LowLevelDriver
    {
        private String comPort;
        private String houseCode;
        private Label statusLabel = null;

        public Cm17LowLevelDriver(String houseCode, String comPort)
        {
            this.comPort = comPort;
            this.houseCode = houseCode;
        }

        public void ResetStatus(Label labelSimulationStatus)
        {
            this.statusLabel = labelSimulationStatus;
            this.statusLabel.Text = "";
        }

        public void ControlDevice(int deviceCode, Function deviceCommand, int lightLevel)
        {
            int portNum = getPortNum();
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "cm17a.exe";
            proc.StartInfo.Arguments = portNum + " " + houseCode + deviceCode + deviceCommand;
            String statusLine = "running " + proc.StartInfo.FileName + " " + proc.StartInfo.Arguments;
            Trace.WriteLine(statusLine);
            if (statusLabel != null) { statusLabel.Text = statusLine; statusLabel.Visible = true; }
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            String output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            Trace.WriteLine("finished " + statusLine);
            if (statusLabel != null) { statusLabel.Text = "finished" + statusLine + "\n" + output; }
        }

        private int getPortNum()
        {
            int portNum = 0;
            if (comPort.Length > 3)
            {
                try
                {
                    portNum = int.Parse(comPort.Substring(3, 1));
                    return portNum;
                }
                catch (FormatException formatException)
                {
                    throw new ArgumentException("COM Port should have a digit in the 4th position. (i.e. 'COM1')", formatException);
                }

            }
            else
            {
                throw new ArgumentException("COM Port should be a string 4 characters long with a digit in the 4th position. (i.e. 'COM1')");
            }
        }

    }
}
