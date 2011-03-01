namespace CruiseControl.Common.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// A blank message.
    /// </summary>
    public class ServerItemList
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemList"/> class.
        /// </summary>
        public ServerItemList()
        {
            this.Children = new List<ServerItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemList"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ServerItemList(IEnumerable<ServerItem> items)
        {
            this.Children = new List<ServerItem>(items);
        }
        #endregion

        #region Public properties
        #region Children
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IList<ServerItem> Children { get; private set; }
        #endregion
        #endregion
    }
}
