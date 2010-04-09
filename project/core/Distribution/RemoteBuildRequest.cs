namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Distribution.Messages;

    /// <summary>
    /// A request for a remote build.
    /// </summary>
    public class RemoteBuildRequest
    {
        #region Private fields
        private Timer statusCheck;
        private Func<string, RetrieveBuildStatusResponse> onCheck;
        private Action<RemoteBuildRequest> onCompleted;
        private RetrieveBuildStatusResponse lastResponse;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteBuildRequest"/> class.
        /// </summary>
        /// <param name="machine">The machine.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="onCheck">The check to perform for any status updates.</param>
        public RemoteBuildRequest(
            IBuildMachine machine, 
            string identifier, 
            Func<string, RetrieveBuildStatusResponse> onCheck,
            Action<RemoteBuildRequest> onCompleted)
        {
            this.BuildMachine = machine;
            this.BuildIdentifier = identifier;
            this.onCheck = onCheck;
            this.onCompleted = onCompleted;
            this.statusCheck = new Timer(
                OnStatusCheck, 
                null, 
                5000, 
                Timeout.Infinite);
        }
        #endregion

        #region Public properties
        #region BuildMachine
        /// <summary>
        /// Gets the build machine.
        /// </summary>
        /// <value>The build machine.</value>
        public IBuildMachine BuildMachine { get; private set; }
        #endregion

        #region BuildIdentifier
        /// <summary>
        /// Gets the build identifier.
        /// </summary>
        /// <value>The build identifier.</value>
        public string BuildIdentifier { get; private set; }
        #endregion

        #region Cancelled
        /// <summary>
        /// Gets a value indicating whether this <see cref="RemoteBuildRequest"/> is cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool Cancelled { get; private set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public IntegrationStatus Status
        {
            get { return this.lastResponse.Status; }
        }
        #endregion

        #region Result
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public ITaskResult Result { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Cancel()
        /// <summary>
        /// Cancels a remote build.
        /// </summary>
        public void Cancel()
        {
            if (!this.Cancelled)
            {
                this.Cancelled = true;
                this.BuildMachine.CancelBuild(this.BuildIdentifier);
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region OnStatusCheck()
        /// <summary>
        /// Perform a status check.
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnStatusCheck(object state)
        {
            var response = this.onCheck(this.BuildIdentifier);
            this.lastResponse = response;
            if (response.Status == IntegrationStatus.Unknown)
            {
                this.statusCheck.Change(5000, Timeout.Infinite);
            }
            else
            {
                this.Result = new RemoteBuildTaskResult(response.Status);
                this.onCompleted(this);
            }
        }
        #endregion
        #endregion
    }
}
