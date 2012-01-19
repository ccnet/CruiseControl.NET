﻿
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    [Serializable]
    public class SessionInvalidException
        : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInvalidException" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public SessionInvalidException() : base("The session token is either invalid or is for a session that has expired.") {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInvalidException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public SessionInvalidException(string message) : base(message) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInvalidException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
		public SessionInvalidException(string message, Exception e) : base(message, e) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInvalidException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public SessionInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
