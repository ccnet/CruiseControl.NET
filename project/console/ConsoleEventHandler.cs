using System;
using System.Runtime.InteropServices;

namespace ThoughtWorks.CruiseControl.Console
{
	/// <summary>
	/// Intercepts events raised by the command console.
	/// </summary>
	public class ConsoleEventHandler : IDisposable
	{
		private ControlEventHandler handler;

		public delegate void ControlEventHandler(ConsoleEvent consoleEvent);
		public EventHandler OnConsoleEvent;

		public enum ConsoleEvent
		{
			CTRL_C = 0,		// From wincom.h
			CTRL_BREAK = 1,
			CTRL_CLOSE = 2,
			CTRL_LOGOFF = 5,
			CTRL_SHUTDOWN = 6
		}

		public ConsoleEventHandler()
		{
			handler = new ControlEventHandler(HandleControlEvent);
			SetConsoleCtrlHandler(handler, true);
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

		[DllImport("kernel32.dll")]
		static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);
	}
}