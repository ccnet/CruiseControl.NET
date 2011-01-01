namespace CruiseControl.Core.Structure
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;

    /// <summary>
    /// A base implementation of a server item that has children.
    /// </summary>
    [ContentProperty("Children")]
    public abstract class ServerItemContainerBase
        : ServerItem, IServerItemContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemContainerBase"/> class.
        /// </summary>
        protected ServerItemContainerBase()
        {
            this.InitialiseChildren(new ServerItem[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemContainerBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        protected ServerItemContainerBase(string name, params ServerItem[] children)
            : base(name)
        {
            this.InitialiseChildren(children);
        }
        #endregion

        #region Public properties
        #region Children
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public IList<ServerItem> Children { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates the this item after it has been loaded.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            ValidationHelpers.CheckForDuplicateItems(this.Children, validationLog, "child");
        }
        #endregion

        #region ListProjects()
        /// <summary>
        /// Lists the projects within this item.
        /// </summary>
        /// <returns>The projects within this item.</returns>
        public override IEnumerable<Project> ListProjects()
        {
            var projects = this.Children
                .SelectMany(c => c.ListProjects());
            return projects;
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnServerChanged()
        /// <summary>
        /// Called when the server has been changed.
        /// </summary>
        protected override void OnServerChanged()
        {
            base.OnServerChanged();
            foreach (var item in this.Children)
            {
                item.Server = this.Server;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region InitialiseChildren()
        /// <summary>
        /// Initialises the children.
        /// </summary>
        /// <param name="children">The children.</param>
        private void InitialiseChildren(IEnumerable<ServerItem> children)
        {
            var collection = new ObservableCollection<ServerItem>(children);
            foreach (var child in children)
            {
                child.Host = this;
                child.Server = this.Server;
            }

            collection.CollectionChanged += this.UpdateChildren;
            this.Children = collection;
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
                child.Host = null;
                child.Server = null;
            }

            foreach (ServerItem child in e.NewItems ?? new ServerItem[0])
            {
                child.Host = this;
                child.Server = this.Server;
            }
        }
        #endregion
        #endregion
    }
}
