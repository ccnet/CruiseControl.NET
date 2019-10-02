using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Console
{
    public class ConsoleMain
    {
        private static object lockObject = new object();

        [STAThread]
        internal static int Main(string[] args)
        {
            MainRunner runner = new MainRunner(args, lockObject);
            return runner.Run();
        }
    }
}
