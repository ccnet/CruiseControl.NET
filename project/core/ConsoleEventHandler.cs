
using System;
using System.Runtime.InteropServices;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Intercepts events raised by the command console.
	/// </summary>
	public class ConsoleEventHandler : IDisposable
	{
		private ControlEventHandler handler;
		private ExecutionEnvironment environment = new ExecutionEnvironment();
		public event EventHandler OnConsoleEvent;

		public ConsoleEventHandler()
		{
			if (environment.IsRunningOnWindows)
			{
				handler = new ControlEventHandler(HandleControlEvent);
				SetConsoleCtrlHandler(handler, true);
			}
		}


		private void HandleControlEvent(ConsoleEvent consoleEvent)
		{
			lock (this)
			{
				if (OnConsoleEvent != null)
				{
					OnConsoleEvent(this, EventArgs.Empty);
				}
			}
		}

		void IDisposable.Dispose()
		{
			lock (this)
			{
				UnregisterHandler();
			}
		}

		private void UnregisterHandler()
		{
			if (handler != null)
			{
				SetConsoleCtrlHandler(handler, false);
				handler = null;
			}
		}

		private delegate void ControlEventHandler(ConsoleEvent consoleEvent);

		// From wincom.h
		private enum ConsoleEvent
		{
			CTRL_C = 0,
			CTRL_BREAK = 1,
			CTRL_CLOSE = 2,
			CTRL_LOGOFF = 5,
			CTRL_SHUTDOWN = 6
		}

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);
	}
}