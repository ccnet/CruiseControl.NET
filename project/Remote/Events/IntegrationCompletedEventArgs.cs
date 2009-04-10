using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// The event arguments for when an integration is attempting to start.
    /// </summary>
    public class IntegrationCompletedEventArgs
        : ProjectEventArgs
    {
        #region Private fields
        private readonly IntegrationRequest request;
        private readonly IntegrationStatus status;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="IntegrationCompletedEventArgs"/>.
        /// </summary>
        /// <param name="request">The request to process.</param>
        public IntegrationCompletedEventArgs(IntegrationRequest request, string projectName, IntegrationStatus status)
            : base(projectName)
        {
            this.request = request;
            this.status = status;
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

        #region Status
        /// <summary>
        /// The status of the request.
        /// </summary>
        public IntegrationStatus Status
        {
            get { return this.status; }
        }
        #endregion
        #endregion
    }
}
