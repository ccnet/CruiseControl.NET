namespace CruiseControl.Web
{
    using System;

    /// <summary>
    /// The target levels for an action handler.
    /// </summary>
    [Flags]
    public enum ActionHandlerTargets
    {
        /// <summary>
        /// The action handler targets the root level of the console.
        /// </summary>
        Root,

        /// <summary>
        /// The action handler targets the server level.
        /// </summary>
        Server,

        /// <summary>
        /// The action handler targets the project level.
        /// </summary>
        Project,

        /// <summary>
        /// The action handler targets the build level.
        /// </summary>
        Build,
    }
}