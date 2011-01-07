namespace CruiseControl.Core
{
    /// <summary>
    /// The status of an integration.
    /// </summary>
    public enum IntegrationStatus
    {
        /// <summary>
        /// The status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The integration is successful.
        /// </summary>
        Success,

        /// <summary>
        /// The integration has failed.
        /// </summary>
        Failure
    }
}
