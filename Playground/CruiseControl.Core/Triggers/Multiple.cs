namespace CruiseControl.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A trigger that contains multiple triggers.
    /// </summary>
    [ContentProperty("Triggers")]
    public class Multiple
        : Trigger
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Multiple"/> class.
        /// </summary>
        public Multiple()
        {
            this.Condition = CombinationOperator.Or;
            this.Triggers = new List<Trigger>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Multiple"/> class.
        /// </summary>
        /// <param name="triggers">The triggers.</param>
        public Multiple(params Trigger[] triggers)
        {
            this.Condition = CombinationOperator.Or;
            this.Triggers = new List<Trigger>(triggers);
        }
        #endregion

        #region Public properties
        #region NextTime
        /// <summary>
        /// Gets the time the trigger will next be tripped or checked to see if tripped.
        /// </summary>
        public override DateTime? NextTime
        {
            get
            {
                var dates = this.Triggers
                    .Where(t => t.NextTime != null)
                    .Select(t => t.NextTime);
                return dates.Count() > 0 ? dates.Min() : null;
            }
        }
        #endregion

        #region Triggers
        /// <summary>
        /// Gets the triggers.
        /// </summary>
        public IList<Trigger> Triggers { get; private set; }
        #endregion

        #region Condition
        /// <summary>
        /// Gets or sets the condition for combining triggers.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public CombinationOperator Condition { get; set; }
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
            foreach (var trigger in this.Triggers)
            {
                trigger.Validate(validationLog);
            }
        }
        #endregion

        #region Initialise()
        /// <summary>
        /// Initialises this when the project starts.
        /// </summary>
        public override void Initialise()
        {
            base.Initialise();
            foreach (var trigger in this.Triggers)
            {
                trigger.Initialise();
            }
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up when the project stops.
        /// </summary>
        public override void CleanUp()
        {
            base.CleanUp();
            foreach (var trigger in this.Triggers)
            {
                trigger.CleanUp();
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
            var isTripped = false;
            switch (this.Condition)
            {
                case CombinationOperator.Or:
                    isTripped = this.Triggers.Any(t => t.Check() != null);
                    break;

                case CombinationOperator.And:
                    isTripped = this.Triggers.All(t => t.Check() != null);
                    break;
            }

            return isTripped ?
                new IntegrationRequest(this.NameOrType) :
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
            foreach (var trigger in this.Triggers)
            {
                trigger.Reset();
            }
        }
        #endregion

        #region OnProjectChanged()
        /// <summary>
        /// Called when the project has been changed.
        /// </summary>
        protected override void OnProjectChanged()
        {
            base.OnProjectChanged();
            foreach (var trigger in this.Triggers)
            {
                trigger.Project = this.Project;
            }
        }
        #endregion
        #endregion

        #region Public enums
        #region Condition
        #endregion
        #endregion
    }
}
