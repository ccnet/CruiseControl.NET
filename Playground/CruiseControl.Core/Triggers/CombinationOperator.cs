namespace CruiseControl.Core.Triggers
{
    /// <summary>
    /// The condition to use for combining the triggers.
    /// </summary>
    public enum CombinationOperator
    {
        /// <summary>
        /// Fire the trigger only if all triggers are tripped.
        /// </summary>
        And,

        /// <summary>
        /// Fire the trigger if at least one child trigger is tripped.
        /// </summary>
        Or,
    }
}