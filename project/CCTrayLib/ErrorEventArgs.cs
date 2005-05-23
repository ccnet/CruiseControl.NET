using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class ErrorEventArgs : EventArgs
	{
		public Exception Exception;

		public ErrorEventArgs (Exception exception)
		{
			Exception = exception;
		}

	}

	public delegate void ErrorEventHandler (object sauce, ErrorEventArgs e);

}