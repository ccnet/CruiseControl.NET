namespace CruiseControl.Core.Triggers
{
    using System;
    using System.ComponentModel;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    /// <summary>
    /// Prevents the inner trigger from firing within the time period.
    /// </summary>
    [ContentProperty("InnerTrigger")]
    public class RollUp
        : Trigger
    {
        #region Private fields
        private DateTime? nextTime;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RollUp"/> class.
        /// </summary>
        public RollUp()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollUp"/> class.
        /// </summary>
        /// <param name="period">The period.</param>
        public RollUp(TimeSpan period)
        {
            this.Period = period;
        }
        #endregion

        #region Public properties
        #region Period
        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        /// <value>
        /// The period.
        /// </value>
        public TimeSpan? Period { get; set; }
        #endregion

        #region InnerTrigger
        /// <summary>
        /// Gets or sets the inner trigger.
        /// </summary>
        /// <value>
        /// The inner trigger.
        /// </value>
        public Trigger InnerTrigger { get; set; }
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
            if (!this.Period.HasValue)
            {
                validationLog.AddError("No period set - trigger will not fire");
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
            return this.nextTime.Value < this.Clock.Now ? 
                this.InnerTrigger.Check() :
                null;
        }
        #endregion

        #region OnReset()
        /// <summary>
        /// Called when a reset needs to be performed.
        /// </summary>
        protected override void OnReset()
        {
            base.OnReset();
            this.nextTime = this.Clock.Now.Add(this.Period.Value);
        }
        #endregion
        #endregion
    }
}
