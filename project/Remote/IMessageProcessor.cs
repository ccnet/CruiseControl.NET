
using ThoughtWorks.CruiseControl.Remote.Messages;
namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Processes messages from a client.
    /// </summary>
    public interface IMessageProcessor
    {
        #region ProcessMessage()
        /// <summary>
        /// Processes a message in an XML format.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message in an XML format.</param>
        /// <returns>The response message in an XML format.</returns>
        string ProcessMessage(string action, string message);

        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        Response ProcessMessage(string action, ServerRequest message);
        #endregion
    }
}
