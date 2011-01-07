namespace CruiseControl.Core.Tasks.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// Checks that at least one child condition is true.
    /// </summary>
    [ContentProperty("Children")]
    public class Or
        : TaskCondition
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Or"/> class.
        /// </summary>
        public Or()
        {
            this.Children = new List<TaskCondition>();
        }
        #endregion

        #region Public properties
        #region Children
        /// <summary>
        /// Gets the children.
        /// </summary>
        public IList<TaskCondition> Children { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this condition.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            foreach (var child in this.Children)
            {
                child.Validate(validationLog);
            }
        }
        #endregion

        #region Evaluate()
        /// <summary>
        /// Evaluates whether this condition is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance is valid; <c>false</c> otherwise.
        /// </returns>
        public override bool Evaluate(TaskExecutionContext context)
        {
            var passed = this.Children.Any(c => c.Evaluate(context));
            return passed;
        }
        #endregion
        #endregion
    }
}
