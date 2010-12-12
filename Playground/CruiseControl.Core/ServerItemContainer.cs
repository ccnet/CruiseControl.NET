namespace CruiseControl.Core
{
    using System.Collections.Generic;

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
