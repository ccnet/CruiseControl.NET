using System;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    class KillUtil
    {
        public static void KillPid(int pid)
        {
            Process process = new Process();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    process.StartInfo.FileName = string.Format("{0}\\taskkill.exe",
                            Environment.GetFolderPath(Environment.SpecialFolder.System));
                    process.StartInfo.Arguments = string.Format("/pid {0} /t /f", pid);
                    break;

                case PlatformID.Unix:
                    process.StartInfo.FileName = "/usr/bin/pkill";
                    process.StartInfo.Arguments = string.Format("-9 -g {0}", pid);
                    break;

                default:
                    throw new Exception("Unknown Operating System.");
            }

            if (!File.Exists(process.StartInfo.FileName))
            {
                throw new Exception(string.Format("Kill command {0} not found.",
                                                  process.StartInfo.FileName));
            }

            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
