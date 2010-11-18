namespace CruiseControl.Web
{
    /// <summary>
    /// Exposes the metadata about an action handler.
    /// </summary>
    public interface IActionHandlerMetadata
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <value>The action name.</value>
        string Name { get; }
        #endregion
        #endregion
    }
}