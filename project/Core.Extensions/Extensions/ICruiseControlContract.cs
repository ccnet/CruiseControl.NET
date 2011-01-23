using System.ServiceModel;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// The contract of operations that will be provided.
    /// </summary>
    [ServiceContract(Namespace = "http://ccnet.thoughtworks.com/1/5/extensions")]
    public interface ICruiseControlContract
    {
        #region Methods
        #region ProcessMessage()
        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        [OperationContract]
        Response ProcessMessage(string action, ServerRequest message);
        #endregion
        #endregion
    }
}
