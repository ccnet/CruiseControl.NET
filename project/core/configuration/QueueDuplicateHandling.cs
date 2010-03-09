namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Defines the different ways duplicates can be handled in a queue.
    /// </summary>
    public enum QueueDuplicateHandlingMode
    {
        /// <summary>
        /// If a duplicate is found, then it should be ignored.
        /// </summary>
        UseFirst,

        /// <summary>
        /// If a duplicate is found and it is not a force build, then the initial item should be removed and the 
        /// new item added to the end of the queue (position of the item may change.)
        /// </summary>
        ApplyForceBuildsReAdd,

        /// <summary>
        /// If a duplicate is found and it is not a force build, then the initial item should be removed and the 
        /// new item added to the beginning of the queue (position of the item may change.)
        /// </summary>
        ApplyForceBuildsReAddTop,

        /// <summary>
        /// If a duplicate is found and it is not a force build, then the initial item should be replaced with
        /// the new item (position of the item won't change).
        /// </summary>
        ApplyForceBuildsReplace,
    }
}
