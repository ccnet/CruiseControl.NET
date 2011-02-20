namespace CruiseControl.Core.Channels
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// An endpoint using .NET TCP.
    /// </summary>
    public class NetTcp
        : WcfEndpoint
    {
        #region Public properties
        #region Binding
        /// <summary>
        /// Gets the binding.
        /// </summary>
        public override Binding Binding
        {
            get { return new NetTcpBinding(); }
        }
        #endregion
        #endregion
    }
}
