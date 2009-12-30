#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// The event arguments for when an integration is attempting to start.
    /// </summary>
    public class IntegrationStartedEventArgs
        : ProjectEventArgs
    {
        #region Private fields
        private EventResult result = EventResult.Continue;
        private readonly IntegrationRequest request;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="IntegrationStartedEventArgs"/>.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="request">The request to process.</param>
        public IntegrationStartedEventArgs(IntegrationRequest request, string projectName)
            : base(projectName)
        {
            this.request = request;
        }
        #endregion

        #region Public properties
        #region Request
        /// <summary>
        /// The integration request.
        /// </summary>
        public IntegrationRequest Request
        {
            get { return this.request; }
        }
        #endregion

        #region Result
        /// <summary>
        /// The result of the event.
        /// </summary>
        public EventResult Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        #endregion
        #endregion

        #region Public enums
        #region Result
        /// <summary>
        /// The result of the event.
        /// </summary>
        public enum EventResult
        {
            /// <summary>
            /// Cancel the integration request completely.
            /// </summary>
            Cancel,
            /// <summary>
            /// Delay the integration request until cleared.
            /// </summary>
            Delay,
            /// <summary>
            /// Continue with the integration request.
            /// </summary>
            Continue,
        }
        #endregion
        #endregion
    }
}
