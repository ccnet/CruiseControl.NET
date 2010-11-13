namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Interface definition for username-to-emailaddress converters in the Email publisher
    /// (<see cref="ThoughtWorks.CruiseControl.Core.Publishers.EmailPublisher"/>).
    /// </summary>
    /// <title>Email Converter</title>
    public interface IEmailConverter
    {
        /// <summary>
        /// Apply the conversion from userName to email address.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <returns>The email address.</returns>
        string Convert(string userName);
    }
}