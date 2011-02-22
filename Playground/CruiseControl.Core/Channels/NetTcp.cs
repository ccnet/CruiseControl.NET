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
        #region ConfigName
        /// <summary>
        /// Gets or sets the name of the config section for this binding.
        /// </summary>
        /// <value>
        /// The name of the config section.
        /// </value>
        public string ConfigName { get; set; }
        #endregion

        #region Binding
        /// <summary>
        /// Gets the binding.
        /// </summary>
        public override Binding Binding
        {
            get
            {
                if (string.IsNullOrEmpty(this.ConfigName))
                {
                    var binding = new NetTcpBinding();
                    return binding;
                }

                return new NetTcpBinding(this.ConfigName);
            }

        }
        #endregion
        #endregion
    }
}
