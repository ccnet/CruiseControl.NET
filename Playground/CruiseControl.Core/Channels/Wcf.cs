namespace CruiseControl.Core.Channels
{
    using System;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A client channel using WCF.
    /// </summary>
    public class Wcf
        : ClientChannel
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Wcf"/> class.
        /// </summary>
        public Wcf()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wcf"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Wcf(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The channel name.
        /// </value>
        public string Name { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this channel.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            if (string.IsNullOrEmpty(this.Name))
            {
                validationLog.AddWarning("Channel does not have a name");
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnInitialise()
        /// <summary>
        /// Called when this channel is initialised.
        /// </summary>
        protected override void OnInitialise()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OnCleanUp()
        /// <summary>
        /// Called when this channel is cleaned up.
        /// </summary>
        protected override void OnCleanUp()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
