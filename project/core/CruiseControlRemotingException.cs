
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	[Serializable]
	public class CruiseControlRemotingException : CruiseControlException
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlRemotingException" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public CruiseControlRemotingException() {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlRemotingException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public CruiseControlRemotingException(string message) : base(message) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlRemotingException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="url">The URL.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
		public CruiseControlRemotingException(string message, string url, Exception e) : base(CreateMessage(message, url), e) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlRemotingException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		protected CruiseControlRemotingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		private static string CreateMessage(string message, string url)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Cannot connect to CruiseControl server {0}.  {1}", url, message);
		}
	}
}
