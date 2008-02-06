using System;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    class KillUtil
    {
        /// <summary>
        /// Default installation directory for the "Windows 2000 Service Pack 4 Support Tools" package.
        /// </summary>
        public const string WIN2K_SUPPORT_TOOLS_DIR = @"C:\\Program Files\\Support Tools";

        public static void KillPid(int pid)
        {
            Process process = new Process();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    if (Environment.OSVersion.Version.Minor == 0)
                    {
                        // Windows 2000 doesn't have taskkill.exe, so use kill.exe from the 
                        // "Windows 2000 Service Pack 4 Support Tools" package from Microsoft's download center
                        // (http://www.microsoft.com/Downloads/details.aspx?FamilyID=f08d28f3-b835-4847-b810-bb6539362473&displaylang=en)
                        // instead.  It may not exist, but if it doesn't, at least if can be obtained.
                        process.StartInfo.FileName = string.Format("{0}\\kill.exe", WIN2K_SUPPORT_TOOLS_DIR);
                        process.StartInfo.Arguments = string.Format("-f {0}", pid);
                        break;
                    }
                    else
                    {
                        process.StartInfo.FileName = string.Format("{0}\\taskkill.exe",
                                Environment.GetFolderPath(Environment.SpecialFolder.System));
                        process.StartInfo.Arguments = string.Format("/pid {0} /t /f", pid);
                        break;
                    }

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
