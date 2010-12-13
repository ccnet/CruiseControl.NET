namespace CruiseControl.Core.Structure
{
    using System.Collections.Generic;
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
            this.Children = new List<ServerItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemContainerBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        protected ServerItemContainerBase(string name, params ServerItem[] children)
            : base(name)
        {
            this.Children = new List<ServerItem>(children);
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
    }
}
