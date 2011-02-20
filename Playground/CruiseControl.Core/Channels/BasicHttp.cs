namespace CruiseControl.Core.Channels
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// An endpoint using basic HTTP.
    /// </summary>
    public class BasicHttp
        : WcfEndpoint
    {
        #region Public properties
        #region Binding
        /// <summary>
        /// Gets the binding.
        /// </summary>
        public override Binding Binding
        {
            get { return new BasicHttpBinding(); }
        }
        #endregion
        #endregion
    }
}
