namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Interface definition for username-to-emailaddress converters in the Email publisher
    /// (<see cref="ThoughtWorks.CruiseControl.Core.Publishers.EmailPublisher"/>).
    /// </summary>
    public interface IEmailConverter
    {
        /// <summary>
        /// Apply the conversion from username to email address.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The email address.</returns>
        string Convert(string username);
    }
}