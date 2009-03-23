using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console
{
    public class AppRunner
        : MarshalByRefObject
    {
        private ConsoleRunner runner;
        private bool isStopping = false;

        public int Run(string[] args)
        {
            try
            {
                runner = new ConsoleRunner(new ArgumentParser(args), new CruiseServerFactory());
                runner.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return 1;
            }
            finally
            {
                runner = null;
            }
        }

        public void Stop(string reason)
        {
            if (!isStopping)
            {
                isStopping = true;
                Log.Info("Stopping console: " + reason);
                runner.Stop();
            }
        }
    }
}
