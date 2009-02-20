using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleMain
	{
		[STAThread]
		internal static int Main(string[] args)
		{
            ArgumentParser parsedArgs = new ArgumentParser(args);
			try
			{
				new ConsoleRunner(parsedArgs, new CruiseServerFactory()).Run();
			    return 0;
			}
			catch (Exception ex)
			{
                Log.Error(ex);
                if (!parsedArgs.NoPauseOnError)
                {
                    System.Console.WriteLine("An unexpected error has caused the console to crash, please press any key to continue...");
                    System.Console.ReadKey();
                }
			    return 1;
			}
		}
	}
}
