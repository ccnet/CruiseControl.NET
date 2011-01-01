namespace CruiseControl.Core.Triggers
{
    using System;
    using System.ComponentModel;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    /// <summary>
    /// Trips after a set interval.
    /// </summary>
    public class Interval
        : Trigger
    {
        #region Private fields
        private DateTime? nextTime;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        public Interval()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        /// <param name="period">The period.</param>
        public Interval(TimeSpan period)
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
        public TimeSpan Period { get; set; }
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
        #region Initialise()
        /// <summary>
        /// Initialises this when the project starts.
        /// </summary>
        public override void Initialise()
        {
            base.Initialise();
            this.Reset();
        }
        #endregion

        #region Check()
        /// <summary>
        /// Checks if this trigger has been tripped.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if tripped; <c>null</c> otherwise.
        /// </returns>
        public override IntegrationRequest Check()
        {
            var hasPassed = this.Clock.Now >= this.nextTime;
            return hasPassed ?
                new IntegrationRequest(this.NameOrType) :
                null;
        }
        #endregion

        #region Reset()
        /// <summary>
        /// Resets this trigger after an integration.
        /// </summary>
        public override void Reset()
        {
            this.nextTime = this.Clock.Now.Add(this.Period);
        }
        #endregion
        #endregion
    }
}
