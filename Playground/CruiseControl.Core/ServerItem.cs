namespace CruiseControl.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// An item of server configuration - this is typically a structure item.
    /// </summary>
    public abstract class ServerItem
    {
        #region Private fields
        private Server server;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItem"/> class.
        /// </summary>
        protected ServerItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ServerItem(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion

        #region ItemType
        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string ItemType
        {
            get { return this.GetType().Name; }
        }
        #endregion

        #region Host
        /// <summary>
        /// Gets or sets the host for the project.
        /// </summary>
        /// <value>The host.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ServerItem Host { get; set; }
        #endregion

        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server this item belongs to.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Server Server
        {
            get { return this.server; }
            set
            {
                this.server = value;
                this.OnServerChanged();
            }
        }
        #endregion

        #region UniversalName
        /// <summary>
        /// Gets the universal name of this item.
        /// </summary>
        /// <value>
        /// The universal name.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string UniversalName
        {
            get
            {
                var localName = ":" + this.Name ?? string.Empty;
                if (this.Host == null)
                {
                    var name = this.server == null ? "urn:ccnet:" : this.server.UniversalName;
                    return name + localName;
                }

                return this.Host.UniversalName + localName;
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public abstract void AskToIntegrate(IntegrationContext context);
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this item after it has been loaded.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public virtual void Validate(IValidationLog validationLog)
        {
            // Everything must have a name
            if (string.IsNullOrEmpty(this.Name))
            {
                validationLog.AddError("The {0} has no name specified.", this.ItemType);
            }
        }
        #endregion

        #region ListProjects()
        /// <summary>
        /// Lists the projects within this item.
        /// </summary>
        /// <returns>The projects within this item.</returns>
        public abstract IEnumerable<Project> ListProjects();
        #endregion
        #endregion

        #region Protected methods
        #region OnServerChanged()
        /// <summary>
        /// Called when the server has been changed.
        /// </summary>
        protected virtual void OnServerChanged()
        {
        }
        #endregion
        #endregion
    }
}
