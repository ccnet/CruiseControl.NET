namespace CruiseControl.Core.Triggers
{
    using System;
    using System.ComponentModel;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    /// <summary>
    /// Triggers a build at a set time.
    /// </summary>
    public class Schedule
        : Trigger
    {
        #region Private fields
        private DateTime? nextTime;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        public Schedule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        public Schedule(TimeSpan time)
        {
            this.Time = time;
        }
        #endregion

        #region Public properties
        #region Time
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public TimeSpan? Time { get; set; }
        #endregion

        #region NextTime
        /// <summary>
        /// Gets the time the trigger will next be tripped or checked to see if tripped.
        /// </summary>
        public override DateTime? NextTime
        {
            get { return this.nextTime; }
        }
        #endregion

        #region Clock
        /// <summary>
        /// Gets or sets the clock.
        /// </summary>
        /// <value>
        /// The clock.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IClock Clock { get; set; }
        #endregion
        #endregion
        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this trigger.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            if (!this.Time.HasValue)
            {
                validationLog.AddError("No time set - trigger will not fire");
            }
        }
        #endregion
        #endregion


        #region Protected methods
        #region OnCheck()
        /// <summary>
        /// Called when the trigger needs to be checked.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if tripped; <c>null</c> otherwise.
        /// </returns>
        protected override IntegrationRequest OnCheck()
        {
            if (this.Clock.Now < this.nextTime.Value)
            {
                return null;
            }

            return new IntegrationRequest(this.NameOrType);
        }
        #endregion

        #region OnReset()
        /// <summary>
        /// Called when a reset needs to be performed.
        /// </summary>
        protected override void OnReset()
        {
            base.OnReset();
            var timeToCheck = this.Clock.Today.Add(this.Time.Value);
            if (timeToCheck <= this.Clock.Now)
            {
                timeToCheck = timeToCheck.AddDays(1);
            }

            this.nextTime = timeToCheck;
        }
        #endregion
        #endregion
    }
}
