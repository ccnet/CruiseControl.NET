namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// Interface for storing sessions.
    /// </summary>
    public interface ISessionStorer
    {
        /// <summary>
        /// The session token to store, null to delete.
        /// </summary>
		void StoreSessionToken(string sessionToken);

        /// <summary>
        /// The display name to store, null to delete.
        /// </summary>
        void StoreDisplayName(string displayName);
    }
}
