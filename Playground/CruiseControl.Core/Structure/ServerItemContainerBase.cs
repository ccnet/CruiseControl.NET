namespace CruiseControl.Core.Structure
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Markup;

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
            }

            foreach (ServerItem child in e.NewItems ?? new ServerItem[0])
            {
                child.Host = this;
            }
        }
        #endregion
        #endregion
    }
}
