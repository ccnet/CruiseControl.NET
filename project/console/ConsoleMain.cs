using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Console
{
	public class ConsoleMain
	{
		[STAThread]
		internal static void Main(string[] args)
		{
			try
			{
				new ConsoleRunner(new ArgumentParser(args), new CruiseServerFactory()).Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}
