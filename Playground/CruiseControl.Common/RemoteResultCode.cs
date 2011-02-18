namespace CruiseControl.Common
{
    /// <summary>
    /// The result of an action or query.
    /// </summary>
    public enum RemoteResultCode
    {
        /// <summary>
        /// The action or query was successful.
        /// </summary>
        Success,

        /// <summary>
        /// An unknown fatal error occurred in the action or query.
        /// </summary>
        FatalError,

        /// <summary>
        /// Unable to find an item matching the URN.
        /// </summary>
        UnknownUrn,

        /// <summary>
        /// Unable to find an action matching the action name.
        /// </summary>
        UnknownAction,

        /// <summary>
        /// The input was invalid for the action.
        /// </summary>
        InvalidInput,

        /// <summary>
        /// The arguments are missing.
        /// </summary>
        MissingArguments,
    }
}