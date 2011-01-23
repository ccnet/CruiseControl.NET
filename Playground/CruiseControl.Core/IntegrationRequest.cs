namespace CruiseControl.Core
{
    using System;

    public class IntegrationRequest
    {
        #region Private fields
        private readonly DateTime time;
        private readonly string sourceTrigger;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationRequest"/> class.
        /// </summary>
        /// <param name="sourceTrigger">The source trigger.</param>
        public IntegrationRequest(string sourceTrigger)
        {
            this.time = DateTime.Now;
            this.sourceTrigger = sourceTrigger;
        }
        #endregion

        #region Public properties
        #region Time
        /// <summary>
        /// Gets the time of the request.
        /// </summary>
        public DateTime Time
        {
            get { return this.time; }
        }
        #endregion

        #region SourceTrigger
        /// <summary>
        /// Gets the name of the source trigger.
        /// </summary>
        public string SourceTrigger
        {
            get { return this.sourceTrigger; }
        }
        #endregion
        #endregion
    }
}
