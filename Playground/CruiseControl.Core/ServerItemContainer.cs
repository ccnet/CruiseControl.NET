namespace CruiseControl.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines that a <see cref="ServerItem"/> contains children.
    /// </summary>
    public interface IServerItemContainer
    {
        #region Public properties
        #region Children
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IList<ServerItem> Children { get; }
        #endregion
        #endregion
    }
}
