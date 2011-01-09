namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Ninject;
    using NLog;

    /// <summary>
    /// The root configuration node.
    /// </summary>
    [ContentProperty("Children")]
    public class Server
        : AttachablePropertyStore, IServerItemContainer
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IActionInvoker actionInvoker;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <remarks>
        /// This is required for Ninject.
        /// </remarks>
        [Inject]
        public Server()
        {
            this.InitialiseChildCollection(new ServerItem[0]);
            this.InitialiseChannelCollection(new ClientChannel[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="children">The children.</param>
        public Server(params ServerItem[] children)
        {
            this.InitialiseChildCollection(children);
            this.InitialiseChannelCollection(new ClientChannel[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public Server(string name, params ServerItem[] children)
        {
            this.Name = name;
            this.InitialiseChildCollection(children);
            this.InitialiseChannelCollection(new ClientChannel[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="channels">The channels.</param>
        public Server(string name, IEnumerable<ClientChannel> channels)
        {
            this.Name = name;
            this.InitialiseChildCollection(new ServerItem[0]);
            this.InitialiseChannelCollection(channels);
        }
        #endregion

        #region Public properties
        #region Version
        /// <summary>
        /// Gets or sets the configuration version.
        /// </summary>
        /// <value>The version.</value>
        [TypeConverter(typeof(VersionTypeConverter))]
        public Version Version { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        [DefaultValue(null)]
        public string Name { get; set; }
        #endregion

        #region UniversalName
        /// <summary>
        /// Gets the universal name of this server.
        /// </summary>
        /// <value>
        /// The universal name.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UniversalName
        {
            get { return "urn:ccnet:" + (this.Name ?? string.Empty); }
        }
        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public IList<ServerItem> Children { get; private set; }
        #endregion

        #region ClientChannels
        /// <summary>
        /// Gets the client channels.
        /// </summary>
        public IList<ClientChannel> ClientChannels { get; private set; }
        #endregion

        #region ActionInvoker
        /// <summary>
        /// Gets or sets the action invoker.
        /// </summary>
        /// <value>
        /// The action invoker.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IActionInvoker ActionInvoker
        {
            get { return this.actionInvoker; }
            set
            {
                this.actionInvoker = value;
                this.actionInvoker.Server = this;
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region OpenCommunications()
        /// <summary>
        /// Opens the communications channels.
        /// </summary>
        public void OpenCommunications()
        {
            foreach (var channel in this.ClientChannels)
            {
                channel.Initialise(this.actionInvoker);
            }
        }
        #endregion

        #region CloseCommunications()
        /// <summary>
        /// Closes the communications channels.
        /// </summary>
        public void CloseCommunications()
        {
            foreach (var channel in this.ClientChannels)
            {
                channel.CleanUp();
            }
        }
        #endregion

        #region Locate()
        /// <summary>
        /// Locates an item by its universal name.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <returns>
        /// The item if found; <c>null</c> otherwise.
        /// </returns>
        public virtual object Locate(string name)
        {
            // Get the name for this server
            var thisName = this.UniversalName;

            // If the server part does not match then this is for a different server
            if (!name.StartsWith(thisName, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            // If the lengths are the same then matching the server itself
            if (name.Length == thisName.Length)
            {
                return this;
            }

            // Otherwise check all the projects and children
            var item = LocateInChildren(name, this.Children.SelectMany(c => c.ListProjects())) ??
                       LocateInChildren(name, this.Children);
            return item;
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this server after it has been loaded.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public virtual void Validate(IValidationLog validationLog)
        {
            logger.Debug("Validating server '{0}'", this.Name ?? string.Empty);

            // Everything must have a name
            if (string.IsNullOrEmpty(this.Name))
            {
                validationLog.AddError("The Server has no name specified.");
            }

            // Validate the children
            foreach (var child in this.Children)
            {
                child.Validate(validationLog);
            }

            // Check if there are any duplicated children
            ValidationHelpers.CheckForDuplicateItems(this.Children, validationLog, "child");
            var projects = this.Children
                .SelectMany(c => c.ListProjects());
            ValidationHelpers.CheckForDuplicateItems(projects, validationLog, "project");
        }
        #endregion
        #endregion

        #region Private methods
        #region LocateInChildren()
        /// <summary>
        /// Attempts to locate an item with a list of children.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        /// <returns>
        /// The item if found; <c>null</c> otherwise.
        /// </returns>
        private static object LocateInChildren(string name, IEnumerable<ServerItem> children)
        {
            object item = null;
            foreach (var child in children)
            {
                item = child.Locate(name);
                if (item != null)
                {
                    break;
                }
            }

            return item;
        }
        #endregion

        #region UpdateChildren()
        /// <summary>
        /// Updates the children.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateChildren(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ServerItem child in e.OldItems ?? new ServerItem[0])
            {
                child.Server = null;
            }

            foreach (ServerItem child in e.NewItems ?? new ServerItem[0])
            {
                child.Server = this;
            }
        }
        #endregion

        #region InitialiseChildCollection()
        /// <summary>
        /// Initialises the collection.
        /// </summary>
        /// <param name="children">The children.</param>
        private void InitialiseChildCollection(IEnumerable<ServerItem> children)
        {
            var collection = new ObservableCollection<ServerItem>(children);
            foreach (var child in collection)
            {
                child.Server = this;
            }

            collection.CollectionChanged += UpdateChildren;
            this.Children = collection;
        }
        #endregion

        #region InitialiseChannelCollection()
        /// <summary>
        /// Initialises the channel collection.
        /// </summary>
        /// <param name="channels">The channels.</param>
        private void InitialiseChannelCollection(IEnumerable<ClientChannel> channels)
        {
            this.ClientChannels = new List<ClientChannel>(channels);
        }
        #endregion
        #endregion
    }
}
