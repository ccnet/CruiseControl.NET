using System;
using System.Threading;

namespace sleeper
{
	internal class ConsoleMain
	{
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
			int duration = 60000;
			if (args.Length > 0)
				duration = Int32.Parse(args[0]);

			Console.Out.WriteLine("Sleeping for {0} milliseconds.", duration);
			Console.Out.WriteLine("zzzzz...");
			try
			{
				Thread.Sleep(duration);
			}
			catch (ThreadAbortException e)
			{
				Console.Write("Aborted: I'm outta here");
			}
			Console.Out.WriteLine("Done!");
		}

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			Console.Write("ProcessExit: I'm outta here");
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.Write("Exception: What happened?");
		}

		private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			Console.Write("Bye bye appdomain.");
		}
	}
}