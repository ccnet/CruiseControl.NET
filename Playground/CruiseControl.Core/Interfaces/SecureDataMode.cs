namespace CruiseControl.Core.Interfaces
{
    /// <summary>
    /// Defines the mode for accessing secure data
    /// </summary>
    public enum SecureDataMode
    {
        /// <summary>
        /// The data is being accessed in private mode.
        /// </summary>
        Private,

        /// <summary>
        /// The data is being accessed in public mode.
        /// </summary>
        Public,
    }
}
