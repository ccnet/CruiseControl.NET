namespace CruiseControl.Core
{
    /// <summary>
    /// The state of a task.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// The task is pending.
        /// </summary>
        Pending,

        /// <summary>
        /// The task is checking conditions.
        /// </summary>
        CheckingConditions,

        /// <summary>
        /// The task is executing.
        /// </summary>
        Executing,

        /// <summary>
        /// The task has been skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// The task has completed.
        /// </summary>
        Completed,

        /// <summary>
        /// The task was terminated.
        /// </summary>
        Terminated,

        /// <summary>
        /// The task state is unknown.
        /// </summary>
        Unknown,
    }
}
