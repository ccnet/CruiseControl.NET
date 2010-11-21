
using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// Formats the Exception into HTML. Need to see if this can be moved to CCException class.
	/// </summary>
	public class HtmlExceptionFormatter : IStringFormatter
	{
		private Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlExceptionFormatter" /> class.	
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
		public HtmlExceptionFormatter(Exception exception)
		{
			this._exception = exception;
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			string message = _exception.Message.Replace(Environment.NewLine, "<br/>");
			return "<br/>ERROR: " + message;
		}
	}
}