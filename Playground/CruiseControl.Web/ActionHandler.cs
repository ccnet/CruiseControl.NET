namespace CruiseControl.Web
{
    using System.Web.Mvc;

    /// <summary>
    /// Handles an action.
    /// </summary>
    public abstract class ActionHandler
    {
        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes this handler.
        /// </summary>
        /// <returns>The result.</returns>
        public abstract ActionResult Execute();
        #endregion
        #endregion
    }
}