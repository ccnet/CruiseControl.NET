using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// Formats the Exception into HTML. Need to see if this can be moved to CCException class.
	/// </summary>
	public class HtmlExceptionFormatter : IStringFormatter
	{
		private Exception _exception;

		public HtmlExceptionFormatter(Exception e)
		{
			this._exception = e;
		}

		public override string ToString()
		{
			string message = _exception.Message.Replace(Environment.NewLine, "<br/>");
			return "<br/>ERROR: " + message;
		}
	}
}